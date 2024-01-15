using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Vector2 spawnMidPoint;
    [SerializeField] float spawnWidth, spawnHeight;
    [SerializeField] int amountToSpawn;

    SpawnManager spawnManager;

    private void Awake()
    {
        spawnManager = GetComponent<SpawnManager>();
    }

    private void OnEnable()
    {
        Invoke("Spawn", 1);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnMidPoint, new Vector2(spawnWidth, spawnHeight));
    }

    void Spawn()
    {
        Debug.Log("Spawning");
        StartCoroutine(spawnManager.SpawnWithinArea(new int[] { 0 }, 1, amountToSpawn, SpawnManager.SpawnMethod.Random, spawnMidPoint, spawnWidth, spawnHeight));
        Debug.Log("Spawned");
    }
}
