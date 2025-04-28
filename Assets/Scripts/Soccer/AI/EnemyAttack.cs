using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : Singleton<EnemyAttack>
{
    [SerializeField] private Transform _attackTransform;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private LayerMask _attackableLayer;
    [SerializeField] private LayerMask _kickableLayer;
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private float _damageAmount = 1;
    [SerializeField] private float _kickForce = 15;
    public float DamageAmount { get => _damageAmount; set => _damageAmount = value; }

    [SerializeField] private bool _isAttacking;
    public bool IsAttacking { get => _isAttacking; set => _isAttacking = value; }

    private RaycastHit2D[] _hits;

    private Animator _animator;
    public Animator Animator  { get => _animator; set => _animator = value; }

    private List<IDamageable> _iDamageable = new List<IDamageable>();

    private float _attackTimeCounter;
    public bool ShouldDamage { get; private set; } = false;

    private CinemachineImpulseSource _impulseSource;
    
    [SerializeField] private AudioClip _hurtClip;
    [SerializeField] private AudioClip _kickClip;
    [SerializeField] private AudioClip _kickBallClip;

    private void Start()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _animator = GetComponent<Animator>();
        _attackTimeCounter = _attackCooldown;
    }

    private void Update()
    {
        _animator.speed = 1;
        if (EnemyInputManager.AttackWasPressed && _attackTimeCounter >= _attackCooldown)
        {
            _attackTimeCounter = 0;
            IsAttacking = true;
        }

        _attackTimeCounter += Time.deltaTime;
    }

    #region Attack

    public IEnumerator KickTheBall()
    {
        _hits = Physics2D.CircleCastAll(_attackTransform.position, _attackRange, transform.right, _kickableLayer);

        SoundEffectManager.Instance.PlaySoundEffect(_kickClip, transform, 1);

        for (int i = 0; i < _hits.Length; i++)
        {
            IKickable kickable = _hits[i].collider.gameObject.GetComponent<IKickable>();
            BallPhysics knockable = _hits[i].collider.gameObject.GetComponent<BallPhysics>();
            ImpactFlash flash = _hits[i].collider.gameObject.GetComponent<ImpactFlash>();

            if (kickable != null)
            {
                knockable.ApplyKnockback(_kickForce, this.transform.position, AINewMovement.Instance.IsFacingRight);
                flash.Flash();
                SoundEffectManager.Instance.PlaySoundEffect(_kickBallClip, transform, 1);
                StopFrame.Instance.Stop(0.15f);
                CameraShake.Instance.GloabalCameraShake(_impulseSource);
            }
        }

        yield return null;

    }

    public IEnumerator DamageWhileAttackIsActive()
    {
        ShouldDamage = true;

        while (ShouldDamage)
        {
            _hits = Physics2D.CircleCastAll(_attackTransform.position, _attackRange, transform.right, 0f, _attackableLayer);

            for (int i = 0; i < _hits.Length; i++)
            {
                IDamageable damageable = _hits[i].collider.gameObject.GetComponent<IDamageable>();
                KnockbackPhysicPlayer knockbackable = _hits[i].collider.gameObject.GetComponent<KnockbackPhysicPlayer>();
                ImpactFlash flash = _hits[i].collider.gameObject.GetComponent<ImpactFlash>();

                if (damageable != null && !damageable.HasTakenDamage)
                {
                    damageable.Damage(_damageAmount);
                    _iDamageable.Add(damageable);
                    knockbackable.ApplyKnockback(_damageAmount, this.transform.position, AINewMovement.Instance.IsFacingRight);
                    flash.Flash();
                    StopFrame.Instance.Stop(0.15f);
                    CameraShake.Instance.GloabalCameraShake(_impulseSource);
                    SoundEffectManager.Instance.PlaySoundEffect(_hurtClip, transform, 1);
                }
            }

            yield return null;
        }

        ReturnAttackToDamageable();
    }
    
    private void ReturnAttackToDamageable()
    {
        foreach (IDamageable damaged in _iDamageable)
        {
            damaged.HasTakenDamage = false;
        }

        _iDamageable.Clear();
    }
    #endregion

    #region Debug
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_attackTransform.position, _attackRange);
    }
    #endregion

    #region SetDamage
    public void SetDamageToFalse()
    {
        ShouldDamage = false;
        IsAttacking = false;
    }

    public void SetDamageToTrue()
    {
        ShouldDamage = true;
    }
    #endregion
}