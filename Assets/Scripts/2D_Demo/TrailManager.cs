using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailManager : MonoBehaviour
{
    TrailRenderer _trailRenderer;

    private void Start()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Awake()
    {
        if (_trailRenderer != null)
        {
            _trailRenderer.enabled = true;
        }
    }
}
