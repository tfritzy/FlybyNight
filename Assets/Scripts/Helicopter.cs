using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    public HelicopterState State;
    public GameObject Blades;
    public GameObject ExplosionPrefab;
    public int Distance => (int)(this.transform.position.x / Constants.BLOCK_WIDTH);
    private static readonly Vector3 UP_FORCE = new Vector3(0, 25, 0);
    private static readonly Vector3 GRAVITY_FORCE = new Vector3(0, -12.5f, 0);
    public float Fuel { get; private set; }

    private static readonly Vector3 START_VELOCITY = new Vector3(7, 3, 0);
    private Rigidbody2D rb;
    private SpriteRenderer[] bodyParts;
    private const float FUEL_BURN_RATE_PERCENT_PER_S = .25f;

    // Blade variables
    private const float MAX_BLADE_A_VEL = 2000;
    private const float MIN_BLADE_A_VEL = 500;
    private const float BLADE_A_VEL_DECEL = 300;
    private const float BLADE_A_VEL_ACCEL = 600;
    private float bladesAngularVelocity;
    private Vector3 bladesRotation;
    private readonly Quaternion flyingUpRotation = Quaternion.Euler(0, 0, -10);
    private readonly Quaternion driftingDownRotation = new Quaternion();

    // Chopping SFX variables
    private AudioSource choppingSound;
    private float targetChoppingPitch;
    private const float actionTargetPitch = 2f;
    private const float driftingTargetPitch = 1.5f;
    private const float maxPitchChangePerSecond = 2f;
    private const float MAX_UP_FORCE_TIME_SINGLE_FRAME_S = 1f / 15f; // 15 fps
    private bool needsToSave;
    private Vector3 velocity;
    private bool isFrozen;

    public enum HelicopterState
    {
        Hovering,
        Flying,
        Dead,
    }

    void Start()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.isFrozen = true;
        this.choppingSound = this.GetComponent<AudioSource>();
        this.State = HelicopterState.Hovering;
        this.bladesRotation = this.Blades.transform.rotation.eulerAngles;
        this.bodyParts = GetComponentsInChildren<SpriteRenderer>();
        Resurrect();
    }

    public void TakeOff()
    {
        if (Managers.Camera.TrackingState != CameraFollow.State.ExactTracking)
        {
            return;
        }

        this.velocity = START_VELOCITY;
        this.isFrozen = false;
        Managers.UIManager.SetUIForHelicopterFlying();
        this.State = HelicopterState.Flying;
    }

    public void ChangeDifficulty()
    {
        this.Resurrect();
    }

    public void Resurrect()
    {
        int x = GameState.Player.GetHighestRegionUnlocked() * Constants.DISTANCE_BETWEEN_SAVES;
        this.transform.position = new Vector3(x, GridManager.GetCaveMidAtPos(x), 0) * Constants.BLOCK_WIDTH;
        this.GetComponent<InterpolatedTransform>().ForgetPreviousTransforms();
        Managers.UIManager.SetUIForHelicopterHovering();
        this.State = HelicopterState.Hovering;
        this.GetComponent<TrailRenderer>().Clear();
        this.bladesAngularVelocity = MIN_BLADE_A_VEL;
        this.targetChoppingPitch = driftingTargetPitch;
        this.choppingSound.pitch = this.targetChoppingPitch;
        Managers.Backdrop.SetColor();
        this.Fuel = 1;

        foreach (SpriteRenderer part in this.bodyParts)
        {
            part.gameObject.SetActive(true);
        }
    }

    public void AddFuel(float amount)
    {
        this.Fuel += amount;

        if (this.Fuel > 1f)
        {
            this.Fuel = 1f;
        }
    }

    void FixedUpdate()
    {
        if (isFrozen)
        {
            return;
        }

        if (Input.GetMouseButton(0) && this.State == HelicopterState.Flying)
        {
            FlyUp();
        }
        else
        {
            DriftDown();
        }

        this.velocity += GRAVITY_FORCE * Time.deltaTime;
        this.transform.position += velocity * Time.deltaTime;
    }

    async void Update()
    {
        CheckCollision();
        AdjustChoppingPitch();
        SpinBlades();
        await SaveIfNeeded();
    }

    private void FlyUp()
    {
        if (this.Fuel <= 0)
        {
            return;
        }

        this.velocity += UP_FORCE * Time.deltaTime;
        this.targetChoppingPitch = actionTargetPitch;
        bladesAngularVelocity = Mathf.Min(MAX_BLADE_A_VEL, bladesAngularVelocity + Time.fixedDeltaTime * BLADE_A_VEL_ACCEL);
        this.transform.rotation = flyingUpRotation;
        this.Fuel -= Time.deltaTime * FUEL_BURN_RATE_PERCENT_PER_S;
    }

    private void DriftDown()
    {
        this.targetChoppingPitch = driftingTargetPitch;
        bladesAngularVelocity = Mathf.Max(MIN_BLADE_A_VEL, bladesAngularVelocity - Time.fixedDeltaTime * BLADE_A_VEL_DECEL);
        this.transform.rotation = driftingDownRotation;
    }

    private void SpinBlades()
    {
        this.bladesRotation.z += Time.fixedDeltaTime * bladesAngularVelocity;
        this.bladesRotation.z %= 360f;
        this.Blades.transform.localRotation = Quaternion.Euler(this.bladesRotation);
    }

    private void AdjustChoppingPitch()
    {
        if (targetChoppingPitch > choppingSound.pitch)
        {
            choppingSound.pitch += Time.fixedDeltaTime * maxPitchChangePerSecond;
        }
        else if (targetChoppingPitch < choppingSound.pitch)
        {
            choppingSound.pitch -= Time.fixedDeltaTime * maxPitchChangePerSecond;
        }
    }

    private void CheckCollision()
    {
        if (isFrozen)
        {
            return;
        }

        if (Physics2D.OverlapCircle(this.transform.position, .3f, Constants.Layers.Blocks))
        {
            OnCollide();
        }
    }

    private void OnCollide()
    {
        this.isFrozen = true;
        Managers.UIManager.SetUIForHelicopterDead();
        this.State = HelicopterState.Dead;
        this.SpawnExplosion(this.transform.position);
        Managers.Camera.TriggerShake();
        foreach (SpriteRenderer part in this.bodyParts)
        {
            part.gameObject.SetActive(false);
        }
        GameState.Player.HighestDistanceUnlocked[GameState.Player.SelectedDifficulty] = Distance;
        needsToSave = true;
    }

    private async Task SaveIfNeeded()
    {
        if (!needsToSave)
        {
            return;
        }

        await GameState.Save();
        // Managers.GPGManager.PostScore(Distance);
        needsToSave = false;
    }

    private void SpawnExplosion(Vector3 position)
    {
        GameObject explosion = Instantiate(ExplosionPrefab, position, new Quaternion(), null);
        explosion.transform.SetParent(null);
        Destroy(explosion, 10f);
    }
}
