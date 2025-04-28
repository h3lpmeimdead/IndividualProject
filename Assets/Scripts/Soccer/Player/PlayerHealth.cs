using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : Singleton<PlayerHealth>, IDamageable
{
    [SerializeField] private float _maxHealth = 15f;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _healthScaleFactor;
    [SerializeField] private float _knockbackMultiplier;
    public float CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
    public float KnockbackMultiplier { get => _knockbackMultiplier; set => _knockbackMultiplier = value; }
    public bool HasTakenDamage { get; set; }

    private bool _isStunned = false;

    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        ResetHealth();
    }

    public void Damage(float damageAmount)
    {
        HasTakenDamage = true;
        _currentHealth -= damageAmount;

        if (_currentHealth <= 0)
        {
            Stun(5);
            _currentHealth = _maxHealth;
        }

        _knockbackMultiplier = 1 + ((_maxHealth - _currentHealth) / _maxHealth) * _healthScaleFactor;
    }

    private IEnumerator Stunned()
    {
        _rb.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(5);
        _rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public void Stun(float duration)
    {
        if (!_isStunned)
        {
            StartCoroutine(StunCoroutine(duration));
        }
    }

    private IEnumerator StunCoroutine(float duration)
    {
        _isStunned = true;

        _rb.velocity = new Vector2(0, _rb.velocity.y);

        yield return new WaitForSeconds(duration);

        _isStunned = false;
    }

    public void ResetHealth()
    {
        _currentHealth = _maxHealth;
    }
}
