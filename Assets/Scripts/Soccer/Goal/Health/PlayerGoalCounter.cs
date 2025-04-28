using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGoalCounter : Singleton<PlayerGoalCounter>, IGoalCount
{
    [SerializeField] private float _maxPlayerHealthCounter = 5;
    [SerializeField] private float _currentPlayerHealthCounter;
    public float CurrentPlayerHealthCounter { get => _currentPlayerHealthCounter; set => _currentPlayerHealthCounter = value; }

    private bool _isGoal;
    public bool IsGoal { get => _isGoal; set => _isGoal = value; }

    [SerializeField] private Image[] _ballImages;

    [SerializeField] private AudioClip _endClip, _victoryClip, _explodeClip;
    [SerializeField] private AudioClip[] _goalClips;
    
    [SerializeField] private GameObject _explode, _player;
    
    private void Start()
    {
        _currentPlayerHealthCounter = _maxPlayerHealthCounter;
    }
    public void Goal()
    {
        SoundEffectManager.Instance.PlayRandomSoundEffect(_goalClips, transform, 1);
        _isGoal = true;
        _currentPlayerHealthCounter--;
        UpdateUI();
        if (_currentPlayerHealthCounter <= 0)
        {
            SoundEffectManager.Instance.PlaySoundEffect(_endClip, transform, 1);
            SoundEffectManager.Instance.PlaySoundEffect(_victoryClip, transform, 1);
            SoundEffectManager.Instance.PlaySoundEffect(_explodeClip, transform, 1);
            _explode.SetActive(true);
            _player.SetActive(false);
        }
    }

    public void UpdateUI()
    {
        for(int i = 0; i < _ballImages.Length; i++)
        {
            if(i >= _currentPlayerHealthCounter)
            {
                _ballImages[i].color = new Color(1, 1, 1, 0.25f);
            }
        }
    }
}
