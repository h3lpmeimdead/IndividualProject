using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyGoalCounter : Singleton<EnemyGoalCounter>, IGoalCount
{
    [SerializeField] private float _maxEnemyHealthCounter = 5;
    [SerializeField] private float _currentEnemyHealthCounter;
    public float CurrentEnemyHealthCounter { get => _currentEnemyHealthCounter; set => _currentEnemyHealthCounter = value; }

    [SerializeField] private Image[] _ballImages;

    [SerializeField] private AudioClip _endClip, _victoryClip, _explodeClip;
    [SerializeField] private AudioClip[] _goalClips;
    [SerializeField] private GameObject _explode, _enemy;

    private bool _isGoal;
    public bool IsGoal { get => _isGoal; set => _isGoal = value; }

    private void Start()
    {
        _currentEnemyHealthCounter = _maxEnemyHealthCounter;
    }

    public void Goal()
    {
        SoundEffectManager.Instance.PlayRandomSoundEffect(_goalClips, transform, 1);
        _isGoal = true;
        _currentEnemyHealthCounter--;
        UpdateUI();
        if(_currentEnemyHealthCounter <= 0)
        {
            SoundEffectManager.Instance.PlaySoundEffect(_endClip, transform, 1);
            SoundEffectManager.Instance.PlaySoundEffect(_victoryClip, transform, 1);
            SoundEffectManager.Instance.PlaySoundEffect(_explodeClip, transform, 1);
            _explode.SetActive(true);
            _enemy.SetActive(false);
        }
    }

    public void UpdateUI()
    {
        for(int i = 0; i < _ballImages.Length; i++)
        {
            if(i >= _currentEnemyHealthCounter)
            {
                _ballImages[i].color = new Color(1, 1, 1, 0.25f);
            }
        }
    }
}
