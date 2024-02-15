using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] Bullet Prefab;
    [SerializeField] BoxCollider2D SpawnArea;
    [SerializeField] int BulletsPerSecond = 10;
    [SerializeField] float Speed = 5f;
    [SerializeField] bool UseObjectPool = false;

    ObjectPool<Bullet> BulletPool;

    private float LastSpawnTime;

    private void Awake()
    {
        BulletPool = new ObjectPool<Bullet>(CreatePooledObject, OnTakeFromPool, OnReturnToPool, OnDestroyObject, false, 200, 100_000);
    }

    private Bullet CreatePooledObject()
    {
        Bullet instance = Instantiate(Prefab, Vector3.zero, Quaternion.identity);
        instance.Disable += ReturnObjectToPool;
        instance.gameObject.SetActive(false);

        return instance;
    }

    private void ReturnObjectToPool(Bullet instance)
    {
        BulletPool.Release(instance);
    }

    private void OnTakeFromPool(Bullet instance)
    {
        instance.gameObject.SetActive(true);
        SpawnBullet(instance);
        instance.transform.SetParent(transform, true);
    }

    private void OnReturnToPool(Bullet instance)
    {
        instance.gameObject.SetActive(false);
    }

    private void OnDestroyObject(Bullet instance)
    {
        Destroy(instance.gameObject);
    }

    private void OnGUI()
    {
        if (UseObjectPool)
        {
            GUI.Label(new Rect(10, 10, 200, 30), $"Total Pool Size: {BulletPool.CountAll}");
            GUI.Label(new Rect(10, 10, 200, 30), $"Active Objects: {BulletPool.CountActive}");
        }
    }

    private void Update()
    {
        float delay = 1f / BulletsPerSecond;
        if (LastSpawnTime + delay < Time.time)
        {
            int bulletsToSpawnInFrame = Mathf.CeilToInt(Time.deltaTime / delay);
            while (bulletsToSpawnInFrame > 0)
            {
                if (!UseObjectPool)
                {
                    Bullet instance = Instantiate(Prefab, Vector3.zero, Quaternion.identity);
                    instance.transform.SetParent(transform, true);

                    SpawnBullet(instance);
                }

                else
                {
                    BulletPool.Get();
                }
                bulletsToSpawnInFrame--;
            }

            LastSpawnTime = Time.time;
        }
    }

    void SpawnBullet(Bullet instance)
    {
        Vector2 spawnPosition = new Vector2(
            SpawnArea.transform.position.x + SpawnArea.bounds.center.x + Random.Range(-1 * SpawnArea.bounds.extents.x, SpawnArea.bounds.extents.x),
            SpawnArea.transform.position.y + SpawnArea.bounds.center.y + Random.Range(-1 * SpawnArea.bounds.extents.y, SpawnArea.bounds.extents.y)
        );
        instance.transform.position = spawnPosition;
        instance.Shoot(spawnPosition, SpawnArea.transform.right, Speed);
    }
}
