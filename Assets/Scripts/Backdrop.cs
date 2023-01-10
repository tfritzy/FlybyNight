using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backdrop : MonoBehaviour
{
    public SpriteRenderer[] Sky;

    private SpriteRenderer moonRenderer;
    private Vector3 pos;
    private float xOffset;

    void Awake()
    {
        moonRenderer = this.GetComponent<SpriteRenderer>();
        pos = this.transform.position;
        xOffset = this.transform.position.x - Managers.Camera.transform.position.x;
        SetColor();
    }

    private float TIME_BETWEEN_COLOR_UPDATES = .3f;
    float lastColorUpdateTime;
    void Update()
    {
        pos.x = Managers.Camera.transform.position.x + xOffset;
        this.transform.position = pos;

        if (Time.time < lastColorUpdateTime + TIME_BETWEEN_COLOR_UPDATES)
        {
            return;
        }
        lastColorUpdateTime = Time.time;

        SetColor();
    }

    public void SetColor()
    {
        Color color = GridManager.GetColorForColumn(Managers.Helicopter.Distance);
        foreach (SpriteRenderer renderer in Sky)
        {
            renderer.color = color;
        }

        Managers.GridManager.Tilemap.color = color;

        Color.RGBToHSV(color, out float h, out float s, out float v);

        moonRenderer.color = Color.HSVToRGB(h, .2f, 1f);
    }
}
