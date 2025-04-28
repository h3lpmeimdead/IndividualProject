using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopFrame : Singleton<StopFrame>
{
    private bool _isWaiting;
    public void Stop(float duration)
    {
        if (_isWaiting)
            return;
        Time.timeScale = 0;
        StartCoroutine(Wait(duration));
    }

    private IEnumerator Wait(float duration)
    {
        _isWaiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
        _isWaiting = false;
    }
}
