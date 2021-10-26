using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pan : MonoBehaviour
{
    // private Rigidbody2D rb;
    decimal posX;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;
        Time.fixedDeltaTime = 1 / ((float)Application.targetFrameRate);
    }

    // private bool hasSet = false;
    void FixedUpdate()
    {
        this.transform.Translate(Vector3.right * 10 * Time.deltaTime);
        // this.transform.position += new Vector3((float)(posX + 10) * Time.deltaTime, 0, 0);
    }
}
