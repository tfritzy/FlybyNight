using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMarker : MonoBehaviour
{
    public int ZoneIndex;
    public GameObject Fire;
    private AudioSource source;
    private bool lit;

    void Start()
    {
        ParticleSystem fireColor = Fire.transform.Find("FireColor").GetComponent<ParticleSystem>();
        var colorOverLifetime = fireColor.colorOverLifetime;

        Color baseColor = GridManager.GetColorForColumn(this.ZoneIndex * Constants.DISTANCE_BETWEEN_SAVES);
        Color targetColor = GridManager.GetColorForColumn((this.ZoneIndex + 1) * Constants.DISTANCE_BETWEEN_SAVES);
        Gradient gradient = new Gradient();
        gradient.colorKeys = new GradientColorKey[] {
                new GradientColorKey(baseColor, .2f),
                new GradientColorKey(targetColor, .3f),
                new GradientColorKey(ColorExtensions.Lighten(targetColor, -.2f), 1f),
            };
        gradient.alphaKeys = colorOverLifetime.color.gradient.alphaKeys;
        colorOverLifetime.color = gradient;

        ParticleSystem smallFlames = fireColor.transform.Find("Small flames").GetComponent<ParticleSystem>();
        var main = smallFlames.main;
        main.startColor = baseColor;

        // if (GameState.Player.GetHighestRegionUnlocked() >= ZoneIndex)
        // {
        //     Helpers.TriggerAllParticleSystems(Fire.transform, true);
        //     lit = true;
        // }
        // else
        // {
        //     Helpers.TriggerAllParticleSystems(Fire.transform, false);
        // }

        source = this.GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (lit)
        {
            return;
        }

        if (col.CompareTag(Constants.Tags.Helicopter))
        {
            // Managers.FireworkShooter.Fire();
            // Helpers.TriggerAllParticleSystems(Fire.transform, true);
            source.Play();
            lit = true;
        }
    }
}
