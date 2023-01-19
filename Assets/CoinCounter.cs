using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinCounter : MonoBehaviour
{
    private Text Text;
    private Text Delta;

    private float deductStartTime;
    private const float TimeBetweenDeduct = .07f;
    private int deductCount;
    private int renderedCount;
    private int pendingCount;

    void Start()
    {
        this.Text = this.transform.Find("Text").GetComponent<Text>();
        this.Delta = this.transform.Find("Delta").GetComponent<Text>();
        this.renderedCount = GameState.Player.GemCount;
        Text.text = renderedCount.ToString();
        Delta.gameObject.SetActive(false);
    }

    void Update()
    {
        if (GameState.Player.GemCount > renderedCount + pendingCount)
        {
            pendingCount += GameState.Player.GemCount - (renderedCount + pendingCount);
            deductStartTime = Time.time + .75f;
            Delta.text = $"+ {pendingCount}";
            Delta.gameObject.SetActive(true);
        }

        if (pendingCount > 0 && Time.time > deductStartTime)
        {
            pendingCount -= 1;
            renderedCount += 1;
            deductStartTime += TimeBetweenDeduct;

            if (pendingCount <= 0)
            {
                Delta.gameObject.SetActive(false);
            }
            else
            {
                Delta.gameObject.SetActive(true);
            }

            Delta.text = $"+ {pendingCount}";
            Text.text = renderedCount.ToString();
        }
    }
}
