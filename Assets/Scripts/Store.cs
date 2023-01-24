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
    private Color Gold = ColorExtensions.Create("#F8D27B");
    private Color ActivePurchaseButtonColor = ColorExtensions.Create("#CCF9FF");
    private Image PurchaseButtonOutline;
    private Text PurchaseButtonText;
    private Image PurchaseButtonGemIcon;

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
        SelectSkin(GameState.Player.SelectedSkin);
        InitPurchaseButtons();
    }

    private void InitPurchaseButtons()
    {
        int i = 0;
        foreach (SkinDetails skinDetails in Skins.Values)
        {
            var button = ScrollContent.GetChild(i);
            BuildUiHeli(skinDetails.Type, button, false);
            button.GetComponent<Button>().onClick.AddListener(() => SelectSkin(skinDetails.Type));
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
            parent.transform.position,
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
    public void SelectSkin(SkinType type)
    {
        selectedSkin = type;
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
        PurchaseButtonOutline = PurchaseButton.GetComponent<Image>();
        PurchaseButtonText = PurchaseButton.GetComponentInChildren<Text>();
        PurchaseButtonGemIcon = PurchaseButton.transform.Find("Gem").GetComponentInChildren<Image>();

        if (!GameState.Player.PurchasedSkins.Contains(selectedSkin))
        {
            PurchaseButtonOutline.color = Gold;

            PurchaseButtonText.text = string.Format("{0:#,0}", Skins[selectedSkin].Price);
            PurchaseButtonText.color = Gold;

            PurchaseButtonGemIcon.color = Gold;
            PurchaseButtonGemIcon.gameObject.SetActive(true);

            if (Skins[selectedSkin].Price >= 10000)
            {
                PurchaseButtonGemIcon.transform.localPosition = Vector3.right * -207f;
            }
            else
            {
                PurchaseButtonGemIcon.transform.localPosition = Vector3.right * -185f;
            }
        }
        else if (GameState.Player.SelectedSkin == selectedSkin)
        {
            PurchaseButtonOutline.color = ActivePurchaseButtonColor;

            PurchaseButtonText.text = "Equipped";
            PurchaseButtonText.color = ActivePurchaseButtonColor;

            PurchaseButtonGemIcon.gameObject.SetActive(false);
        }
        else
        {
            PurchaseButtonOutline.color = ActivePurchaseButtonColor;

            PurchaseButtonText.text = "Equip";
            PurchaseButtonText.color = ActivePurchaseButtonColor;

            PurchaseButtonGemIcon.gameObject.SetActive(false);
        }
    }
}