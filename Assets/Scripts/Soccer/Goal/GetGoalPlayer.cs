using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGoalPlayer : Singleton<GetGoalPlayer>
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Ball")
        {
            //goal
            PlayerGoalCounter.Instance.Goal();
        }
    }
}
