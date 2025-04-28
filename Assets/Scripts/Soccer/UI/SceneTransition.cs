using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneTransition : MonoBehaviour
{
    public abstract IEnumerator TransitionIn();
    public abstract IEnumerator TransitionOut();
}
