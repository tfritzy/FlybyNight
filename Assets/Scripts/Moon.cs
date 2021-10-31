using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{
    public SpriteRenderer[] Sky;

    private SpriteRenderer moonRenderer;
    private Vector3 pos;
    private float xOffset;

    void Start()
    {
        moonRenderer = this.GetComponent<SpriteRenderer>();
        pos = this.transform.position;
        xOffset = this.transform.position.x - Managers.Camera.transform.position.x;
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

        Color color = GridManager.GetColorForColumn(Managers.Helicopter.Distance);
        // foreach (SpriteRenderer renderer in Sky)
        // {
        //     renderer.color = color;
        // }

        moonRenderer.color = ColorExtensions.Lighten(color, .5f);
    }
}
