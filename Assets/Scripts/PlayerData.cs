using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public int HighestDistanceUnlocked;
    public int GemCount;
    private const DifficultySetting difficultySetting = DifficultySetting.Casual;
    public float SFXLevel = 1;
    public float MusicLevel = 1;
    public HashSet<SkinType> PurchasedSkins = new HashSet<SkinType> { SkinType.Default };
    public SkinType SelectedSkin;

    public int GetHighestRegionUnlocked()
    {
        return HighestDistanceUnlocked / Constants.DISTANCE_BETWEEN_SAVES;
    }

    public int GetFirePosOfHighestRegion()
    {
        return GetHighestRegionUnlocked() * Constants.DISTANCE_BETWEEN_SAVES;
    }
}