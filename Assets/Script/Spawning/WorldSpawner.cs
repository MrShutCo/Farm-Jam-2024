using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] farmPrefabs;
    [SerializeField] GameObject[] industrialPrefabs;
    [SerializeField] GameObject[] cityPrefabs;
    [SerializeField] Vector2 SpawnPos;
    GameObject activeWorld;
    private void OnEnable()
    {
        GameManager.Instance.onGameStateChange += SpawnWorld;
    }
    private void OnDisable()
    {
        GameManager.Instance.onGameStateChange -= SpawnWorld;
    }
    void SpawnWorld(EGameState gameState)
    {

        if (gameState == EGameState.Wild)
        {
            GameObject world;
            int progressIndex = GameManager.Instance.ProgressManager.ProgressIndex;
            if (progressIndex < 2)
                world = farmPrefabs[Random.Range(0, farmPrefabs.Length)];
            else if (progressIndex < 4)
                world = industrialPrefabs[Random.Range(0, industrialPrefabs.Length)];
            else
                world = cityPrefabs[Random.Range(0, cityPrefabs.Length)];
            GameObject newWorld = Instantiate(world, SpawnPos, Quaternion.identity);
            GameObject wildHumanoids = GameManager.Instance.WildHumanoidParent;
            wildHumanoids.GetComponent<World>().Grid = newWorld.GetComponent<Grid2D>();
            activeWorld = newWorld;
        }
        else if (activeWorld != null && gameState == EGameState.Normal)
        {
            Destroy(activeWorld);
        }
    }
}

