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
    public Grid2D PathfindingGrid;

    public event UnityAction onTimeStep;
    public Action<Building> onFarmCreate;
    public Action onResourceChange;
    public Action<List<Human>> onCarriedHumansChange;// humans in bag/tendrils
    public Action<Dictionary<EResource, int>> onCarriedResourcesChange; // resources in bag/tendrils
    public Action onEnterHomeBase;
    public Action onExitHomeBase;
    public Action<bool> onPause;
    public Action<bool> onTeleport;
    public Action<Collider2D> onGridChange;

    public List<Building> Buildings;
    public Transform Player;
    public Human CurrentlySelectedHuman;

    public Carrier Carrier;

    bool paused;


    private void Awake()
    {
        Player = FindObjectOfType<Player>().transform;
        Buildings = FindObjectsByType<Building>(FindObjectsSortMode.None).ToList();
        Resources = InitializeResources();
        PathfindingGrid = FindObjectOfType<Grid2D>();

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

    void Start()
    {
        SetTargetFrameRate();
        AddResource(EResource.Food, 50);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
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

    public void TogglePause()
    {
        bool pause = !paused;
        onPause?.Invoke(pause);
    }
    void SetTargetFrameRate()
    {
        Application.targetFrameRate = 120;
    }
}