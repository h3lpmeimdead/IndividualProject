using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class SecondCharacterDataBase : ScriptableObject
{
    [SerializeField] private SecondCharacter[] _characters;
    public int CharacterCount { get =>  _characters.Length; }

    public SecondCharacter GetCharacter(int index)
    {
        return _characters[index];
    }
}
