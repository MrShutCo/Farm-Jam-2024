using Assets.Buildings;
using Assets.Script.Buildings;
using Assets.Script.Humans;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;

public enum EGameState
{
    Normal, Build, Death
}

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public EGameState GameState { get; private set; }

    public Dictionary<EResource, int> Resources { get; private set; }
    public List<Placeable> EnabledPlaceables;
    public Grid2D PathfindingGrid;
    public Grid2D PathfindingGridOutside;

    public Action onResourceChange;
    public Action<List<Human>> onCarriedHumansChange;// humans in bag/tendrils
    public Action<Dictionary<EResource, int>> onCarriedResourcesChange; // resources in bag/tendrils
    public Action<int, int> onHealthChange;
    public Action onEnterHomeBase;
    public Action onExitHomeBase;
    public Action<bool> onPause;
    public Action<bool, Vector2> onTeleport;
    public Action onDeath;
    public Action<Collider2D> onGridChange;
    public Action<Human> onHumanDie;
    public Action<EGameState> onGameStateChange;

    public Action<Building, Package> onPackageCreate;

    public List<Building> Buildings;
    public Transform Player;
    public Human CurrentlySelectedHuman;

    public GameObject WildHumanoidParent;
    public GameObject HomeHumanoidParent;
    public Carrier Carrier;

    bool paused;


    private void Awake()
    {
        GameState = EGameState.Normal;
        Player = FindObjectOfType<Player>().transform;
        Buildings = FindObjectsByType<Building>(FindObjectsSortMode.None).ToList();
        Resources = InitializeResources();
        PathfindingGrid = FindObjectOfType<Grid2D>();
        Carrier = Player.GetComponent<Carrier>();

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
        UpdatePathFindingGrids(true, Player.position);
    }
    private void OnEnable()
    {
        onTeleport += UpdatePathFindingGrids;
    }
    private void OnDisable()
    {
        onTeleport -= UpdatePathFindingGrids;

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void SetGameState(EGameState gameState)
    {
        GameState = gameState;
        onGameStateChange?.Invoke(gameState);
    }

    public void AddResource(EResource resource, int amount)
    {
        Resources[resource] += amount;
        onResourceChange?.Invoke();
    }

    public Dictionary<EResource, int> InitializeResources()
        => new Dictionary<EResource, int>()
        {
            { EResource.Food, 0 },
            { EResource.Wood, 0 }, { EResource.Steel, 0 },
            { EResource.Electronics, 0 }, { EResource.Blood, 0 },
            { EResource.Bones, 0 }, { EResource.Organs, 0 },
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

    void UpdatePathFindingGrids(bool teleport, Vector2 position = default)
    {
        if (teleport)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 10);
            if (colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    if (collider.TryGetComponent(out Grid2D grid))
                    {
                        if (PathfindingGrid == grid) continue;
                        PathfindingGridOutside = grid;
                    }
                }
            }
        }
    }
}