using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Character
{
    public string CharacterName;
    public Sprite CharacterSprite;
    public Color Color;
    public AbilitySO Power;
    public PlayerMovementStats PlayerMovementStats;
}
