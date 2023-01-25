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
        SettingsWindow.gameObject.SetActive(false);
    }

    void Start()
    {
        AudioManager.SetSFXLevel(GameState.Player.SFXLevel);
        AudioManager.SetMusicLevel(GameState.Player.MusicLevel);
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
        MusicSlider.SetValueWithoutNotify(GameState.Player.MusicLevel);
        SFXSlider.SetValueWithoutNotify(GameState.Player.SFXLevel);
    }

    public void OpenSettings()
    {
        SettingsWindow.gameObject.SetActive(true);
        InitSettingsWindow();
        SettingsWindow.GetComponent<Animator>().SetBool("IsOpen", true);
    }

    public void CloseSettings()
    {
        GameState.Save();
        SettingsWindow.GetComponent<Animator>().SetBool("IsOpen", false);
    }

    private void SetupAudioSettingsButtons()
    {

    }

    private void SetSettingsButtonIcon(GameObject button, bool isOn)
    {
        button.transform.Find("On").gameObject.SetActive(isOn);
        button.transform.Find("Off").gameObject.SetActive(!isOn);
    }

    public void SetSFXLevel(float value)
    {
        AudioManager.SetSFXLevel(value);
        GameState.Player.SFXLevel = value;
    }

    public void SetMusicLevel(float value)
    {
        AudioManager.SetMusicLevel(value);
        GameState.Player.MusicLevel = value;
    }

    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://tfritzy.github.io/FlyByNightPrivacyPolicy/");
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

    public void TakeOff()
    {
        Managers.Helicopter.TakeOff();
    }

    public void Retry()
    {
        Managers.Helicopter.Resurrect();
    }
}
