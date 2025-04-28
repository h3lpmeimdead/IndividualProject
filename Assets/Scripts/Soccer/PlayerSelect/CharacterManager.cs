using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.FullSerializer;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private CharacterDataBase _characterDB;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private AbilitySO _power;
    [SerializeField] private PlayerMovementStats _playerMovementStats;

    [SerializeField] private TMP_Text _speedText1, _jumpText1, _noJumpText1, _dashText1, _powerText1;

    private int _selectedOption = 0;

    [SerializeField] private AudioClip _selectClip;
    
    private void Start()
    {
        if(!PlayerPrefs.HasKey("SelectedOption"))
        {
            _selectedOption = 0;
        }
        else
        {
            Load();
        }
        UpdateCharacter(_selectedOption);
    }

    public void NextOption()
    {
        _selectedOption++;
        if(_selectedOption >= _characterDB.CharacterCount)
        {
            _selectedOption = 0;
        }

        UpdateCharacter(_selectedOption);
        Save();
        
        SoundEffectManager.Instance.PlaySoundEffect(_selectClip, transform, 1);
    }

    public void BackOption()
    {
        _selectedOption--;

        if(_selectedOption < 0)
        {
            _selectedOption = _characterDB.CharacterCount - 1;
        }

        UpdateCharacter(_selectedOption);
        SoundEffectManager.Instance.PlaySoundEffect(_selectClip, transform, 1);
        Save();
    }

    private void UpdateCharacter(int selectedOption)
    {
        Character character = _characterDB.GetCharacter(selectedOption);
        _spriteRenderer.sprite = character.CharacterSprite;
        _nameText.text = character.CharacterName;
        _spriteRenderer.color = character.Color;
        _power = character.Power;
        _playerMovementStats = character.PlayerMovementStats;
        _speedText1.text = _playerMovementStats.MaxWalkSpeed.ToString();
        _jumpText1.text = _playerMovementStats.JumpHeight.ToString();
        _noJumpText1.text = _playerMovementStats.NumberofJumpsAllowed.ToString();
        _dashText1.text = _playerMovementStats.DashSpeed.ToString();
        _powerText1.text = _playerMovementStats.Power.ToString();
    }

    private void Load()
    {
        _selectedOption = PlayerPrefs.GetInt("SelectedOption");
    }

    private void Save()
    {
        PlayerPrefs.SetInt("SelectedOption", _selectedOption);
    }
    
}
