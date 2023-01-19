using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkShooter : MonoBehaviour
{
    public GameObject FireworkPrefab;
    public int NumFireworks;
    public float XRange;
    public float YRange;
    public bool fire;

    private Vector3 offsetFirePos;
    private LinkedList<float> fireFireworkTimes;

    void Start()
    {
        offsetFirePos = this.transform.position - Managers.CameraFollow.transform.position;
        fireFireworkTimes = new LinkedList<float>();
    }

    void Update()
    {
        if (fireFireworkTimes.Count > 0 && Time.time > fireFireworkTimes.First.Value)
        {
            FireSingleFirework();
            fireFireworkTimes.RemoveFirst();
        }

        if (fire)
        {
            fire = false;
            Fire();
        }
    }

    public void Fire()
    {
        this.transform.position = Managers.CameraFollow.transform.position + offsetFirePos;
        float fireTime = Time.time + .5f;
        for (int i = 0; i < NumFireworks; i++)
        {
            fireFireworkTimes.AddLast(fireTime);
            fireTime += Random.Range(0, .5f);
        }
    }

    private void FireSingleFirework()
    {
        ParticleSystem ps = Instantiate(
            FireworkPrefab,
            this.transform.position + new Vector3(Random.Range(-XRange, XRange), Random.Range(-YRange, YRange)),
            new Quaternion(),
            this.transform)
        .GetComponent<ParticleSystem>();
        Color baseColor = GridManager.GetColorForColumn(
            Random.Range(
                GameState.Player.GetHighestRegionUnlocked() * Constants.DISTANCE_BETWEEN_SAVES,
                (GameState.Player.GetHighestRegionUnlocked() + 1) * Constants.DISTANCE_BETWEEN_SAVES
            )
        );
        Gradient gradient = new Gradient();
        gradient.colorKeys = new GradientColorKey[] {
                new GradientColorKey(ColorExtensions.Lighten(baseColor, -.2f), 0),
                new GradientColorKey(ColorExtensions.Lighten(baseColor, .2f), 1),
            };
        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(gradient);
    }
}
