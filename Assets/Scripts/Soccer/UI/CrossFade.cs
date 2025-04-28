using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CrossFade : SceneTransition
{
    [SerializeField] private CanvasGroup _crossFadeCanvasGroup;
    public override IEnumerator TransitionIn()
    {
        if (_crossFadeCanvasGroup != null)
        {
            var tweener = _crossFadeCanvasGroup.DOFade(1, 1).SetUpdate(true);
            yield return tweener.WaitForCompletion();
        }
    }

    public override IEnumerator TransitionOut()
    {
        if (_crossFadeCanvasGroup != null)
        {
            var tweener = _crossFadeCanvasGroup.DOFade(0, 1).SetUpdate(true);
            yield return tweener.WaitForCompletion();
        }
    }
}
