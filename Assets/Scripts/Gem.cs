using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public int Value;
    public GameObject CollectionEffect;
    private bool isCollected;
    private GemColor color;

    public enum GemTier
    {
        Low,
        Mid,
        High,
    }

    public enum GemColor
    {
        Blue,
        Green,
        Purple,
        Yellow,
        Orange,
        Red,
    }

    private Dictionary<GemColor, Color> colorMap = new Dictionary<GemColor, Color>
    {
        {GemColor.Blue, ColorExtensions.Create("#399dff")},
        {GemColor.Green, ColorExtensions.Create("#48FF7E")},
        {GemColor.Purple, ColorExtensions.Create("#F34FFF")},
        {GemColor.Yellow, ColorExtensions.Create("#FFE550")},
        {GemColor.Orange, ColorExtensions.Create("#FFA140")},
        {GemColor.Red, ColorExtensions.Create("#FF4F4C")},
    };

    private GemColor GetColor(GemTier tier)
    {
        switch (GameState.Player.SelectedDifficulty)
        {
            case (DifficultySetting.Casual):
                switch (tier)
                {
                    case (GemTier.Low):
                        return GemColor.Blue;
                    case (GemTier.Mid):
                        return GemColor.Green;
                    case (GemTier.High):
                        return GemColor.Purple;
                    default:
                        throw new System.Exception("No known gem tier: " + tier);
                }
            case (DifficultySetting.Intense):
                switch (tier)
                {
                    case (GemTier.Low):
                        return GemColor.Yellow;
                    case (GemTier.Mid):
                        return GemColor.Orange;
                    case (GemTier.High):
                        return GemColor.Red;
                    default:
                        throw new System.Exception("No known gem tier: " + tier);
                }
            default:
                throw new System.Exception($"Difficulty {GameState.Player.SelectedDifficulty} not included in switch");
        }
    }

    private Dictionary<GemColor, int> valueMap = new Dictionary<GemColor, int>
    {
        {GemColor.Blue, 1},
        {GemColor.Green, 3},
        {GemColor.Purple, 5},
        {GemColor.Yellow, 3},
        {GemColor.Orange, 5},
        {GemColor.Red, 15},
    };

    public void SetTier(GemTier tier)
    {
        // this.color = GetColor(tier);
        this.Value = valueMap[this.color];
        // this.GetComponentInChildren<SpriteRenderer>().color = colorMap[this.color];
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected)
        {
            return;
        }

        if (other.CompareTag(Constants.Tags.Helicopter))
        {
            Collect();
            Color color = GridManager.GetColorForColumn(Managers.Helicopter.Distance);
            foreach (ParticleSystem ps in CollectionEffect.GetComponentsInChildren<ParticleSystem>())
            {
                var main = ps.main;
                main.startColor = Color.white;
            }

            CollectionEffect.SetActive(true);
            CollectionEffect.transform.SetParent(null);
            isCollected = true;
            Destroy(this.gameObject);
            Destroy(CollectionEffect, 10f);
        }
    }

    public void Collect()
    {
        Managers.Helicopter.AddFuel(.33f);
    }
}