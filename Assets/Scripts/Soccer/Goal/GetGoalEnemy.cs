using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGoalEnemy : Singleton<GetGoalEnemy>
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Ball")
        {
            //goal
            EnemyGoalCounter.Instance.Goal();
        }
    }
}
