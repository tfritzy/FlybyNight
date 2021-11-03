using System.Collections;
using System.Collections.Generic;
using LeTai.Asset.TranslucentImage;
using UnityEngine;

public class BlurManager : MonoBehaviour
{
    private TranslucentImageSource source;
    private float targetBlurStrength;
    private float currentStrength;
    private const float BLUR_RATE = 100f;

    void Start()
    {
        source = GetComponent<TranslucentImageSource>();
        currentStrength = 0f;
    }

    void FixedUpdate()
    {
        if (this.targetBlurStrength != currentStrength)
        {
            float delta = Mathf.Sign(this.targetBlurStrength - currentStrength);
            this.currentStrength += delta * BLUR_RATE * Time.deltaTime;

            if (delta == -1 && this.currentStrength < this.targetBlurStrength)
            {
                this.currentStrength = this.targetBlurStrength;
            }

            if (delta == 1 && this.currentStrength > this.targetBlurStrength)
            {
                this.currentStrength = this.targetBlurStrength;
            }

            ((ScalableBlurConfig)source.BlurConfig).Strength = this.currentStrength;
        }
    }

    public void IncreaseBlur()
    {
        this.currentStrength = 0;
        this.targetBlurStrength = 18f;
    }

    public void DecreaseBlur()
    {
        this.currentStrength = 18f;
        this.targetBlurStrength = 0;
    }
}
