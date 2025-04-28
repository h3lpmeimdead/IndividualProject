using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _powerMenu, _pauseMenu, _tutorialMenu, _settingMenu;
    private CanvasGroup _canvasGroup;
    
    [SerializeField] private GameObject[] _tutorialPanels;

    private int _currentIndex = 0;
    
    [SerializeField] private Image _characterRun;
    [SerializeField] private Sprite[] _characterRunSprites;

    [SerializeField] private Image _characterJump;
    [SerializeField] private Sprite[] _characterJumpSprites;

    [SerializeField] private Image _characterDash;
    [SerializeField] private Sprite[] _characterDashSprites;

    [SerializeField] private Image _characterKick1;
    [SerializeField] private Sprite[] _characterKickSprites1;
    
    [SerializeField] private Image _characterKick2;
    [SerializeField] private Sprite[] _characterKickSprites2;
    
    [SerializeField] private Image _characterKick3;
    [SerializeField] private Sprite[] _characterKickSprites3;

    [SerializeField] private Image _characterPower;
    [SerializeField] private Sprite[] _characterPowerSprites;

    [SerializeField] private Image _characterIdle;
    [SerializeField] private Sprite[] _characterIdleSprites;

    [SerializeField] private Image _power;
    [SerializeField] private Sprite[] _powerSprites;

    private float _frameRate = 0.1f;
    private int _currentFrame;
    private float _timer = 0f;

    [SerializeField] private AudioClip _pauseClip, _startClip;
    [SerializeField] private AudioClip[] _bgmClips;

    private void Start()
    {
        _canvasGroup = _powerMenu.GetComponent<CanvasGroup>();
        _powerMenu.SetActive(true);
        _pauseMenu.SetActive(false);
        _tutorialMenu.SetActive(false);
        _settingMenu.SetActive(false);
        SoundEffectManager.Instance.PlaySoundEffect(_startClip, transform, 1);
        MusicManager.Instance.PlayRandomSoundEffect(_bgmClips, transform, 1);
    }
    

    private void Update()
    {
        CheckPause();
        _timer += Time.unscaledDeltaTime;
        if (_timer >= _frameRate)
        {
            _timer -= _frameRate;
            
            _currentFrame = (_currentFrame + 1) % _characterIdleSprites.Length;
            
            if (_characterRunSprites.Length > _currentFrame)
                _characterRun.sprite = _characterRunSprites[_currentFrame];
            if (_characterJumpSprites.Length > _currentFrame)
                _characterJump.sprite = _characterJumpSprites[_currentFrame];
            if (_characterDashSprites.Length > _currentFrame)
                _characterDash.sprite = _characterDashSprites[_currentFrame];
            if (_characterKickSprites1.Length > _currentFrame)
                _characterKick1.sprite = _characterKickSprites1[_currentFrame];
            if (_characterKickSprites2.Length > _currentFrame)
                _characterKick2.sprite = _characterKickSprites2[_currentFrame];
            if (_characterKickSprites3.Length > _currentFrame)
                _characterKick3.sprite = _characterKickSprites3[_currentFrame];
            if(_characterPowerSprites.Length > _currentFrame)
                _characterPower.sprite = _characterPowerSprites[_currentFrame];
            if(_characterIdleSprites.Length > _currentFrame)
                _characterIdle.sprite = _characterIdleSprites[_currentFrame];
            if(_powerSprites.Length > _currentFrame)
                _power.sprite = _powerSprites[_currentFrame];
        }
    }

    void CheckPause()
    {
        if (InputManager.PauseWasPressed || EnemyInputManager.PauseWasPressed)
        {
            Time.timeScale = 0;
            _canvasGroup.alpha = .5f;
            _pauseMenu.SetActive(true);
            SoundEffectManager.Instance.PlaySoundEffect(_pauseClip, transform, 1);
        }
    }

    public void Resume()
    {
        Time.timeScale = 1;
        _canvasGroup.alpha = 1f;
        _pauseMenu.SetActive(false);
        SoundEffectManager.Instance.PlaySoundEffect(_pauseClip, transform, 1);
    }

    public void PlayerSelect()
    {
        LevelManager.Instance.LoadScene(1, "CrossFade");
        _settingMenu.SetActive(false);
        _tutorialMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        _powerMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Home()
    {
        LevelManager.Instance.LoadScene(0, "CrossFade");
        _settingMenu.SetActive(false);
        _tutorialMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        _powerMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void SoundSetting()
    {
        _settingMenu.SetActive(true);
        _pauseMenu.SetActive(false);
        _tutorialMenu.SetActive(false);
        _canvasGroup.alpha = .5f;
    }
    
    public void TutorialButton()
    {
        ShowTutorialPanel(_currentIndex);
        _settingMenu.SetActive(false);
        _tutorialMenu.SetActive(true);
        _pauseMenu.SetActive(false);
        _canvasGroup.alpha = 0.5f;
    }

    public void ShowTutorialPanel(int index)
    {
        index = _currentIndex;
        for (int i = 0; i < _tutorialPanels.Length; i++)
        {
            _tutorialPanels[i].SetActive(index == i);
        }
    }
    public void NextPanel()
    {
        if (_currentIndex < _tutorialPanels.Length - 1)
        {
            _currentIndex++;
            ShowTutorialPanel(_currentIndex);
        }
    }

    public void BackPanel()
    {
        if (_currentIndex > 0)
        {
            _currentIndex--;
            ShowTutorialPanel(_currentIndex);
        }
    }

    public void ClosePanel()
    {
        _pauseMenu.SetActive(true);
        _tutorialMenu.SetActive(false);
        _settingMenu.SetActive(false);
        _currentIndex = 0;
    }
}
