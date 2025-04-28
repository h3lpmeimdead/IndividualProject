using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCharacter : MonoBehaviour
{
    [SerializeField] private CharacterDataBase _characterDB;

    [SerializeField] private Image _image;

    private int _selectedOption = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("SelectedOption"))
        {
            _selectedOption = 0;
        }
        else
        {
            Load();
        }
        UpdateCharacterUI(_selectedOption);
    }

    private void UpdateCharacterUI(int selectedOption)
    {
        Character character = _characterDB.GetCharacter(selectedOption);
        _image.color = character.Color;
    }

    private void Load()
    {
        _selectedOption = PlayerPrefs.GetInt("SelectedOption");
    }
}
