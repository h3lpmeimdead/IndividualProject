using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MissGoal : MonoBehaviour
{
    [SerializeField] private AudioClip _missGoalClip;
    private CinemachineImpulseSource _impulseSource;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            SoundEffectManager.Instance.PlaySoundEffect(_missGoalClip, transform, 1);
            CameraShake.Instance.GloabalCameraShake(_impulseSource);
        }
    }
}
