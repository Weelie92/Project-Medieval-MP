using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerPlatform : MonoBehaviour
{
    public List<AudioClip> buildupSounds;
    public List<AudioClip> explosionSounds;

    public AudioSource buildupSource;
    public AudioSource explosionSource;

    public void PlayBuildupSound()
    {
        if (explosionSource.isPlaying)
        {
            explosionSource.Stop();
        }

        int index = Random.Range(0, buildupSounds.Count);
        AudioClip selectedClip = buildupSounds[index];
        buildupSource.clip = selectedClip;
        buildupSource.Play();
    }

    public void PlayExplosionSound()
    {
        if (buildupSource.isPlaying)
        {
            buildupSource.Stop();
        }

        int index = Random.Range(0, explosionSounds.Count);
        AudioClip selectedClip = explosionSounds[index];
        explosionSource.clip = selectedClip;
        explosionSource.Play();
    }
}
