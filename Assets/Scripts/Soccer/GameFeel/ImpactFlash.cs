using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactFlash : Singleton<ImpactFlash>
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _duration = 0.25f;
    [SerializeField] private Color _flashColor;
    public void Flash()
    {
        StartCoroutine(DoFlash(_spriteRenderer, _duration, _flashColor));
    }

    private IEnumerator DoFlash(SpriteRenderer spriteRenderer, float duration, Color flashColor)
    {
        Color orginalColor = spriteRenderer.color;
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(duration);

        spriteRenderer.color = orginalColor;
    }
}
