using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    public TransitionState State;
    public enum TransitionState
    {
        Darkening,
        Dark,
        Lightening,
        Light
    };
    private Image image;
    private const float FADE_TO_BLACK_TIME_S = 4f;
    private const float FADE_MAX_DELTA_TIME = 1f / 15f; // 15 fps;
    public delegate void OnComplete();
    private OnComplete onComplete;

    void Start()
    {
        image = this.GetComponent<Image>();
    }

    void Update()
    {
        if (this.State == TransitionState.Darkening)
        {
            Color color = image.color;
            color.a += Mathf.Min(FADE_TO_BLACK_TIME_S * Time.deltaTime, FADE_MAX_DELTA_TIME);
            image.color = color;
            if (color.a >= 1)
            {
                this.State = TransitionState.Dark;
                this?.onComplete();
                this.onComplete = null;
            }
        }
        else if (this.State == TransitionState.Lightening)
        {
            Color color = image.color;
            color.a -= Mathf.Min(FADE_TO_BLACK_TIME_S * Time.deltaTime, FADE_MAX_DELTA_TIME);
            image.color = color;
            if (color.a <= 0)
            {
                this.State = TransitionState.Light;
                if (this.onComplete != null) this.onComplete();
                this.onComplete = null;
            }
        }
    }

    public void Darken(OnComplete onComplete)
    {
        this.State = TransitionState.Darkening;
        this.onComplete = onComplete;
    }

    public void Lighten()
    {
        this.State = TransitionState.Lightening;
    }
}
