using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    public Transform PreviewPos;
    public Transform PurchaseButton;
    public List<SkinType> Helicoptors = new List<SkinType>
    {
        SkinType.Default, SkinType.MechaCoptor, SkinType.NightCoptor, SkinType.SteamPunk,
    };
    private SkinType selectedSkin;

    struct SkinDetails
    {
        public int Price;
        public string Name;
        public SkinType Type;
    };

    private Dictionary<SkinType, SkinDetails> Skins = new Dictionary<SkinType, SkinDetails>
    {
        {
            SkinType.SteamPunk,
            new SkinDetails {Type = SkinType.SteamPunk, Price = 10000, Name = "Steam Coptor"}
        },
        {
            SkinType.MechaCoptor,
            new SkinDetails {Type = SkinType.MechaCoptor, Price = 3000, Name = "Mechanor"}
        },
        {
            SkinType.NightCoptor,
            new SkinDetails {Type = SkinType.NightCoptor, Price = 10000, Name = "Night Hawk"}
        },
    };

    private GameObject previewHeli;
    public void SelectSkin(string type)
    {
        SkinType parsedType = (SkinType)System.Enum.Parse(typeof(SkinType), type);
        selectedSkin = parsedType;
        Destroy(previewHeli);
        GameObject heli = Managers.GetHelicoptorBody(parsedType);
        previewHeli = GameObject.Instantiate(heli, Vector3.zero, Quaternion.Euler(0, 0, -10), PreviewPos);
        var previewScript = previewHeli.AddComponent<PreviewHelicopter>();
        previewScript.Init();
        previewHeli.transform.localScale = Vector3.one * 500f;
    }

    public void Purchase()
    {

    }
}