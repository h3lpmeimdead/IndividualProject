using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SecondCharacter3", menuName = "Ability/SecondCharacter3")]
public class PowerCharacter8 : AbilitySO
{
    //WIND SHOT
    public static bool IsPowering;
    [SerializeField] private AudioClip _power3SFX;
    
    public override void ActivateAbillity(GameObject parent, Transform ballTransform, Rigidbody2D ballRB, float power, bool isFacingRight)
    {
        IsPowering = true;
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        ballRB.velocity = Vector2.zero;
        Vector3 ballPosition = ballRB.transform.position;
        ballPosition.y = -19.3f;
        ballRB.transform.position = ballPosition;
        ballRB.AddForce(direction * power, ForceMode2D.Impulse);
        SoundEffectManager.Instance.PlaySoundEffect(_power3SFX, parent.transform, 1);
        EnemyPower.Instance.IsReady = false;
    }
}
