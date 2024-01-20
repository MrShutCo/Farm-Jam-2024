using Assets.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Script.Buildings
{
    public class Builder : MonoBehaviour
    {

        public Tilemap Tilemap;
        [SerializeField] Tile WalkableTile;
        [SerializeField] Tile UnwalkableTile;

        public Camera Camera;

        bool isBuildMode;
        int currentTileLayout;

        Vector3Int prevMouseTile;
        Vector3Int currMouseTile;

        Transform parent;


        // Start is called before the first frame update
        void Start()
        {
            currentTileLayout = 0;
        }

        private void OnEnable()
        {
            isBuildMode = false;
        }

        private void Awake()
        {
            parent = GameObject.Find("Buildings")?.transform;
            if (parent == null)
                parent = new GameObject("Buildings").transform;
        }

        // Update is called once per frame
        void Update()
        {
            // Toggle build mode
            if (Input.GetKeyDown(KeyCode.B))
            {
                isBuildMode = !isBuildMode;
                Clear8x8Area(currMouseTile, 1);
            }

            if (isBuildMode)
            {
                if (Input.GetMouseButtonDown(0))
                    AttemptToPlace();
                CheckSwitch();

                // Show outline of current 
                currMouseTile = MouseToCellPos();
                if (currMouseTile != prevMouseTile)
                {
                    Clear8x8Area(prevMouseTile, 1);
                }
                else
                {
                    var layout = GameManager.Instance.EnabledPlaceables[currentTileLayout].Layout;
                    var placeColor = IsValidPlacement(currMouseTile, layout, 0) ? Color.green : Color.red;
                    PlaceTilePatternOnLayer(currMouseTile, layout, 1, WalkableTile, placeColor);
                }
                prevMouseTile = currMouseTile;
            }
        }

        void AttemptToPlace()
        {
            var v = MouseToCellPos();
            var placeable = GameManager.Instance.EnabledPlaceables[currentTileLayout];

            if (IsValidPlacement(v, placeable.Layout, 0))
            {
                var newBuilding = Instantiate(placeable.BaseObject);
                var worldPos = Tilemap.CellToWorld(v);
                newBuilding.transform.position = worldPos + placeable.GetMidpoint() - new Vector3(0,0, worldPos.z);

                PlaceTilePatternOnLayer(v, placeable.Layout, 0, placeable.IsWalkable ? WalkableTile : UnwalkableTile, Color.white);
                PlaceTilePatternOnLayer(v, placeable.Layout, 1, WalkableTile, Color.red);

                newBuilding.transform.parent = parent;

            }
        }

        Vector3Int MouseToCellPos()
        {
            Vector3 mousePos = Input.mousePosition;
            var worldPos = Camera.ScreenToWorldPoint(mousePos);
            return Tilemap.WorldToCell(worldPos);
        }

        bool IsValidPlacement(Vector3Int origin, List<Vector2Int> layout, int layer)
        {
            foreach (var tile in layout)
            {
                var potentialSpot = Tilemap.GetTile(origin + new Vector3Int(tile.x, tile.y, 0));
                if (potentialSpot is not null) return false;
            }
            return true;
        }

        void PlaceTilePatternOnLayer(Vector3Int origin, List<Vector2Int> layout, int layer, Tile tilebase, Color colorMask)
        {
            // Place the building otherwise
            foreach (var tile in layout)
            {
                var tilebaseCopy = tilebase;
                if (tilebaseCopy)
                    tilebaseCopy.color = colorMask;
                Tilemap.SetTile(origin + new Vector3Int(tile.x, tile.y, layer), tilebaseCopy);
            }
        }

        void CheckSwitch()
        {
            for (int i = 1; i <= GameManager.Instance.EnabledPlaceables.Count; i++)
            {
                if (Input.GetKeyDown($"{i}"))
                {
                    Clear8x8Area(currMouseTile, 1);
                    currentTileLayout = i-1;
                }
            }
        }

        void Clear8x8Area(Vector3Int origin, int layer)
        {
            for (int x = -4; x < 4; x++)
                for (int y = -4; y < 4; y++)
                    Tilemap.SetTile(origin + new Vector3Int(x, y, layer), null);
        }
    }
}