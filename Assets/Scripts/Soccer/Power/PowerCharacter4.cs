using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character4", menuName = "Ability/Character4")]
public class PowerCharacter4 : AbilitySO
{
    public static bool CanStun;
    [SerializeField] private AudioClip _power4SFX;

    //ICE SHOT
    public override void ActivateAbillity(GameObject parent, Transform ballTransform, Rigidbody2D ballRB, float power, bool isFacingRight)
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        ballRB.velocity = Vector2.zero;
        ballRB.AddForce(direction * power, ForceMode2D.Impulse);
        PlayerPower.Instance.IsReady = false;
        SoundEffectManager.Instance.PlaySoundEffect(_power4SFX, parent.transform, 1);
        CanStun = true;
    }
}
