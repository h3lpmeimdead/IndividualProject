using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class CharacterDataBase : ScriptableObject
{
    [SerializeField] private Character[] _characters;
    public int CharacterCount { get =>  _characters.Length; }

    public Character GetCharacter(int index)
    {
        return _characters[index];
    }
}
