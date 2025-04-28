using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UI_Manager : Singleton<UI_Manager>
{
    [SerializeField] private Image _characterImage;
    [SerializeField] private Image _character2Image;    
    [SerializeField] private Sprite[] _characterSprites;
    
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
    
    [SerializeField] private List<Image> _uiImages = new List<Image>();

    private float _frameRate = 0.1f;
    private int _currentFrame;
    private float _timer = 0f;

    [SerializeField] private GameObject _settingMenu, _mainMenu, _tutorialMenu, _sfxSlider, _musicSlider;
    [SerializeField] private GameObject[] _tutorialPanels;

    private int _currentIndex = 0;

    [SerializeField] private AudioClip _clickClip;
    private void Start()
    {
        _settingMenu.SetActive(false);
        _mainMenu.SetActive(true);
        _tutorialMenu.SetActive(false);
        _sfxSlider.SetActive(false);
        _musicSlider.SetActive(false);
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _frameRate)
        {
            _timer -= _frameRate;
            _currentFrame = (_currentFrame + 1) % _characterSprites.Length;
            _characterImage.sprite = _characterSprites[_currentFrame];
            _character2Image.sprite = _characterSprites[_currentFrame];
            
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

    public void StartGameButton(int index)
    {
        LevelManager.Instance.LoadScene(index, "CrossFade");
        foreach (Image images in _uiImages)
        {
            images.gameObject.SetActive(false);
        }
        _mainMenu.SetActive(false);
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
    }

    public void QuitGameButton()
    {
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
        Application.Quit();
    }

    public void ExitButton(int index)
    {
        LevelManager.Instance.LoadScene(index, "CrossFade");
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
    }

    public void SettingButton()
    {
        _settingMenu.SetActive(true);
        _mainMenu.SetActive(false);
        _tutorialMenu.SetActive(false);
        _sfxSlider.SetActive(false);
        _musicSlider.SetActive(false);
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
    }

    public void BackButton()
    {
        _settingMenu.SetActive(false);
        _mainMenu.SetActive(true);
        _tutorialMenu.SetActive(false);
        _sfxSlider.SetActive(false);
        _musicSlider.SetActive(false);
        _currentIndex = 0;
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
    }

    public void TutorialButton()
    {
        ShowTutorialPanel(_currentIndex);
        _settingMenu.SetActive(false);
        _mainMenu.SetActive(false);
        _tutorialMenu.SetActive(true);
        _sfxSlider.SetActive(false);
        _musicSlider.SetActive(false);
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
    }

    public void ShowTutorialPanel(int index)
    {
        index = _currentIndex;
        for (int i = 0; i < _tutorialPanels.Length; i++)
        {
            _tutorialPanels[i].SetActive(index == i);
        }
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
    }
    public void NextPanel()
    {
        if (_currentIndex < _tutorialPanels.Length - 1)
        {
            _currentIndex++;
            ShowTutorialPanel(_currentIndex);
        }
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
    }

    public void BackPanel()
    {
        if (_currentIndex > 0)
        {
            _currentIndex--;
            ShowTutorialPanel(_currentIndex);
        }
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
    }

    public void ClosePanel()
    {
        _settingMenu.SetActive(true);
        _mainMenu.SetActive(false);
        _tutorialMenu.SetActive(false);
        _sfxSlider.SetActive(false);
        _musicSlider.SetActive(false);
        _currentIndex = 0;
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
    }

    public void SFXSlider()
    {
        _sfxSlider.gameObject.SetActive(!_sfxSlider.gameObject.activeSelf);
        _musicSlider.SetActive(false);
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
    }

    public void MusicSlider()
    {
        _musicSlider.gameObject.SetActive(!_musicSlider.gameObject.activeSelf);
        _sfxSlider.SetActive(false);
        SoundEffectManager.Instance.PlaySoundEffect(_clickClip, transform, 1);
    }
}
