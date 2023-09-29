using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomIntervalAudio : MonoBehaviour
{
    public float minSeconds = 1f;
    public float maxSeconds = 5f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Invoke("PlaySound", GetRandomInterval());
    }

    private void PlaySound()
    {
        audioSource.Play();
        Invoke("PlaySound", GetRandomInterval());
    }

    private float GetRandomInterval()
    {
        return Random.Range(minSeconds, maxSeconds);
    }
}
