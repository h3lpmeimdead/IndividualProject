using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SecondCharacterManager : MonoBehaviour
{
    [SerializeField] private SecondCharacterDataBase _characterDB;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private AbilitySO _power;
    [SerializeField] private AIMovementStats _aiMovementStats;
    
    [SerializeField] private TMP_Text _speedText2, _jumpText2, _noJumpText2, _dashText2, _powerText2;

    private int _selectedOption = 0;
    
    [SerializeField] private AudioClip _selectClip;

    private void Start()
    {
        if(!PlayerPrefs.HasKey("SecondSelectedOption"))
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
        SoundEffectManager.Instance.PlaySoundEffect(_selectClip, transform, 1);

        Save();
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
        SecondCharacter character = _characterDB.GetCharacter(selectedOption);
        _spriteRenderer.sprite = character.CharacterSprite;
        _nameText.text = character.CharacterName;
        _spriteRenderer.color = character.Color;
        _power = character.Power;
        _aiMovementStats = character.AIMovementStats;
        _speedText2.text = _aiMovementStats.MaxWalkSpeed.ToString();
        _jumpText2.text = _aiMovementStats.JumpHeight.ToString();
        _noJumpText2.text = _aiMovementStats.NumberofJumpsAllowed.ToString();
        _dashText2.text = _aiMovementStats.DashSpeed.ToString();
        _powerText2.text = _aiMovementStats.Power.ToString();
    }

    private void Load()
    {
        _selectedOption = PlayerPrefs.GetInt("SecondSelectedOption");
    }

    private void Save()
    {
        PlayerPrefs.SetInt("SecondSelectedOption", _selectedOption);
    }
}
