using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKickable 
{
    public void ApplyKnockback(float damageTaken, Vector2 attackerPosition, bool isFacingRight);
}
