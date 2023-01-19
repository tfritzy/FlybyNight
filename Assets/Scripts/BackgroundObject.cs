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
        lastTargetPos = Managers.CameraFollow.transform.position;
        zDistToTarget = this.transform.position.z - Managers.CameraFollow.transform.position.z;
        parallaxFactor = Mathf.Atan(zDistToTarget / 3) / (Mathf.PI / 2);
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isInactive)
        {
            return;
        }

        float delta = Managers.CameraFollow.transform.position.x - lastTargetPos.x;
        delta *= parallaxFactor;
        this.transform.position += Vector3.right * delta;
        lastTargetPos = Managers.CameraFollow.transform.position;

        while (Managers.CameraFollow.transform.position.x - this.transform.position.x > LAGGING_FLIP_DELTA)
        {
            this.transform.position = this.transform.position + Vector3.right * (FORWARD_FLIP_TO_DELTA + LAGGING_FLIP_DELTA);
        }

        while (this.transform.position.x - Managers.CameraFollow.transform.position.x > FORWARD_FLIP_TO_DELTA)
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
            color = Color.HSVToRGB(h, .2f, .7f);
            spriteRenderer.color = color;
        }
    }
}
