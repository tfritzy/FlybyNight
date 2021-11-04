using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer Mixer;

    private const string SFX_GROUP = "SFXVol";
    private const string MUSIC_GROUP = "MusicVol";
    public const float AUDIBLE = 0f;
    public const float MUTED = -80f;

    public void SetMusicLevel(float value)
    {
        print(value);
        print(Mixer.SetFloat(MUSIC_GROUP, Mathf.Log10(value) * 20));
    }

    public void SetSFXLevel(float value)
    {
        Mixer.SetFloat(SFX_GROUP, Mathf.Log10(value) * 20);
    }
}
