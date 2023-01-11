using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundObject : MonoBehaviour
{
    private float zDistToTarget;
    private float parallaxFactor;
    private Vector3 lastTargetPos;
    private const float LAGGING_FLIP_DELTA = 19f;
    private const float FORWARD_FLIP_TO_DELTA = 34f;
    private bool isInactive = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        lastTargetPos = Managers.Camera.transform.position;
        zDistToTarget = this.transform.position.z - Managers.Camera.transform.position.z;
        parallaxFactor = Mathf.Atan(zDistToTarget / 3) / (Mathf.PI / 2);
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isInactive)
        {
            return;
        }

        float delta = Managers.Camera.transform.position.x - lastTargetPos.x;
        delta *= parallaxFactor;
        this.transform.position += Vector3.right * delta;
        lastTargetPos = Managers.Camera.transform.position;

        while (Managers.Camera.transform.position.x - this.transform.position.x > LAGGING_FLIP_DELTA)
        {
            this.transform.position = this.transform.position + Vector3.right * (FORWARD_FLIP_TO_DELTA + LAGGING_FLIP_DELTA);
        }

        while (this.transform.position.x - Managers.Camera.transform.position.x > FORWARD_FLIP_TO_DELTA)
        {
            this.transform.position = this.transform.position - Vector3.right * (FORWARD_FLIP_TO_DELTA + LAGGING_FLIP_DELTA);
        }

        UpdateColor();
    }

    private void UpdateColor()
    {
        if (spriteRenderer != null)
        {
            Color color = GridManager.GetColorForColumn(Managers.Helicopter.Distance);
            Color.RGBToHSV(color, out float h, out float s, out float v);
            color = Color.HSVToRGB(h, s * .45f, v * .7f);
            spriteRenderer.color = color;
        }
    }
}
