using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer Mixer;

    private const string SFX_GROUP = "SFXVol";
    private const string MUSIC_GROUP = "MusicVol";

    public void SetMusicLevel(float value)
    {
        Mixer.SetFloat(MUSIC_GROUP, Mathf.Log10(value) * 20);
    }

    public void SetSFXLevel(float value)
    {
        Mixer.SetFloat(SFX_GROUP, Mathf.Log10(value) * 20);
    }
}
