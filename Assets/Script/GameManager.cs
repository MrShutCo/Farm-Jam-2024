using Assets.Buildings;
using Assets.Script.Buildings;
using Assets.Script.Humans;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public int CurrTime;

    public Dictionary<EResource, int> Resources { get; private set; }
    public List<Placeable> EnabledPlaceables;

    public event UnityAction onTimeStep;
    public Action<Building> onFarmCreate;
    public Action onResourceChange;

    public List<Building> Buildings;

    public Carrier Carrier;


    private void Awake()
    {
        Buildings = FindObjectsByType<Building>(FindObjectsSortMode.None).ToList();
        Resources = InitializeResources();

        int count = FindObjectsOfType<GameManager>().Length;
        if (count > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        Instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        AddResource(EResource.Food, 50);
    }

    public void AddResource(EResource resource, int amount)
    {
        Resources[resource] += amount;
        onResourceChange?.Invoke();
    }

    public Dictionary<EResource, int> InitializeResources()
        => new Dictionary<EResource, int>()
        {
            { EResource.Food, 0 }, {EResource.Wood, 0 }, { EResource.Iron, 0 },
            { EResource.Electronics, 0 }, { EResource.Plutonium, 0}
        };
}