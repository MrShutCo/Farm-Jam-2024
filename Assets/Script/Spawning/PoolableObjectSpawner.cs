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
    [SerializeField] Vector2 Direction;

    private void Start()
    {
        Spawn(transform, this);
    }


    public void Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour)
    {
        this.ActiveMonoBehaviour = ActiveMonoBehaviour;
        pool = new ObjectPool<GameObject>(CreateObject);

        Random.InitState(System.DateTime.Now.Millisecond);
        int startTime = Random.Range(0, 4);

        InvokeRepeating("GetObject", startTime, 12);
    }
    GameObject CreateObject()
    {
        GameObject obj = Instantiate(objectPrefab);
        obj.SetActive(false);
        return obj;
    }

    public void GetObject()
    {
        GameObject instance = pool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = (Vector2)transform.position + Direction;
        instance.transform.rotation = Quaternion.identity;
        instance.transform.SetParent(this.transform);
        instance.GetComponent<TilePathFollowers>().SetFacingDirection(Direction);
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Release(obj);
    }


}
