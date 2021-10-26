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
    public Button ToggleSFXButton;
    public Button ToggleMusicButton;
    public GameObject ToggleDifficultyButton;
    // public AudioManager AudioManager;
    public GameObject LeaderboardButton;
    public Transform FPSSelectButton;

    private List<Image> colorUIImages;
    private List<Text> colorUIText;

    void Start()
    {
        FindColorDependentUI();
        SetUIForHelicopterHovering();
        CloseSettings();
        SetupAudioSettingsButtons();
        FormatDifficultyButton();
        SetGemCount(GameState.Player.GemCount);
        SetupFramerateButtons();
    }

    void Update()
    {
        SetUIColor();
    }

    public void SetUIForHelicopterFlying()
    {
        LiftoffButton.SetActive(false);
        ResurrectButton.SetActive(false);
        SettingsButton.SetActive(false);
        ToggleDifficultyButton.SetActive(false);
        LeaderboardButton.SetActive(false);
        CloseSettings();
    }

    public void SetUIForHelicopterDead()
    {
        ResurrectButton.SetActive(true);
        CloseSettings();
    }

    public void SetUIForHelicopterHovering()
    {
        LiftoffButton.SetActive(true);
        ResurrectButton.SetActive(false);
        SettingsButton.SetActive(true);
        ToggleDifficultyButton.SetActive(true);
        LeaderboardButton.SetActive(true);
        CloseSettings();
    }

    public void OpenSettings()
    {
        SettingsWindow.SetActive(true);
    }

    public void CloseSettings()
    {
        SettingsWindow.SetActive(false);
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

    public void ToggleSFX()
    {
        // AudioManager.ToggleSFX();
        SetupAudioSettingsButtons();
    }

    public void ToggleMusic()
    {
        // AudioManager.ToggleMusic();
        SetupAudioSettingsButtons();
    }

    public void SetGemCount(int value)
    {
        // this.GemCount.text = value.ToString();
    }

    public void ToggleDifficulty()
    {
        Managers.FadeToBlackScreen.Darken(() =>
        {
            if (GameState.Player.SelectedDifficulty == DifficultySetting.Casual)
            {
                GameState.Player.SelectedDifficulty = DifficultySetting.Intense;
            }
            else
            {
                GameState.Player.SelectedDifficulty = DifficultySetting.Casual;
            }

            Managers.Helicopter.ChangeDifficulty();
            Managers.GridManager.ResetGrid();
            FormatDifficultyButton();
            Managers.Camera.ForceMove();
            Managers.FadeToBlackScreen.Lighten();
        });
    }

    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://github.com/tfritzy/FlyByNightPrivacyPolicy/blob/main/PrivacyPolicy.md");
    }

    private Color intenseTextColor = ColorExtensions.Create("DB3906");
    private Color casualTextColor = ColorExtensions.Create("60CC52");
    private void FormatDifficultyButton()
    {
        Text text = ToggleDifficultyButton.transform.Find("Text").GetComponent<Text>();
        string diffName = GameState.Player.SelectedDifficulty == DifficultySetting.Casual ? "Casual" : "Intense";
        text.text = diffName;
        Color color = GameState.Player.SelectedDifficulty == DifficultySetting.Casual ? casualTextColor : intenseTextColor;
        text.color = color;
    }

    private float lastColorUpdateTime;
    private const float TIME_BETWEEN_COLOR_UPDATES = .5f;
    private void SetUIColor()
    {
        if (Time.time < lastColorUpdateTime + TIME_BETWEEN_COLOR_UPDATES)
        {
            return;
        }

        lastColorUpdateTime = Time.time;

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

    private void SetupFramerateButtons()
    {
        if (Application.targetFrameRate == 30)
        {
            Set30FPS();
        }
        else if (Application.targetFrameRate == 60)
        {
            Set60FPS();
        }
        else
        {
            Set30FPS();
        }
    }

    public void Set60FPS()
    {
        Application.targetFrameRate = 60;
        Time.fixedDeltaTime = 1 / ((float)Application.targetFrameRate);
        FormatFPSButtons();
    }

    public void Set30FPS()
    {
        Application.targetFrameRate = 30;
        Time.fixedDeltaTime = 1 / ((float)Application.targetFrameRate);
        FormatFPSButtons();
    }

    private void FormatFPSButtons()
    {
        FPSSelectButton.transform.Find("30 FPS").Find("On").gameObject.SetActive(Application.targetFrameRate == 30);
        FPSSelectButton.transform.Find("30 FPS").Find("Off").gameObject.SetActive(Application.targetFrameRate != 30);
        FPSSelectButton.transform.Find("60 FPS").Find("On").gameObject.SetActive(Application.targetFrameRate == 60);
        FPSSelectButton.transform.Find("60 FPS").Find("Off").gameObject.SetActive(Application.targetFrameRate != 60);
    }
}
