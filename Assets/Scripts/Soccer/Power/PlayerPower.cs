using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPower : Singleton<PlayerPower>
{
    [SerializeField] private PlayerMovementStats _playerMovementStats;
    
    [SerializeField] private float _currentPower;
    [SerializeField] private float _maxPower;
    [SerializeField] private float _chargeRate;

    
    private bool _isCharging;
    private bool _isReady;
    private bool _isPowerActivated = false;
    private bool _isFull;
    private bool _isPowerUseable;
    private bool _hasPowerStack;

    public bool IsReady { get => _isReady; set { _isReady = value; } }

    [SerializeField] private Image _powerBarImage;
    [SerializeField] private Sprite[] _powerBarSprites;

    private float _activeTimer;

    [SerializeField] private Rigidbody2D _ballRB;
    [SerializeField] private Transform _ballTransform;
    [SerializeField] private float _powerShotForce = .001f;

    [SerializeField] private CharacterDataBase _characterDB;
    [SerializeField] private AbilitySO _abilitySO;
    
    [SerializeField] private GameObject _powerEffect;

    private int _selectedOption = 0;

    enum AbilityState { Ready, Active, Cooldown}
    AbilityState _abilityState = AbilityState.Cooldown;

    [SerializeField] private AudioClip _powerClip;
    
    private void Start()
    {
        _powerEffect.SetActive(false);
        if (!PlayerPrefs.HasKey("SelectedOption"))
        {
            _selectedOption = 0;
        }
        else
        {
            Load();
        }
        UpdateCharacter(_selectedOption);

        _isCharging = true;
        _isPowerActivated = true;
        _hasPowerStack = false;
        _abilityState = AbilityState.Cooldown;
        _maxPower = _playerMovementStats.Power;
    }

    private void Update()
    {
        UsePower();
    }

    private void UpdateCharacter(int selectedOption)
    {
        Character character = _characterDB.GetCharacter(selectedOption);
        _abilitySO = character.Power;
        _playerMovementStats = character.PlayerMovementStats;
    }

    private void Load()
    {
        _selectedOption = PlayerPrefs.GetInt("SelectedOption");
    }

    public void UpdatePowerUI()
    {
        float fillAmount = _currentPower / _maxPower;
        int totalFrames = _powerBarSprites.Length;
        
        int frameIndex = Mathf.FloorToInt(fillAmount * totalFrames);

        frameIndex = Mathf.Clamp(frameIndex, 0, totalFrames - 1);

        _powerBarImage.sprite = _powerBarSprites[frameIndex];
        if (PlayerGoalCounter.Instance.IsGoal)
        {
            _currentPower += 3;
        }
        if (frameIndex == _powerBarSprites.Length - 1)
        {
            _isCharging = false;
            _isPowerActivated = false;
            _isFull = true;
        }
    }
    private void ResetPower()
    {
        _currentPower = 0f;
        _powerBarImage.sprite = _powerBarSprites[0];
        _abilityState = AbilityState.Cooldown;
        _isCharging = true;
        _isPowerActivated = true;
        _isReady = false;
        _isFull = false;
        _hasPowerStack = true;
    }

    public void Charging()
    {
        if (_isCharging)
        {
            _currentPower += _chargeRate * Time.deltaTime;
            _currentPower = Mathf.Clamp(_currentPower, 0f, _maxPower);
            UpdatePowerUI();
        }
        if (_currentPower == 0)
        {
            _powerBarImage.sprite = _powerBarSprites[0];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball" && _isPowerUseable)
        {
            _isPowerUseable = false;
            _isReady = true;
            _hasPowerStack = false;
        }
    }

    public void UsePower()
    {
        if (InputManager.PowerWasPressed && _isFull)
        {
            ResetPower();
            SoundEffectManager.Instance.PlaySoundEffect(_powerClip, transform, 1);
            _powerEffect.SetActive(true);
            return;
        }
        switch (_abilityState)
        {
            case AbilityState.Ready:
                if (_isReady)
                {
                    _abilitySO.ActivateAbillity(gameObject, _ballTransform, _ballRB, _powerShotForce, PlayerMovement.Instance.IsFacingRight);
                    _powerEffect.SetActive(false);
                    _abilityState = AbilityState.Active;
                    _activeTimer = _abilitySO.ActiveTimer;
                }
                break;
            case AbilityState.Active:
                if(_activeTimer > 0)
                {
                    _activeTimer -= Time.deltaTime;
                }
                else
                {
                    _abilityState = AbilityState.Cooldown;
                    _currentPower = _abilitySO.CooldownTimer;
                    
                }
                break;
            case AbilityState.Cooldown: 
                if(_isPowerActivated)
                {
                    Charging();
                }
                if (_hasPowerStack)
                {
                    _isPowerUseable = true;
                }
                if(_isPowerUseable)
                {
                    _hasPowerStack = false;
                    _abilityState = AbilityState.Ready;
                }
                break;
        }
    }

}
