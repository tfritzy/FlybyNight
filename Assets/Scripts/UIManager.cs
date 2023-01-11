using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject LiftoffButton;
    public GameObject ResurrectButton;
    public GameObject SettingsWindow;
    public GameObject SettingsButton;
    public GameObject DistanceTracker;
    public Text GemCount;
    public Slider SFXSlider;
    public Slider MusicSlider;
    public AudioManager AudioManager;
    public GameObject LeaderboardButton;
    public Transform FPSSelectGroup;
    public GameObject FuelGauge;

    private List<Image> colorUIImages;
    private List<Text> colorUIText;

    void Awake()
    {
        FindColorDependentUI();
        SetUIForHelicopterHovering();
        InitSettingsWindow();
        SetupAudioSettingsButtons();
        SetGemCount(GameState.Player.GemCount);
        // SetupFramerateButtons();
    }

    void Update()
    {
        UpdateColorLoop();
    }

    public void SetUIForHelicopterFlying()
    {
        LiftoffButton.SetActive(false);
        ResurrectButton.SetActive(false);
        SettingsButton.SetActive(false);
        LeaderboardButton.SetActive(false);
        FuelGauge.SetActive(true);
        CloseSettings();
    }

    public void SetUIForHelicopterDead()
    {
        ResurrectButton.SetActive(true);
        FuelGauge.SetActive(false);
        CloseSettings();
    }

    public void SetUIForHelicopterHovering()
    {
        LiftoffButton.SetActive(true);
        ResurrectButton.SetActive(false);
        SettingsButton.SetActive(true);
        LeaderboardButton.SetActive(true);
        FuelGauge.SetActive(false);
        UpdateColors();
        CloseSettings();
    }

    private void InitSettingsWindow()
    {
        SettingsWindow.gameObject.SetActive(false);
    }

    public void OpenSettings()
    {
        // Managers.Camera.GetComponent<BlurManager>().IncreaseBlur();
        SettingsWindow.gameObject.SetActive(true);
        SettingsWindow.GetComponent<Animator>().SetBool("IsOpen", true);
    }

    public void CloseSettings()
    {
        // Managers.Camera.GetComponent<BlurManager>().DecreaseBlur();
        SettingsWindow.GetComponent<Animator>().SetBool("IsOpen", false);
    }

    private void SetupAudioSettingsButtons()
    {
        // bool isMusicAudible = AudioManager.IsMusicAudible();
        // SetSettingsButtonIcon(ToggleMusicButton.gameObject, isMusicAudible);

        // bool areSFXAudible = AudioManager.AreSFXAudible();
        // SetSettingsButtonIcon(ToggleSFXButton.gameObject, areSFXAudible);
    }

    private void SetSettingsButtonIcon(GameObject button, bool isOn)
    {
        button.transform.Find("On").gameObject.SetActive(isOn);
        button.transform.Find("Off").gameObject.SetActive(!isOn);
    }

    public void SetSFXLevel(float value)
    {
        AudioManager.SetSFXLevel(value);
    }

    public void SetMusicLevel(float value)
    {
        AudioManager.SetMusicLevel(value);
    }

    public void SetGemCount(int value)
    {
        // this.GemCount.text = value.ToString();
    }

    // public void ToggleDifficulty()
    // {
    //     Managers.FadeToBlackScreen.Darken(() =>
    //     {
    //         if (GameState.Player.SelectedDifficulty == DifficultySetting.Casual)
    //         {
    //             GameState.Player.SelectedDifficulty = DifficultySetting.Intense;
    //         }
    //         else
    //         {
    //             GameState.Player.SelectedDifficulty = DifficultySetting.Casual;
    //         }

    //         Managers.Helicopter.ChangeDifficulty();
    //         Managers.Camera.ForceMove();
    //         Managers.FadeToBlackScreen.Lighten();
    //     });
    // }

    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://github.com/tfritzy/FlyByNightPrivacyPolicy/blob/main/PrivacyPolicy.md");
    }

    private float lastColorUpdateTime;
    private const float TIME_BETWEEN_COLOR_UPDATES = .5f;
    private void UpdateColorLoop()
    {
        if (Time.time < lastColorUpdateTime + TIME_BETWEEN_COLOR_UPDATES)
        {
            return;
        }

        lastColorUpdateTime = Time.time;

        UpdateColors();
    }

    private void UpdateColors()
    {
        if (Managers.Helicopter == null)
        {
            return;
        }

        Color color = GridManager.GetColorForColumn(Managers.Helicopter.Distance);

        foreach (Image image in colorUIImages)
        {
            image.color = color;
        }

        foreach (Text text in colorUIText)
        {
            text.color = color;
        }
    }

    private void FindColorDependentUI()
    {
        colorUIImages = new List<Image>();
        colorUIText = new List<Text>();

        foreach (GameObject go in GameObject.FindGameObjectsWithTag(Constants.Tags.ColoredUI))
        {
            Image image = go.GetComponent<Image>();
            if ((image != null))
            {
                colorUIImages.Add(image);
            }

            Text text = go.GetComponent<Text>();
            if (text != null)
            {
                colorUIText.Add(text);
            }
        }
    }

    // private void SetupFramerateButtons()
    // {
    //     if (Application.targetFrameRate == 30)
    //     {
    //         Set30FPS();
    //     }
    //     else if (Application.targetFrameRate == 60)
    //     {
    //         Set60FPS();
    //     }
    //     else
    //     {
    //         Set30FPS();
    //     }
    // }

    // public void Set60FPS()
    // {
    //     Application.targetFrameRate = 60;
    //     Time.fixedDeltaTime = 1f / 60f;
    //     FormatFPSButtons();
    // }

    // public void Set30FPS()
    // {
    //     Application.targetFrameRate = 30;
    //     Time.fixedDeltaTime = 1f / 30f;
    //     FormatFPSButtons();
    // }

    // private Color toggleUnselectedBaseColor = ColorExtensions.Create("#434343");
    // private Color toggleUnselectedTextColor = ColorExtensions.Create("#808080");
    // private Color toggleSelectedTextColor = Color.white;
    // private void FormatFPSButtons()
    // {
    //     Color activeColor = GridManager.GetColorForColumn(Managers.Helicopter.Distance);
    //     Transform fps30 = FPSSelectGroup.transform.Find("30 FPS");
    //     Transform fps60 = FPSSelectGroup.transform.Find("60 FPS");
    //     fps30.GetComponent<Image>().color = Application.targetFrameRate == 30 ? activeColor : toggleUnselectedBaseColor;
    //     fps60.GetComponent<Image>().color = Application.targetFrameRate == 60 ? activeColor : toggleUnselectedBaseColor;
    //     fps30.Find("Text").GetComponent<Text>().color = Application.targetFrameRate == 30 ? toggleSelectedTextColor : toggleUnselectedTextColor;
    //     fps60.Find("Text").GetComponent<Text>().color = Application.targetFrameRate == 60 ? toggleSelectedTextColor : toggleUnselectedTextColor;
    // }
}
