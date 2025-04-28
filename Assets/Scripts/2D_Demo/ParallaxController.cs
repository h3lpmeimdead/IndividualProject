using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    Transform _camera;
    Vector3 _cameraStartPos;
    float _distance;
    GameObject[] _backgrounds;
    Material[] _materials;
    float[] _backSpeed;

    float _furthestBack;

    [Range(0.01f, 0.05f)]
    public float ParallaxSpeed;

    private void Start()
    {
        _camera = Camera.main.transform;
        _cameraStartPos = _camera.position;

        int backCount = transform.childCount;
        _materials = new Material[backCount];
        _backSpeed = new float[backCount];
        _backgrounds = new GameObject[backCount];

        for(int i = 0; i < backCount; i++)
        {
            _backgrounds[i] = transform.GetChild(i).gameObject;
            _materials[i] = _backgrounds[i].GetComponent<Renderer>().material;
        }
        BackSpeedCalculation(backCount);
    }

    void BackSpeedCalculation(int backCount)
    {
        for (int i = 0; i < backCount; i++)
        {
            if ((_backgrounds[i].transform.position.z - _camera.position.z) > _furthestBack)
            {
                _furthestBack = _backgrounds[i].transform.position.z - _camera.position.z;
            }
        }

        for(int i = 0; i < backCount; i++)
        {
            _backSpeed[i] = 1 - (_backgrounds[i].transform.position.z - _camera.position.z) / _furthestBack;
        }
    }

    private void LateUpdate()
    {
        _distance = _camera.position.x - _cameraStartPos.x;
        transform.position = new Vector3(_camera.position.x, _camera.position.y, 0);

        for(int i = 0; i < _backgrounds.Length; i++)
        {
            float speed = _backSpeed[i] * ParallaxSpeed;
            _materials[i].SetTextureOffset("_MainTex", new Vector2(_distance, 0) * speed);
        }
    }
}
