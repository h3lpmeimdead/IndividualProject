using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character2", menuName = "Ability/Character2")]
public class PowerCharacter2 : AbilitySO
{
    //ELECTRIC SHOT
    public static bool CanSlow = false;
    [SerializeField] private AudioClip _power2SFX;
    public override void ActivateAbillity(GameObject parent, Transform ballTransform, Rigidbody2D ballRB, float power, bool isFacingRight)
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        ballRB.velocity = Vector2.zero;
        ballRB.AddForce(direction * power, ForceMode2D.Impulse);
        PlayerPower.Instance.IsReady = false;
        SoundEffectManager.Instance.PlaySoundEffect(_power2SFX, parent.transform, 1);
        CanSlow = true;
    }
}
