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
            List<Trait> traits = new List<Trait>();
            /*switch (GameManager.Instance.ProgressManager.ProgressIndex)
            {
                case 0:
                    traits = generateUpToN(1, Gen1);
                    break;
                case 1:
                    traits = generateUpToN(2, Gen2);
                    break;
                case 2:
                    traits = generateUpToN(1, Gen3);
                    break;
                case 3:
                    traits = generateUpToN(2, Gen4);
                    break;
                case 4:
                    traits = generateUpToN(1, Gen5);
                    break;
                case 5:
                    traits = generateUpToN(2, Gen6);
                    break;
                default:
                    
                    break;
            }*/
            switch (GameManager.Instance.ChosenWorld)
            {
                case "Farm":
                    traits = GameManager.Instance.ProgressManager.ProgressIndex > 0
                        ? generateUpToN(2, Gen1)
                        : generateUpToN(1, Gen1);
                    break;
                case "Industrial Block":
                    traits = GameManager.Instance.ProgressManager.ProgressIndex > 2
                        ? generateUpToN(2, Gen3)
                        : generateUpToN(1, Gen3);
                    break;
                case "City":
                    traits = GameManager.Instance.ProgressManager.ProgressIndex > 4
                        ? generateUpToN(2, Gen5)
                        : generateUpToN(1, Gen5);
                    break;
            }
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

    List<Trait> generateUpToN(int n, Func<ERank> gen)
    {
        var traits = new List<Trait>();
        var count = _random.Next(1, n+1);
        for (int i = 0; i < count; i++)
        {
            traits.Add(new ResourceTrait(GenerateRandomHumanResource(), gen()));
        }
        return traits;
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

    EResource GenerateRandomHumanResource() => (EResource)_random.Next(4, 7);

    ERank Gen1()
    {
        var x = _random.Next(0, 100);
        return x switch
        {
            < 75 => ERank.F,
            _ => ERank.D
        };
    }
    
    ERank Gen3()
    {
        var x = _random.Next(0, 100);
        return x switch
        {
            < 40 => ERank.D,
            < 80 => ERank.C,
            _ => ERank.B
        };
    }
    
    ERank Gen5()
    {
        var x = _random.Next(0, 100);
        return x switch
        {
            < 50 => ERank.B,
            < 85 => ERank.A,
            _ => ERank.S
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
