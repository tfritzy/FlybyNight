using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewHelicopter : MonoBehaviour
{
    Vector3 startPos;
    public GameObject Blade;
    public GameObject Rotor;
    Vector3 movementProportions = new Vector3(.5f, 1f);

    void Start()
    {
        this.startPos = this.transform.position;
    }

    void Update()
    {
        this.transform.position =
            this.startPos +
            (movementProportions * 20f) *
                (Mathf.Sin(Time.time) +
                 //  Mathf.Sin(.5f * Time.time) +
                 Mathf.Sin(10 * Time.time) * .2f +
                 Mathf.Sin(5 * Time.time) * .2f);

        Vector3 bladeRot = Blade.transform.rotation.eulerAngles;
        bladeRot.z += Time.deltaTime * 720f;
        Blade.transform.rotation = Quaternion.Euler(bladeRot);

        Vector3 rotorRot = Rotor.transform.rotation.eulerAngles;
        rotorRot.z += Time.deltaTime * 1100f;
        Rotor.transform.rotation = Quaternion.Euler(rotorRot);
    }
}
