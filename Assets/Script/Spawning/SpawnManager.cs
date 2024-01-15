using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] protected List<PoolableObject> poolableObjectPrefabs = new();
    [SerializeField] protected int poolCapacity;
    [SerializeField] protected LayerMask layersToAvoid;
    [SerializeField] protected GameObject spawnVFX;


    protected Dictionary<int, ObjectPool> objectPools = new();
    public enum SpawnMethod { Repeat, RoundRobin, Random };

    public int GetSpawnIndex(PoolableObject poolableObject)
    {
        int index = poolableObjectPrefabs.IndexOf(poolableObject);
        if (index == -1)
        {
            Debug.LogError("Poolable object: " + poolableObject.name + " not found in poolable object prefabs list.");
            return -1;
        }
        Debug.Log("GetSpawnIndex is returning: " + index + " for " + poolableObject.name);
        return poolableObjectPrefabs.IndexOf(poolableObject);
    }

    public List<PoolableObject> GetPoolableObjects()
    {
        return poolableObjectPrefabs;
    }
    public ObjectPool GetObjectPool(PoolableObject poolableObject)
    {
        return objectPools[poolableObjectPrefabs.IndexOf(poolableObject)];
    }
    public void AddToPool(PoolableObject poolableObject)
    {
        if (poolableObjectPrefabs.Contains(poolableObject)) return;

        poolableObjectPrefabs.Add(poolableObject);
        int index = poolableObjectPrefabs.IndexOf(poolableObject);
        objectPools.Add(index, ObjectPool.CreateInstance(poolableObjectPrefabs[index], poolCapacity));
    }
    public virtual void Awake()
    {
        for (int i = 0; i < poolableObjectPrefabs.Count; i++)
        {
            objectPools.Add(i, ObjectPool.CreateInstance(poolableObjectPrefabs[i], poolCapacity));
        }
    }

    public IEnumerator SpawnAtSinglePoint(int[] spawnIndeces, float spawnInterval, int maxObjects, SpawnMethod spawnMethod, Vector3 spawnPoint, int maxScale = 1)
    {

        Debug.Log("Waiting");
        WaitForSeconds wait = new WaitForSeconds(spawnInterval);

        int objectsSpawned = 0;
        int currentSpawnIndex = 0;

        while (objectsSpawned < maxObjects)
        {
            Debug.Log("Getting Spawn Index");
            currentSpawnIndex = GetNextSpawnIndex(objectsSpawned, currentSpawnIndex, spawnMethod, spawnIndeces);

            Debug.Log("Spawning");
            if (spawnVFX)
                Instantiate(spawnVFX, spawnPoint, Quaternion.identity);
            else
                SpawnObject(currentSpawnIndex, spawnPoint, Quaternion.identity, maxScale);
            objectsSpawned++;

            yield return wait;
        }
    }
    public IEnumerator SpawnWithinArea(int[] spawnIndeces, float spawnInterval, int maxObjects, SpawnMethod spawnMethod, Vector3 spawnMidPoint, float spawnWidth, float spawnHeight, int maxScale = 1)
    {
        WaitForSeconds wait = new WaitForSeconds(spawnInterval);

        int objectsSpawned = 0;
        int currentSpawnIndex = 0;

        while (objectsSpawned < maxObjects)
        {
            currentSpawnIndex = GetNextSpawnIndex(objectsSpawned, currentSpawnIndex, spawnMethod, spawnIndeces);

            Vector3 spawnPoint = new Vector3(Random.Range(spawnMidPoint.x - spawnWidth / 2, spawnMidPoint.x + spawnWidth / 2), Random.Range(spawnMidPoint.y - spawnHeight / 2, spawnMidPoint.y + spawnHeight / 2), 1);

            if (!BadSpawnPoint(spawnPoint))
            {
                if (spawnVFX)
                    Instantiate(spawnVFX, spawnPoint, Quaternion.identity);
                else
                    SpawnObject(currentSpawnIndex, spawnPoint, Quaternion.identity, maxScale);
                objectsSpawned++;
            }

            yield return wait;
        }
    }

    public PoolableObject SpawnObject(int spawnIndex, Vector3 spawnPos, Quaternion spawnRot, int maxScale = 1)
    {
        PoolableObject poolableObject = objectPools[spawnIndex].GetObject(spawnPos, spawnRot);
        if (poolableObject == null)
        {
            Debug.LogError($"No available objects in the pool of type {poolableObjectPrefabs[spawnIndex].name}. May need to increase the pool size.");
        }
        return poolableObject;
    }
    protected int GetNextSpawnIndex(int objectsSpawned, int currentSpawnIndex, SpawnMethod spawnMethod, int[] spawnIndeces)
    {

        switch (spawnMethod)
        {
            case SpawnMethod.Repeat:
                currentSpawnIndex = objectsSpawned;
                break;
            case SpawnMethod.RoundRobin:
                currentSpawnIndex = objectsSpawned % spawnIndeces.Length;
                break;
            case SpawnMethod.Random:
                currentSpawnIndex = Random.Range(0, spawnIndeces.Length);
                break;
        }

        return currentSpawnIndex;
    }

    protected bool BadSpawnPoint(Vector3 spawnPoint)
    {
        /// <summary>
        /// Returns true if the spawn point does not collide with an layer to avoid
        /// </summary>

        return Physics.CheckSphere(spawnPoint, 1f, layersToAvoid);
    }
    public PoolableObject SpawnSimple(int index, Vector3 position, Quaternion rotation)
    {
        PoolableObject poolableObject = null;
        if (index >= 0 && index < objectPools.Count)
            if (objectPools.ContainsKey(index))
            {
                Debug.Log("Index is : " + index);
                poolableObject = objectPools[index].GetObject(position, rotation);
            }
        if (poolableObject == null)
        {
            Debug.LogError($"No available objects in the pool of index # " + index + " . May need to increase the pool size.");
        }
        return poolableObject;
    }
}
