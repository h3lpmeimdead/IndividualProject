using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class InformationData : MonoBehaviour
{
    [SerializeField] private GameObject _player1StatsPanel, _player2StatsPanel, _canvas;
    
    [SerializeField] private AudioClip _clickClip;
    
    private void Start()
    {
        _player1StatsPanel.SetActive(false);
        _player2StatsPanel.SetActive(false);
        _canvas.SetActive(true);
    }
    
    
    public void InformationPlayer1()
    {
        _player1StatsPanel.SetActive(true);
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
    }

    public void InformationPlayer2()
    {
        _player2StatsPanel.SetActive(true);
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
    }


    public void CloseInformationPlayer1()
    {
        _player1StatsPanel.SetActive(false);
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
    }

    public void CLoseInformationPlayer2()
    {
        _player2StatsPanel.SetActive(false);
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
    }
    
    public void HomeButton(int index)
    {
        LevelManager.Instance.LoadScene(index, "CrossFade");
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
        _canvas.SetActive(false);
    }

    public void StartGame(int index)
    {
        LevelManager.Instance.LoadScene(index, "CrossFade");
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
        _canvas.SetActive(false);
    }
}
