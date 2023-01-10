using UnityEngine;
using UnityEngine.UI;

public class FuelGauge : MonoBehaviour
{
    private Image inner;

    void Start()
    {
        this.inner = this.transform.Find("Inner").GetComponent<Image>();
    }

    void Update()
    {
        this.inner.fillAmount = Managers.Helicopter.Fuel;
    }
}