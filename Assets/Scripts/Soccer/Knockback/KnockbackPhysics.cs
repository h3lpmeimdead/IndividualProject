using UnityEngine;
using System.Collections;

public class KnockbackPhysic : MonoBehaviour
{
    [SerializeField] private float _baseKnockback = 5f; 
    [SerializeField] private float _knockbackGrowth = 0.1f; 
    [SerializeField] private float _knockbackAngle = 45f; 
    [SerializeField] private float _knockbackScalingFactor = 0.02f; 
    [SerializeField] private float _gravity = 9.8f; 
    [SerializeField] private float _airDrag = 0.98f; 
    [SerializeField] private float _hitstunMultiplier = 0.05f; 
    [SerializeField] private float _wallBounceReduction = 0.7f; 
    [SerializeField] private float _ceilingBounceReduction = 0.5f; 

    private Rigidbody2D _rb;
    private bool _inHitstun = false;
    public bool InHitStun { get => _inHitstun; set => _inHitstun = value; }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(float damageTaken, Vector2 attackerPosition, bool isFacingRight)
    {
        
        float totalKnockback = _baseKnockback + (damageTaken * _knockbackScalingFactor * _knockbackGrowth);
        float wholeknockback = totalKnockback * EnemyHealth.Instance.KnockbackMultiplier;
       
        float facingMultiplier = isFacingRight ? 1f : -1f;
        Vector2 knockbackDirection = new Vector2(facingMultiplier * Mathf.Cos(_knockbackAngle * Mathf.Deg2Rad), Mathf.Sin(_knockbackAngle * Mathf.Deg2Rad)).normalized;

        
        Vector2 knockbackForce = knockbackDirection * wholeknockback;

        _rb.velocity = knockbackForce;
        
        float hitstunDuration = Mathf.Clamp(totalKnockback * _hitstunMultiplier, 0.1f, 1f);
        StartCoroutine(ApplyHitstun(hitstunDuration));
    }

    public IEnumerator ApplyHitstun(float duration)
    {
        _inHitstun = true;
        yield return new WaitForSeconds(duration);
        _inHitstun = false;
    }

    void FixedUpdate()
    {
        if (_inHitstun)
        {
            // Apply gravity over time
            _rb.velocity += Vector2.down * _gravity * Time.fixedDeltaTime;

            // Apply air drag
            _rb.velocity *= _airDrag;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle wall bounces
        if (collision.gameObject.tag == "Wall")
        {
            _rb.velocity = new Vector2(_rb.velocity.x * _wallBounceReduction, _rb.velocity.y);
        }
        // Handle ceiling bounces
        else if (collision.gameObject.tag == "Ceiling")
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * _ceilingBounceReduction);
        }
    }
}