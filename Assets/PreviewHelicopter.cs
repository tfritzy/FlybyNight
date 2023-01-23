using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewHelicopter : MonoBehaviour
{
    private Transform Blades;
    private Transform Rotor;
    Vector3 movementProportions = new Vector3(.5f, 1f);

    void Start()
    {
        Init();
    }

    public void Init()
    {
        this.Blades = this.transform.Find("Blades");
        this.Rotor = this.transform.Find("Rotor");

        Destroy(this.GetComponent<Helicopter>());
        Destroy(this.GetComponent<InterpolatedTransform>());
        Destroy(this.GetComponent<InterpolatedTransformUpdater>());
        Destroy(this.GetComponent<Collider2D>());

        foreach (SpriteRenderer sr in this.GetComponentsInChildren<SpriteRenderer>())
        {
            Sprite sprite = sr.sprite;
            Color color = sr.color;
            Destroy(sr);
            Image image = sr.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
        }
    }

    void Update()
    {
        this.transform.localPosition =
            (Vector3.forward * -1f) +
            (movementProportions * 20f) *
                (Mathf.Sin(Time.time) * .5f +
                 Mathf.Sin(10 * Time.time) * .2f +
                 Mathf.Sin(5 * Time.time) * .2f +
                 Mathf.Sin(20 * Time.time) * .1f);

        if (Blades != null)
        {
            Vector3 bladeRot = Blades.transform.rotation.eulerAngles;
            bladeRot.z += Time.deltaTime * 720f;
            Blades.transform.rotation = Quaternion.Euler(bladeRot);
        }

        if (Rotor != null)
        {
            Vector3 rotorRot = Rotor.transform.rotation.eulerAngles;
            rotorRot.z += Time.deltaTime * 1100f;
            Rotor.transform.rotation = Quaternion.Euler(rotorRot);
        }
    }
}
