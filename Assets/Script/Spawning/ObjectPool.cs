using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private GameObject _parent;
    private int _capacity;
    PoolableObject _prefab;
    List<PoolableObject> availableObj;

    private ObjectPool(PoolableObject prefab, int capacity)
    {
        _prefab = prefab;
        _capacity = capacity;
        availableObj = new List<PoolableObject>(capacity);
    }

    public static ObjectPool CreateInstance(PoolableObject prefab, int capacity)
    {
        ObjectPool pool = new ObjectPool(prefab, capacity);

        GameObject poolObject = new GameObject(prefab.name + " Pool");
        Debug.Log(prefab.name + " Pool");
        pool._parent = poolObject;
        poolObject.transform.parent = GameObject.Find("ObjectPools").transform;
        pool.CreateObjects();
        return pool;
    }

    private void CreateObjects()
    {
        for (int i = 0; i < _capacity; i++)
        {
            CreateObject();
        }
    }

    private void CreateObject()
    {
        PoolableObject poolableObject = GameObject.Instantiate(_prefab, Vector3.zero, Quaternion.identity, _parent.transform);
        poolableObject.Parent = this;
        poolableObject.gameObject.SetActive(false);
    }

    public PoolableObject GetObject(Vector3 position, Quaternion rotation)
    {
        if (availableObj.Count < 1)
        {
            Debug.Log("Needed to create an additional object in the pool for : " + _prefab.name);
            CreateObject();
        }
        PoolableObject instance = availableObj[0];
        availableObj.RemoveAt(0);
        instance.gameObject.transform.position = position;
        instance.gameObject.transform.rotation = rotation;
        instance.gameObject.SetActive(true);

        return instance;
    }

    public void ReturnObjectToPool(PoolableObject poolableObject)
    {
        availableObj.Add(poolableObject);
    }
}
