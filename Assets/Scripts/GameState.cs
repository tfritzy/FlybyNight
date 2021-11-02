using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class GameState
{
    private static PlayerData _player;
    public static PlayerData Player
    {
        get
        {
            if (_player == null)
            {
                Load();
            }

            return _player;
        }
        set
        {
            _player = value;
        }
    }

    private static readonly string saveFilePath = Application.persistentDataPath + "/playerData.json";
    public static void Load()
    {
        Player = FileEncryptor.ReadFile(saveFilePath);
    }

    public static async Task Save()
    {
        await FileEncryptor.WriteFile(saveFilePath, Player);
    }
}