using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SpawnArea : MonoBehaviour
{
    [SerializeField] private int MinSpawn;
    [SerializeField] private int MaxSpawn;
    [SerializeField] private bool SnapToGrid;

    [SerializeField] private Transform parentTransform;
    [SerializeField] List<GameObject> spawnables;
    [SerializeField] private List<int> weights;

    private BoxCollider2D _spawnArea;
    private WeightedRouletteWheel<GameObject> wheel;

    // Start is called before the first frame update
    void Start()
    {
        _spawnArea = GetComponent<BoxCollider2D>();

        wheel = new WeightedRouletteWheel<GameObject>(spawnables, weights);
        Spawn();
    }

    public void Spawn()
    {
        int numSpawn = Random.Range(MinSpawn, MaxSpawn);
        for (int i = 0; i < numSpawn; i++)
        {
            var obj = Instantiate(wheel.Spin(), GetRandomPosition(), Quaternion.identity);
            obj.transform.parent = parentTransform;
            obj.TryGetComponent<ISpawnable>(out var spawnable);
            spawnable?.InitializeSpawnable();
        }
    }

    Vector3 GetRandomPosition()
    {
        var b = _spawnArea.bounds;
        var (x, y) = (Random.Range(b.min.x, b.max.x), Random.Range(b.min.y, b.max.y));
        if (SnapToGrid)
        {
            x = Mathf.Floor(x);
            y = Mathf.Floor(y);
        }

        return new Vector3(x, y, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class WeightedRouletteWheel<T>
{
    private readonly List<T> elements;
    private readonly List<int> weights;

    public WeightedRouletteWheel(List<T> elements, List<int> weights)
    {
        if (elements.Count != weights.Count)
        {
            throw new ArgumentException("The number of elements must match the number of weights.");
        }

        this.elements = elements;
        this.weights = weights;
    }

    public T Spin()
    {
        int totalWeight = weights.Sum();
        int randomNumber = Random.Range(1, totalWeight + 1);

        int accumulatedWeight = 0;
        for (int i = 0; i < elements.Count; i++)
        {
            accumulatedWeight += weights[i];
            if (randomNumber <= accumulatedWeight)
            {
                return elements[i];
            }
        }

        // This should not happen under normal circumstances
        throw new InvalidOperationException("Failed to select an element from the roulette wheel.");
    }
}
