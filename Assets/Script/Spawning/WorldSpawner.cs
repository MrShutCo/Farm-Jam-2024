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
            GameObject world = GameManager.Instance.ChosenWorld switch
            {
                "Farm" => farmPrefabs[Random.Range(0, farmPrefabs.Length)],
                "Industrial Block" => industrialPrefabs[Random.Range(0, industrialPrefabs.Length)],
                "City" => cityPrefabs[Random.Range(0, cityPrefabs.Length)],
                _ => farmPrefabs[Random.Range(0, farmPrefabs.Length)]
            };

            GameObject newWorld = Instantiate(world, SpawnPos, Quaternion.identity);
            GameObject wildHumanoids = GameManager.Instance.WildHumanoidParent;
            wildHumanoids.GetComponent<World>().Grid = newWorld.GetComponent<Grid2D>();
            activeWorld = newWorld;
        }
        else if (activeWorld != null && gameState == EGameState.Normal || gameState == EGameState.Death)
        {
            Destroy(activeWorld);
        }
    }
}

