using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanceTracker : MonoBehaviour
{
    private Text text;

    void Start()
    {
        text = transform.GetComponentInChildren<Text>();
    }

    void Update()
    {
        text.text = DistToString(Managers.Helicopter.Distance);
    }

    private string DistToString(int dist)
    {
        if (dist > 99999)
        {
            return ((float)dist / 1000).ToString("0.0 km");
        }
        else
        {
            return dist.ToString("0 m");
        }
    }
}
