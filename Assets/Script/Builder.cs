using Assets.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Builder : MonoBehaviour
{

    public Tilemap Tilemap;
    public Tile Tile;
    public Camera Camera;

    PlayerControls playerControls;
    InputAction build;
    InputAction changeSelection;

    bool isBuildMode;
    int currentTileLayout;
    List<List<(int, int)>> TileLayouts;

    public List<GameObject> Foundations;

    Vector3Int prevMouseTile;
    Vector3Int currMouseTile;

    Transform parent;


    // Start is called before the first frame update
    void Start()
    {
        TileLayouts = new List<List<(int, int)>>()
        {
            new(){ (0,0) }, // 1x1
            new(){ (0,0), (1,0) }, // 2x1
            new(){ (0,0), (0,-1), (1,-1), (1,0) }, // 2x2
            new(){ (0,0), (0, 1), (0, -1), (1,1), (1, -1)} // C
         };
        //foundations = new List<EPlaceable>() { EPlaceable.SmallFarm, EPlaceable.Wall, EPlaceable.MediumFarm, EPlaceable.MediumTurret }
    }

    private void OnEnable()
    {
        build = playerControls.Player.Fire;
        build.Enable();
        build.performed += AttemptToPlace;
        changeSelection = playerControls.Player.SelectBuilding;
        changeSelection.Enable();
        changeSelection.performed += Switch;
        isBuildMode = true;
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();


        parent = GameObject.Find("Buildings")?.transform;
        if (parent == null)
            parent = new GameObject("Buildings").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBuildMode)
        {
            // Show outline of current 
            currMouseTile = MouseToCellPos();
            if (currMouseTile != prevMouseTile)
            {
                Clear8x8Area(prevMouseTile, 1);
            }
            else
            {
                var placeColor = IsValidPlacement(currMouseTile, TileLayouts[currentTileLayout], 0) ? Color.green : Color.red;
                PlaceTilePatternOnLayer(currMouseTile, TileLayouts[currentTileLayout], 1, Tile, placeColor);
            }
            prevMouseTile = currMouseTile;
        }
    }

    void AttemptToPlace(InputAction.CallbackContext context)
    {
        var v = MouseToCellPos();
        var layout = TileLayouts[currentTileLayout];

        if (IsValidPlacement(v, layout, 0))
        {
            var newBuilding = Instantiate(Foundations[currentTileLayout]);
            var worldPos = Tilemap.CellToWorld(v);
            newBuilding.transform.position = worldPos + new Vector3(0.5f, 0.5f, -worldPos.z);

            PlaceTilePatternOnLayer(v, layout, 0, Tile, Color.white);
            PlaceTilePatternOnLayer(v, layout, 1, Tile, Color.red);

            newBuilding.transform.parent = parent;

        }
    }

    Vector3Int MouseToCellPos()
    {
        Vector3 mousePos = Input.mousePosition;
        var worldPos = Camera.ScreenToWorldPoint(mousePos);
        return Tilemap.WorldToCell(worldPos);
    }

    bool IsValidPlacement(Vector3Int origin, List<(int, int)> layout, int layer)
    {
        foreach (var tile in layout)
        {
            var potentialSpot = Tilemap.GetTile(origin + new Vector3Int(tile.Item1, tile.Item2, 0));
            if (potentialSpot is not null) return false;
        }
        return true;
    }

    void PlaceTilePatternOnLayer(Vector3Int origin, List<(int, int)> layout, int layer, Tile tilebase, Color colorMask)
    {
        // Place the building otherwise
        foreach (var tile in layout)
        {
            var tilebaseCopy = tilebase;
            if (tilebaseCopy)
                tilebaseCopy.color = colorMask;
            Tilemap.SetTile(origin + new Vector3Int(tile.Item1, tile.Item2, layer), tilebaseCopy);
        }
    }

    void Switch(InputAction.CallbackContext context)
    {
        Debug.Log(context.control.name);
        currentTileLayout = int.Parse(context.control.name) - 1;
        Clear8x8Area(currMouseTile, 1);
    }

    void Clear8x8Area(Vector3Int origin, int layer)
    {
        for (int x = -4; x < 4; x++)
            for (int y = -4; y < 4; y++)
                Tilemap.SetTile(origin + new Vector3Int(x, y, layer), null);
    }
}
