using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Humans;
using Assets.Script.Humans.Traits;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class HumanSpawner : MonoBehaviour
{
    private List<Human> _wildHumans;
    private System.Random _random;

    [SerializeField] Transform spawner;
    List<SpawnArea> spawnAreas;

    [SerializeField] private Transform portalPosition;
    [SerializeField] private List<float> distanceThresholds;

    private readonly List<string> _lovecraftianNames = new List<string>
    {
        "Azathoth",
        "Nyarlathotep",
        "Yog-Sothoth",
        "Shub-Piggurath",
        "Dagon",
        "Hastur",
        "Ithaqua",
        "Yuggoth",
        "Yig",
        "Tsathoggua",
        "Nephren-Ka",
        "R'lyeh",
        "Alhazred",
        "Shoggoth",
        "Byakhee",
        "Yith",
        "Leng",
        "Gla'aki",
        "Iak-Sakkath"
    };

    List<string> _farmerNames = new List<string>
    {
        "Earl",
        "Jethro",
        "Cletus",
        "Bubba",
        "Luther",
        "Mabel",
        "Bertha",
        "Hank",
        "Buford",
        "Wilma",
        "Ezekiel",
        "Daisy",
        "Gus",
        "Clyde",
        "Enid",
        "Cyrus",
        "Opal",
        "Roscoe",
        "Myrtle",
        "Floyd"
    };

    List<string> industrialWorkerNames = new List<string>
    {
        "George",
        "Edna",
        "Frank",
        "Alice",
        "Arthur",
        "Mildred",
        "Walter",
        "Ethel",
        "Harold",
        "Dorothy",
        "Clarence",
        "Florence",
        "Albert",
        "Gertrude",
        "Henry",
        "Helen",
        "Raymond",
        "Gladys",
        "Chester",
        "Marie"
    };

    List<string> militaryNames = new List<string>
    {
        "Sgt. Johnson",
        "Cpl. Baker",
        "Prv. Williams",
        "Cpl. Martin",
        "Sgt. Jones",
        "Prv. Davis",
        "Sgt. Smith",
        "Cpl. Taylor",
        "Sgt. Brown",
        "Cpl. Robinson",
        "Prv. Miller",
        "Prv. Harris",
        "Prv. Wilson",
        "Prv. Clark",
        "Sgt. Turner",
        "Cpl. Anderson",
        "Prv. Carter",
        "Cpl. Thompson",
        "Prv. Nelson",
        "Cpl. Davis",
    };


    private void OnEnable()
    {
        GameManager.Instance.onGameStateChange += Despawn;

    }
    private void OnDisable()
    {
        GameManager.Instance.onGameStateChange -= Despawn;
    }
    void OnEnteringWild()
    {
        spawnAreas = FindObjectsByType<SpawnArea>(FindObjectsSortMode.None).ToList();
        foreach (var spawner in spawnAreas)
        {
            spawner.Spawn();
        }
        _wildHumans = GetComponentsInChildren<Human>().ToList();
        _random = new Random();
        foreach (var h in _wildHumans)
        {
            var distance = Vector3.Distance(h.transform.position, portalPosition.position);
            List<Func<Trait>> generator;
            switch (GetIndexOfThreshold(distance))
            {
                case 0:
                    generator = new() { GenerateLowLevelTrait, GenerateLowLevelTrait };
                    break;
                case 1:
                    generator = new() { GenerateLowLevelTrait, GenerateMidLevelTrait };
                    break;
                case 2:
                    generator = new() { GenerateMidLevelTrait, GenerateMidLevelTrait };
                    break;
                default:
                    generator = new() { GenerateLowLevelTrait };
                    break;
            }

            var traits = generator.Select(f => f.Invoke()).ToList();
            traits = traits.DistinctBy(t => t.Name).ToList(); // Remove duplicate traits
            h.InitializeHuman(randomName(), traits);
        }
    }

    string randomName()
    {
        if (GameManager.Instance.ChosenWorld == "Farm") return _farmerNames[_random.Next(0, _farmerNames.Count)];
        if (GameManager.Instance.ChosenWorld == "Industrial Block")
            return industrialWorkerNames[_random.Next(0, industrialWorkerNames.Count)];
        if (GameManager.Instance.ChosenWorld == "City") return militaryNames[_random.Next(0, militaryNames.Count)];
        return "Unknown Entity";
    }

    void Despawn(EGameState newState)
    {
        StartCoroutine(DespawnCoroutine(newState));
    }

    IEnumerator DespawnCoroutine(EGameState newState)
    {
        var prevGameState = GameManager.Instance.PreviousGameState;
        yield return new WaitForSeconds(.1f);
        if (prevGameState == EGameState.Wild && newState == EGameState.Normal ||
        prevGameState == EGameState.Death && newState == EGameState.Normal)
        {
            _wildHumans = GetComponentsInChildren<Human>().ToList();
            foreach (var h in _wildHumans)
            {
                Debug.Log("Destorying Human: " + h.name);
                Destroy(h.gameObject);
            }
            _wildHumans.Clear();
            DespawnOtherTransforms();
        }
        else if (prevGameState == EGameState.Normal && newState == EGameState.Wild)
        {
            OnEnteringWild();
        }

    }
    void DespawnOtherTransforms()
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child != transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
    void Start()
    {

    }

    int GetIndexOfThreshold(float distance)
    {
        for (int i = 0; i < distanceThresholds.Count; i++)
            if (distance < distanceThresholds[i]) return i;
        return distanceThresholds.Count - 1;
    }

    private Trait GenerateMidLevelTrait()
        => new ResourceTrait(GenerateRandomHumanResource(), GenerateMidLevelRank());

    private Trait GenerateLowLevelTrait()
    => new ResourceTrait(GenerateRandomHumanResource(), GenerateLowLevelRank());

    EResource GenerateRandomHumanResource() => (EResource)_random.Next(4, 7);

    ERank GenerateMidLevelRank()
    {
        var x = _random.Next(0, 100);
        return x switch
        {
            < 15 => ERank.F,
            < 30 => ERank.D,
            < 75 => ERank.C,
            _ => ERank.B
        };
    }

    ERank GenerateLowLevelRank()
    {
        var x = _random.Next(0, 100);
        return x switch
        {
            < 60 => ERank.F,
            < 90 => ERank.D,
            _ => ERank.C
        };
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var dist in distanceThresholds)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(portalPosition.position, dist);
        }
    }
}
