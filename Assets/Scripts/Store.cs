using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    public Transform ScrollContent;
    public Transform PreviewPos;
    public Transform PurchaseButton;
    public List<SkinType> Helicoptors = new List<SkinType>
    {
        SkinType.Default, SkinType.MechaCoptor, SkinType.NightCoptor, SkinType.SteamPunk,
    };
    private SkinType selectedSkin;
    private Color Gold = ColorExtensions.Create("#FFC84D");
    private Color ActivePurchaseButtonColor = ColorExtensions.Create("#CCF9FF");

    struct SkinDetails
    {
        public int Price;
        public string Name;
        public SkinType Type;
    };

    private Dictionary<SkinType, SkinDetails> Skins = new Dictionary<SkinType, SkinDetails>
    {
        {
            SkinType.Default,
            new SkinDetails {Type = SkinType.Default, Price = 0, Name = "Wisp"}
        },
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
            new SkinDetails {Type = SkinType.NightCoptor, Price = 5000, Name = "Night Hawk"}
        },
    };

    void Start()
    {
        SelectSkin(GameState.Player.SelectedSkin.ToString());
        InitPurchaseButtons();
    }

    private void InitPurchaseButtons()
    {
        int i = 0;
        foreach (SkinDetails skinDetails in Skins.Values)
        {
            var button = ScrollContent.GetChild(i);
            BuildUiHeli(skinDetails.Type, button, false);
            i += 1;
        }

        for (; i < ScrollContent.childCount; i++)
        {
            ScrollContent.GetChild(i).gameObject.SetActive(false);
        }
    }

    private GameObject BuildUiHeli(SkinType skinType, Transform parent, bool forPreviewBox)
    {
        var newHeli = GameObject.Instantiate(
            Managers.GetHelicoptorBody(skinType),
            Vector3.zero,
            Quaternion.Euler(0, 0, -10),
            parent);
        var previewScript = newHeli.AddComponent<PreviewHelicopter>();
        previewScript.Init(moves: forPreviewBox);

        if (forPreviewBox)
        {
            newHeli.transform.localScale = Vector3.one * 500f;
        }
        else
        {
            newHeli.transform.localScale = Vector3.one * 150f;
        }

        return newHeli;
    }

    private GameObject previewHeli;
    public void SelectSkin(string type)
    {
        SkinType parsedType = (SkinType)System.Enum.Parse(typeof(SkinType), type);
        selectedSkin = parsedType;
        Destroy(previewHeli);
        this.previewHeli = BuildUiHeli(selectedSkin, PreviewPos, true);
        FormatPurchaseButton();
    }

    public void Purchase()
    {
        if (GameState.Player.GemCount >= Skins[selectedSkin].Price)
        {
            GameState.Player.GemCount -= Skins[selectedSkin].Price;
            GameState.Player.PurchasedSkins.Add(selectedSkin);
        }
    }

    private void FormatPurchaseButton()
    {
        if (!GameState.Player.PurchasedSkins.Contains(selectedSkin))
        {
            PurchaseButton.GetComponent<Image>().color = Gold;

            Text text = PurchaseButton.GetComponentInChildren<Text>();
            text.text = Skins[selectedSkin].Price.ToString();
            text.color = Gold;
        }
        else if (GameState.Player.SelectedSkin == selectedSkin)
        {
            PurchaseButton.GetComponent<Image>().color = ActivePurchaseButtonColor;
            Text text = PurchaseButton.GetComponentInChildren<Text>();
            text.text = "Equipped";
            text.color = ActivePurchaseButtonColor;
        }
        else
        {
            PurchaseButton.GetComponent<Image>().color = ActivePurchaseButtonColor;
            Text text = PurchaseButton.GetComponentInChildren<Text>();
            text.text = "Equip";
            text.color = ActivePurchaseButtonColor;
        }
    }
}