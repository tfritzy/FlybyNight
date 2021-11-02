using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerData
{
    public Dictionary<DifficultySetting, int> HighestDistanceUnlocked;
    public int GemCount;
    public DifficultySetting SelectedDifficulty;

    public PlayerData()
    {
        HighestDistanceUnlocked = new Dictionary<DifficultySetting, int>();
    }

    public int GetHighestRegionUnlocked()
    {
        if (HighestDistanceUnlocked.ContainsKey(this.SelectedDifficulty))
        {
            return HighestDistanceUnlocked[this.SelectedDifficulty] / Constants.DISTANCE_BETWEEN_SAVES;
        }

        return 0;
    }

    public int GetHighestDistanceReached()
    {
        if (HighestDistanceUnlocked.ContainsKey(this.SelectedDifficulty))
        {
            return HighestDistanceUnlocked[this.SelectedDifficulty];
        }

        return 0;
    }
}