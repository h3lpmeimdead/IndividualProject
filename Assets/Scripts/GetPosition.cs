using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GetPosition : MonoBehaviour
{
    [SerializeField] private GameObject _enemy;

    private void Update()
    {
        this.transform.position = _enemy.transform.position;
    }
}
