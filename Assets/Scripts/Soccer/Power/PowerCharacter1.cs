using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Character1", menuName = "Ability/Character1")]
public class PowerCharacter1 : AbilitySO
{
    public static bool CanKnock;
    [SerializeField] private AudioClip _power1SFX;
    public override void ActivateAbillity(GameObject parent, Transform ballTransform, Rigidbody2D ballRB, float power, bool isFacingRight)
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        ballRB.velocity = Vector2.zero;
        ballRB.AddForce(direction * power, ForceMode2D.Impulse);
        PlayerPower.Instance.IsReady = false;
        SoundEffectManager.Instance.PlaySoundEffect(_power1SFX, parent.transform, 1);
        CanKnock = true;
    }
}
