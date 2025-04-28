using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;

    public void SetMasterVolume(float level)
    {
        _audioMixer.SetFloat("Master", Mathf.Log10(level) * 20);
    }

    public void SetSFxVolume(float level)
    {
        _audioMixer.SetFloat("SFX", Mathf.Log10(level) * 20);
    }

    public void SetMusicVolume(float level)
    {
        _audioMixer.SetFloat("Music", Mathf.Log10(level) * 20);
    }
}
