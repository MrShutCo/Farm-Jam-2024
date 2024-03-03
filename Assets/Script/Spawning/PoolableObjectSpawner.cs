using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolableObjectSpawner : MonoBehaviour
{
    public GameObject objectPrefab;
    public Vector2 SpawnPoint;
    public Vector2 SpawnRotation;
    ObjectPool<GameObject> pool;
    private MonoBehaviour ActiveMonoBehaviour;


    public void Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour)
    {
        this.ActiveMonoBehaviour = ActiveMonoBehaviour;
        pool = new ObjectPool<GameObject>(CreateObject);

        InvokeRepeating("SpawnObject", 0, 1);
    }
    GameObject CreateObject()
    {
        GameObject obj = Instantiate(objectPrefab);
        obj.SetActive(false);
        return obj;
    }

    public void GetObject(Vector2 startPos, Quaternion rotation)
    {
        GameObject instance = pool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = startPos;
        instance.transform.rotation = rotation;
        instance.transform.SetParent(this.transform);
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Release(obj);
    }


}
