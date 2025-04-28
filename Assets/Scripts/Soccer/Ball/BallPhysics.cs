using System;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallPhysics : Singleton<BallPhysics>, IKickable
{
    [SerializeField] private float _baseKnockback = 20f; 
    [SerializeField] private float _knockbackGrowth = 2f; 
    [SerializeField] private float _knockbackAngle = 45f; 
    [SerializeField] private float _knockbackScalingFactor = 2f; 
    [SerializeField] private float _airDrag = 0.98f; 
    [SerializeField] private float _hitstunMultiplier = 0.05f; 

    private bool _inHitstun = false;

    private Rigidbody2D _rb;
    private Vector3 _lastVelocity;

    private CinemachineImpulseSource _impulseSource;

    [SerializeField] private float _originalWalkSpeed = 12.5f;
    [SerializeField] private float _originalRunSpeed = 20f;
    public float OriginalWalkSpeed { get => _originalWalkSpeed; private set => _originalWalkSpeed = value; }
    public float OriginalRunSpeed { get => _originalRunSpeed; private set => _originalRunSpeed = value; }

    [SerializeField] private float _slowPercentage;
    [SerializeField] private float _knockbackForce;

    [SerializeField] private float _slowDuration = 3;
    [SerializeField] private float _freezeDuration = 3;
    [SerializeField] private float _disableTimer = 3;
    Vector2 _knockbackDirection;

    [SerializeField] private PlayerMovementStats _playerMovementStats;
    [SerializeField] private AIMovementStats _aiMovementStats;

    [SerializeField] private KnockbackPhysicPlayer _knockbackPhysicPlayer;
    [SerializeField] private KnockbackPhysic _knockbackPhysic;

    [SerializeField] private AudioClip _hurtClip, _kickBallClip;
    
    [SerializeField] private GameObject _power5CharacterVFX;
    [SerializeField] private GameObject _power10CharacterVFX;
    [SerializeField] private GameObject _power2CharacterVFX;
    [SerializeField] private GameObject _power7CharacterVFX;
    [SerializeField] private GameObject _power3CharacterVFX;
    [SerializeField] private GameObject _power8CharacterVFX;
    [SerializeField] private GameObject _power4CharacterVFX;
    [SerializeField] private GameObject _power9CharacterVFX;
    public GameObject Power5CharacterVFX { get => _power5CharacterVFX; set => _power5CharacterVFX = value; }
    public GameObject Power10CharacterVFX { get => _power10CharacterVFX; set => _power10CharacterVFX = value; }
    public GameObject Power2CharacterVFX { get => _power2CharacterVFX; set => _power2CharacterVFX = value; }
    public GameObject Power7CharacterVFX { get => _power7CharacterVFX; set => _power7CharacterVFX = value; }
    public GameObject Power3CharacterVFX { get => _power3CharacterVFX; set => _power3CharacterVFX = value; }
    public GameObject Power8CharacterVFX { get => _power8CharacterVFX; set => _power8CharacterVFX = value; }
    public GameObject Power4CharacterVFX { get => _power4CharacterVFX; set => _power4CharacterVFX = value; }
    public GameObject Power9CharacterVFX { get => _power9CharacterVFX; set => _power9CharacterVFX = value; }
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _knockbackPhysic = GetComponent<KnockbackPhysic>();
        _knockbackPhysicPlayer = GetComponent<KnockbackPhysicPlayer>();
        _playerMovementStats.MaxRunSpeed = _originalRunSpeed;
        _playerMovementStats.MaxWalkSpeed = _originalWalkSpeed;
        _aiMovementStats.MaxRunSpeed = _originalRunSpeed;
        _aiMovementStats.MaxWalkSpeed = _originalWalkSpeed;
    }

    private void Update()
    {
        _lastVelocity = _rb.velocity;
    }

    void FixedUpdate()
    {
        if (_inHitstun)
        {
            _rb.velocity *= _airDrag;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var speed = _lastVelocity.magnitude;
        var direction = Vector3.Reflect(_lastVelocity.normalized, collision.contacts[0].normal);

        _rb.velocity = direction * Mathf.Max(speed, 0f);

        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
        {
            CameraShake.Instance.GloabalCameraShake(_impulseSource);
            SoundEffectManager.Instance.PlaySoundEffect(_kickBallClip, transform, 1);
        }

        #region Player 1 power
        
        if (collision.gameObject.tag == "Enemy" && PowerCharacter1.CanKnock)
        {
            Rigidbody2D enemy = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 dir = (collision.transform.position - transform.position);
            dir.y = 0f;
            _knockbackDirection = dir.normalized;
            StartCoroutine(KnockUp());
            enemy.AddForce((_knockbackDirection * _knockbackForce), ForceMode2D.Force);
        }
        
        if (collision.gameObject.tag == "Enemy" && PowerCharacter2.CanSlow)
        {
            StartCoroutine(Slow());
            _playerMovementStats.MaxWalkSpeed = _playerMovementStats.MaxWalkSpeed - (_playerMovementStats.MaxWalkSpeed * _slowPercentage);
            _playerMovementStats.MaxRunSpeed = _playerMovementStats.MaxRunSpeed - (_playerMovementStats.MaxRunSpeed * _slowPercentage);
            _power2CharacterVFX.SetActive(true);
        }

        if(collision.gameObject.tag == "Enemy" && PowerCharacter3.IsPowering)
        {
            Rigidbody2D enemy = collision.gameObject.GetComponent<Rigidbody2D>();
            _knockbackDirection = (collision.transform.position - transform.position).normalized;
            _power3CharacterVFX.SetActive(true);
            StartCoroutine(KnockUp());
            enemy.AddForce((_knockbackDirection * _knockbackForce), ForceMode2D.Force);
        }

        if(collision.gameObject.tag == "Enemy" && PowerCharacter4.CanStun)
        {
            Rigidbody2D enemy = collision.gameObject.GetComponent<Rigidbody2D>();
            enemy.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            _power4CharacterVFX.SetActive(true);
            StartCoroutine(Freeze(enemy));
        }

        if(collision.gameObject.tag == "Enemy" && PowerCharacter5.CanDisable)
        {
            GameObject enemy = collision.gameObject;
            _power5CharacterVFX.SetActive(true);
            StartCoroutine(EnableEnemy(enemy));
        }
        #endregion

        #region Player 2 power

        if (collision.gameObject.tag == "Player" && PowerCharacter6.CanKnock)
        {
            Rigidbody2D enemy = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 dir = (collision.transform.position - transform.position);
            dir.y = 0f;
            _knockbackDirection = dir.normalized;
            StartCoroutine(KnockUp());
            enemy.AddForce((_knockbackDirection * _knockbackForce), ForceMode2D.Force);
        }
        
        if (collision.gameObject.tag == "Player" && PowerCharacter7.CanSlow)
        {
            StartCoroutine(Slow());
            _aiMovementStats.MaxWalkSpeed = _aiMovementStats.MaxWalkSpeed - (_aiMovementStats.MaxWalkSpeed * _slowPercentage);
            _aiMovementStats.MaxRunSpeed = _aiMovementStats.MaxRunSpeed - (_aiMovementStats.MaxRunSpeed * _slowPercentage);
            _power7CharacterVFX.SetActive(true);
        }

        if (collision.gameObject.tag == "Player" && PowerCharacter8.IsPowering)
        {
            Rigidbody2D enemy = collision.gameObject.GetComponent<Rigidbody2D>();
            _knockbackDirection = (collision.transform.position - transform.position).normalized;
            _power8CharacterVFX.SetActive(true);
            StartCoroutine(KnockUp());
            enemy.AddForce((_knockbackDirection * _knockbackForce), ForceMode2D.Force);
        }

        if (collision.gameObject.tag == "Player" && PowerCharacter9.CanStun)
        {
            Rigidbody2D enemy = collision.gameObject.GetComponent<Rigidbody2D>();
            enemy.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            _power9CharacterVFX.SetActive(true);
            StartCoroutine(Freeze(enemy));
        }

        if (collision.gameObject.tag == "Player" && PowerCharacter10.CanDisable)
        {
            GameObject enemy = collision.gameObject;
            _power10CharacterVFX.SetActive(true);
            StartCoroutine(EnableEnemy(enemy));
        }
        #endregion
        
    }

    IEnumerator EnableEnemy(GameObject enemy)
    {
        ImpactFlash.Instance.Flash();
        yield return new WaitForSeconds(0.25f);
        enemy.SetActive(false);
        StopFrame.Instance.Stop(0.15f);
        CameraShake.Instance.GloabalCameraShake(_impulseSource);
        SoundEffectManager.Instance.PlaySoundEffect(_hurtClip, transform, 1);
        PowerCharacter5.CanDisable = false;
        PowerCharacter10.CanDisable = false;
        yield return new WaitForSeconds(_disableTimer);
        _power5CharacterVFX.SetActive(false);
        _power10CharacterVFX.SetActive(false);
        enemy.SetActive(true);
    }

    IEnumerator Freeze(Rigidbody2D enemy)
    {
        ImpactFlash.Instance.Flash();
        StopFrame.Instance.Stop(0.15f);
        CameraShake.Instance.GloabalCameraShake(_impulseSource);
        SoundEffectManager.Instance.PlaySoundEffect(_hurtClip, transform, 1);
        PowerCharacter4.CanStun = false;
        PowerCharacter9.CanStun = false;
        yield return new WaitForSeconds(_freezeDuration);
        _power4CharacterVFX.SetActive(false);
        _power9CharacterVFX.SetActive(false);
        enemy.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    IEnumerator Slow()
    {
        ImpactFlash.Instance.Flash();
        StopFrame.Instance.Stop(0.15f);
        CameraShake.Instance.GloabalCameraShake(_impulseSource);
        SoundEffectManager.Instance.PlaySoundEffect(_hurtClip, transform, 1);
        PowerCharacter2.CanSlow = false;
        PowerCharacter7.CanSlow = false;
        yield return new WaitForSeconds(_slowDuration);
        _power2CharacterVFX.SetActive(false);
        _power7CharacterVFX.SetActive(false);
        _playerMovementStats.MaxWalkSpeed = _originalWalkSpeed;
        _playerMovementStats.MaxRunSpeed = _originalRunSpeed;
        _aiMovementStats.MaxWalkSpeed = _originalWalkSpeed;
        _aiMovementStats.MaxRunSpeed = _originalRunSpeed;
    }

    IEnumerator KnockUp()
    {
        ImpactFlash.Instance.Flash();
        StopFrame.Instance.Stop(0.15f);
        CameraShake.Instance.GloabalCameraShake(_impulseSource);
        SoundEffectManager.Instance.PlaySoundEffect(_hurtClip, transform, 1);
        yield return new WaitForSeconds(.5f);
        _power3CharacterVFX.SetActive(false);
        _power8CharacterVFX.SetActive(false);
        PowerCharacter8.IsPowering = false;
        PowerCharacter3.IsPowering = false;
        PowerCharacter1.CanKnock = false;
        PowerCharacter6.CanKnock = false;
    }

    public void ApplyKnockback(float damageTaken, Vector2 attackerPosition, bool isFacingRight)
    {
        // Calculate knockback force
        float totalKnockback = _baseKnockback + (damageTaken * _knockbackScalingFactor * _knockbackGrowth);

        float facingMultiplier = isFacingRight ? 1f : -1f;
        Vector2 knockbackDirection = new Vector2(facingMultiplier * Mathf.Cos(_knockbackAngle * Mathf.Deg2Rad), Mathf.Sin(_knockbackAngle * Mathf.Deg2Rad)).normalized;

        // Determine knockback force
        Vector2 knockbackForce = knockbackDirection * totalKnockback;


        // Apply knockback force
        _rb.velocity = knockbackForce;

        // Apply hitstun
        float hitstunDuration = totalKnockback * _hitstunMultiplier;
        StartCoroutine(ApplyHitstun(hitstunDuration));
    }

    private IEnumerator ApplyHitstun(float duration)
    {
        _inHitstun = true;
        yield return new WaitForSeconds(duration);
        _inHitstun = false;
    }
}
