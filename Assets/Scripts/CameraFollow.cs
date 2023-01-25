using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour
{
    public Vector3 Offset;
    public float ShakeAmount;
    public Image FadeToBlackSprite;
    public State TrackingState;
    private float shakeMagnitude;
    private System.Random random;
    private float dampingSpeed = 2f;
    private Vector3 initialShakePos;
    private int MIN_TELEPORT_DISTANCE = 40;

    private Vector3 targetPos => MutedY(Managers.Helicopter.transform.position) - Offset;

    public enum State
    {
        ExactTracking,
        Teleporting,
        Panning,
        Shaking,
    };

    void Start()
    {
        random = new System.Random();
        this.TrackingState = State.ExactTracking;
        ForceMove();
    }

    void FixedUpdate()
    {
        switch (this.TrackingState)
        {
            case (State.ExactTracking):
                ExactTrack();
                return;
            case (State.Teleporting):
                Teleport();
                return;
            case (State.Panning):
                Pan();
                return;
            case (State.Shaking):
                Shake();
                return;
            default:
                throw new System.Exception("Unknown state: " + this.TrackingState);
        }
    }

    public void ForceMove()
    {
        this.transform.position = targetPos;
        this.GetComponent<InterpolatedTransform>().ForgetPreviousTransforms();
    }

    private void ExactTrack()
    {
        Vector3 diff = targetPos - this.transform.position;
        if (diff.magnitude < 1f)
        {
            this.transform.position = targetPos;
        }
        else if (diff.magnitude < MIN_TELEPORT_DISTANCE)
        {
            this.TrackingState = State.Panning;
        }
        else
        {
            this.TrackingState = State.Teleporting;
        }
    }

    private void Teleport()
    {
        Vector3 diff = targetPos - this.transform.position;
        if (diff.magnitude > 1f)
        {
            if (Managers.FadeToBlackScreen.State == FadeToBlack.TransitionState.Dark)
            {
                this.transform.position = targetPos;
                this.GetComponent<InterpolatedTransform>().ForgetPreviousTransforms();
            }
            else if (Managers.FadeToBlackScreen.State == FadeToBlack.TransitionState.Light)
            {
                Managers.FadeToBlackScreen.Darken(() => { });
            }
        }
        else
        {
            if (Managers.FadeToBlackScreen.State == FadeToBlack.TransitionState.Light)
            {
                this.TrackingState = State.ExactTracking;
            }
            else if (Managers.FadeToBlackScreen.State == FadeToBlack.TransitionState.Dark)
            {
                Managers.FadeToBlackScreen.Lighten();
            }
        }
    }

    private Vector3 panVelocity;
    private const float panTime = .05f;
    private void Pan()
    {
        Vector3 diff = targetPos - this.transform.position;
        if (diff.magnitude < .01f)
        {
            this.TrackingState = State.ExactTracking;
        }
        else if (diff.magnitude > MIN_TELEPORT_DISTANCE)
        {
            this.TrackingState = State.Teleporting;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref panVelocity, panTime, 80f);
        }
    }

    public void Shake()
    {
        if (shakeMagnitude > 0)
        {
            Vector3 randomUnitSphere = new Vector3(
                (float)random.NextDouble(),
                (float)random.NextDouble());
            transform.localPosition = initialShakePos + randomUnitSphere * shakeMagnitude;
            shakeMagnitude -= Time.fixedDeltaTime * dampingSpeed;
        }
        else
        {
            this.TrackingState = State.ExactTracking;
        }
    }

    public void TriggerShake()
    {
        this.TrackingState = State.Shaking;
        shakeMagnitude = ShakeAmount;
        initialShakePos = this.transform.position;
    }

    private static Vector3 MutedY(Vector3 vector)
    {
        vector.y *= .25f;
        return vector;
    }
}
