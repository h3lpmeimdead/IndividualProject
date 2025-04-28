using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] private AudioSource _musicSource;
    
    public void PlaySoundEffect(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(_musicSource, spawnTransform.transform.position, Quaternion.identity);
        
        audioSource.clip = audioClip;
        
        audioSource.volume = volume;
        
        audioSource.Play();
        
        float clipLength = audioSource.clip.length;
        
        Destroy(audioSource.gameObject, clipLength);
    }
    
    public void PlayRandomSoundEffect(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        int random = Random.Range(0, audioClip.Length);
        
        AudioSource audioSource = Instantiate(_musicSource, spawnTransform.transform.position, Quaternion.identity);
        
        audioSource.clip = audioClip[random];
        
        audioSource.volume = volume;
        
        audioSource.Play();
        
        float clipLength = audioSource.clip.length;
        
        Destroy(audioSource.gameObject, clipLength);
    }
}
