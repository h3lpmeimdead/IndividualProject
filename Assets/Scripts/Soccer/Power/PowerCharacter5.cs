using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character5", menuName = "Ability/Character5")]
public class PowerCharacter5 : AbilitySO
{
    //FIRE SHOT
    public static bool CanDisable;
    [SerializeField] private AudioClip _power5SFX;
    public override void ActivateAbillity(GameObject parent, Transform ballTransform, Rigidbody2D ballRB, float power, bool isFacingRight)
    {
        Vector2 direction = isFacingRight ? new Vector2(1, .5f) : new Vector2(-1, .5f);
        direction.Normalize();
        ballRB.velocity = Vector2.zero;
        ballRB.AddForce(direction * power, ForceMode2D.Impulse);
        PlayerPower.Instance.IsReady = false;
        SoundEffectManager.Instance.PlaySoundEffect(_power5SFX, parent.transform, 1);
        CanDisable = true;
    }
}
