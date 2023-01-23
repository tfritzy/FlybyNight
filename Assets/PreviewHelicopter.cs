using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    void Update()
    {
        this.transform.localPosition =
            (Vector3.forward * -1f) +
            (movementProportions * 20f) *
                (Mathf.Sin(Time.time) +
                 //  Mathf.Sin(.5f * Time.time) +
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
