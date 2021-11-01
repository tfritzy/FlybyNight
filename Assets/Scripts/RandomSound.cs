using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSound : MonoBehaviour
{
    public float Variance = .3f;

    public AudioClip[] Clips;

    void Start()
    {
        AudioSource audioSource = this.GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(audioSource.pitch - Variance, audioSource.pitch + Variance);
        audioSource.clip = Clips[Random.Range(0, Clips.Length)];
        audioSource.Play();
    }
}
