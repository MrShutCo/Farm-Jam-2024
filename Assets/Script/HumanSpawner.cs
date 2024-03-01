using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Humans;
using Assets.Script.Humans.Traits;
using UnityEngine;
using Random = System.Random;

public class HumanSpawner : MonoBehaviour
{
    private List<Human> _wildHumans;
    private System.Random _random;

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

    private readonly List<string> _englishNames = new ()
    {
        "Alexander",
        "Emily",
        "Jacob",
        "Sophia",
        "William",
        "Olivia",
        "Michael",
        "Emma",
        "Daniel",
        "Isabella",
        "Matthew",
        "Ava",
        "Christopher",
        "Mia",
        "Andrew",
        "Abigail",
        "Ethan",
        "Madison",
        "Grace",
        "Henry"
    };
    
    // Start is called before the first frame update
    void Start()
    {
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
            h.InitializeHuman(_englishNames[_random.Next(0, _englishNames.Count)], traits);
        }
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
            < 85 => ERank.D,
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