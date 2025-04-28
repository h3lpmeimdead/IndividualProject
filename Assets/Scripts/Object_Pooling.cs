using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Pooling : Singleton<Object_Pooling>
{
    private List<GameObject> _pooledObjects = new List<GameObject>();
    [SerializeField] public int _amountToPool = 10;
    [SerializeField] private GameObject _poolGameObject;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < _amountToPool; i++)
        {
            GameObject obj = Instantiate(_poolGameObject);
            obj.SetActive(false);
            _pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for(int i = 0; i < _pooledObjects.Count; i++)
        {
            if (!_pooledObjects[i].activeInHierarchy)
            {
                return _pooledObjects[i];
            }
        }

        return null;
    }
}
