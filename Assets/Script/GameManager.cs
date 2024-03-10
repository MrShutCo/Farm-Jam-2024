using Assets.Buildings;
using Assets.Script.Buildings;
using Assets.Script.Humans;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.Stats_and_Upgrades;
using UnityEngine;


public enum EGameState
{
    Normal, Wild, Build, Death, Dialogue
}

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public EGameState GameState { get; private set; }
    public EGameState PreviousGameState { get; private set; }
    public Dictionary<EResource, int> Resources { get; private set; }
    public Grid2D PathfindingGrid;
    public AudioManager AudioManager;

    public Action onResourceChange;
    public Action<List<Human>> onCarriedHumansChange;
    public Action<Dictionary<EResource, int>> onCarriedResourcesChange;
    public Action onHumanCarrierFull;
    public Action<float, float> onHealthChange;
    public Action<int> onAddTime;
    public Action onEnterHomeBase;
    public Action onExitHomeBase;
    public Action<bool> onPause;
    public Action<bool, Vector2> onTeleport;
    public Action onDeath;
    public Action<Collider2D> onGridChange;
    public Action<Human> onHumanDie;
    public Action<EGameState> onGameStateChange;
    public Action<Building, Package> onPackageCreate;
    public Action onGoalReached;

    #region Sound Events
    public Action<SoundRequest> onPlaySound;
    public Action<SoundRequest> onStopSound;
    #endregion

    public List<Building> Buildings;
    public Transform Player;
    public Human CurrentlySelectedHuman;

    public GameObject WildHumanoidParent;
    public GameObject HomeHumanoidParent;
    public Carrier Carrier;
    public ProgressManager ProgressManager;

    public Dictionary<EResource, int> BaseBuildLevel;

    public IconSO Icon;

    public int Stage;
    public string ChosenWorld;

    bool paused;


    private void Awake()
    {
        GameState = EGameState.Normal;
        Player = FindObjectOfType<Player>().transform;
        Buildings = FindObjectsByType<Building>(FindObjectsSortMode.None).ToList();
        Resources = InitializeResources();
        PathfindingGrid = FindObjectOfType<Grid2D>();
        Carrier = Player.GetComponent<Carrier>();
        BaseBuildLevel = new Dictionary<EResource, int>()
        {
            { EResource.Blood, 0 }, { EResource.Bones, 0 }, { EResource.Organs, 0 }
        };

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
        AddResource(EResource.Food, 0);
        AddResource(EResource.Wood, 100);
        AddResource(EResource.Steel, 0);
        AddResource(EResource.Electronics, 0);
        AddResource(EResource.Blood, 0);
        AddResource(EResource.Bones, 0);
        AddResource(EResource.Organs, 0);
        UpdatePathFindingGrids(true, Player.position);
        SetGameState(EGameState.Normal);
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

        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            AddResource(EResource.Food, 1000);
            AddResource(EResource.Wood, 1000);
            AddResource(EResource.Steel, 1000);
            AddResource(EResource.Electronics, 1000);
            AddResource(EResource.Blood, 1000);
            AddResource(EResource.Bones, 1000);
            AddResource(EResource.Organs, 1000);
        }
    }

    public void UnassignHumanFromBuilding(Human human)
    {
        foreach (var building in Instance.Buildings)
        {
            if (building.TryUnassignHuman(human)) return;
        }
    }

    public void SetGameState(EGameState gameState)
    {
        PreviousGameState = GameState;
        onGameStateChange?.Invoke(gameState);
        GameState = gameState;
        Debug.Log("Game State Changed to: " + gameState);
    }

    public void AddResource(EResource resource, int amount)
    {
        Resources[resource] += amount;
        onResourceChange?.Invoke();
    }

    public bool CanAfford(UpgradeCost cost)
    {
        foreach (var c in cost.cost)
        {
            if (Resources[c.Resource] < c.Amount) return false;
        }
        return true;
    }

    public void SubtractUpgradeCost(UpgradeCost cost)
    {
        foreach (var c in cost.cost)
        {
            AddResource(c.Resource, -c.Amount);
        }
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
        Application.targetFrameRate = 60;
    }

    void UpdatePathFindingGrids(bool teleport, Vector2 position = default)
    {
        /*if (teleport)
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
        }*/
    }
    public Sprite GetIcon(EResource resource)
    {
        return Icon.GetIcon(resource);
    }
}