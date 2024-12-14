using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    public AudioSource[] audioSources; // Array to hold AudioSources
    public AudioSource[] soundEffectSources; // Array to hold sound effect AudioSources

    // public UnityEvent onAudioChange;
    public void Start()
    {
        // Get all AudioSources in the scene
        // audioSources = FindObjectsOfType<AudioSource>();
        Debug.Log("Found " + audioSources.Length + " AudioSources in the scene.");
    }

    // Function to play a specific AudioSource by index
    public void PlayAudio(int index)
    {
        if (index >= 0 && index < audioSources.Length)
        {
            // Stop all audio sources
            foreach (AudioSource source in audioSources)
            {
                source.enabled = false;
            }

            // Play the selected AudioSource
            audioSources[index].enabled = true;
        }
        else
        {
            Debug.LogWarning("Audio index out of range!");
        }
    }

    public void PlaySoundEffect(int index)
    {
        if (index >= 0 && index < soundEffectSources.Length)
        {
            soundEffectSources[index].Play();
        }
        else
        {
            Debug.LogWarning("Audio index out of range!");
        }
    }

    // Function to stop all audio
    public void StopAllAudio()
    {
        foreach (AudioSource source in audioSources)
        {
            source.enabled = false;
        }
    }
}