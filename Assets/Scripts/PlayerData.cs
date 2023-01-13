using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public int HighestDistanceUnlocked;
    public int GemCount;
    private const DifficultySetting difficultySetting = DifficultySetting.Casual;
    public float SFXLevel;
    public float MusicLevel;

    public int GetHighestRegionUnlocked()
    {
        return HighestDistanceUnlocked / Constants.DISTANCE_BETWEEN_SAVES;
    }

    public int GetFirePosOfHighestRegion()
    {
        return GetHighestRegionUnlocked() * Constants.DISTANCE_BETWEEN_SAVES;
    }
}