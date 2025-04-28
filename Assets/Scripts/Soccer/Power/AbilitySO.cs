using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySO : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private float _cooldownTimer;
    [SerializeField] private float _activeTimer;

    public float CooldownTimer { get => _cooldownTimer; set { _cooldownTimer = value; } }
    public float ActiveTimer { get => _activeTimer; set { _activeTimer = value; } }
    
    public virtual void ActivateAbillity(GameObject parent, Transform ballTransform, Rigidbody2D ballRB, float power, bool isFacingRight)
    {

    }
}
