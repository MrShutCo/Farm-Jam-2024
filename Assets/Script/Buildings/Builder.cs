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

        [SerializeField] Tilemap GroundMap;
        [SerializeField] Tilemap EffectMap;
        [SerializeField] Tilemap ColliderMap;
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
            /*if (Input.GetKeyDown(KeyCode.B))
            {
                isBuildMode = !isBuildMode;
                EffectMap.ClearAllTiles();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                EffectMap.ClearAllTiles();

                GameManager.Instance.EnabledPlaceables[2] = FleshFactory.CreateFlesh(UnityEngine.Random.Range(1,5),UnityEngine.Random.Range(1,5), UnityEngine.Random.Range(3,12), 0);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                EffectMap.ClearAllTiles();
                GameManager.Instance.EnabledPlaceables[currentTileLayout].RotateCW();
            }*/

            if (GameManager.Instance.GameState == EGameState.Build)
            {
                if (Input.GetMouseButtonDown(0))
                    AttemptToPlace();
                CheckSwitch();

                // Show outline of current 
                currMouseTile = MouseToCellPos();
                if (currMouseTile != prevMouseTile)
                {
                    EffectMap.ClearAllTiles();
                }
                else
                {
                    var layout = GameManager.Instance.EnabledPlaceables[currentTileLayout].Layout;
                    var placeColor = IsValidPlacement(currMouseTile, layout, ColliderMap) ? Color.green : Color.red;
                    PlaceTilePatternOnLayer(currMouseTile, layout, EffectMap, WalkableTile, placeColor);
                }
                prevMouseTile = currMouseTile;
            }
        }

        void AttemptToPlace()
        {
            var v = MouseToCellPos();
            var placeable = GameManager.Instance.EnabledPlaceables[currentTileLayout];

            if (IsValidPlacement(v, placeable.Layout, ColliderMap))
            {
                
                //var newBuilding = Instantiate(placeable.BaseObject);
                //newBuilding.GetComponent<Building>().Origin = new Vector2Int(v.x, v.y);
                var worldPos = GroundMap.CellToWorld(v);
                //newBuilding.transform.position = worldPos + placeable.GetMidpoint() - new Vector3(0,0, worldPos.z);

                var col = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
                PlaceTilePatternOnLayer(v, placeable.Layout, ColliderMap, placeable.IsWalkable ? WalkableTile : UnwalkableTile, col);
                PlaceTilePatternOnLayer(v, placeable.Layout, EffectMap, WalkableTile, Color.red);
                foreach (var pos in placeable.Layout)
                {
                    var actualPos = new Vector3Int(pos.x, pos.y, 0) + v;
                    GameManager.Instance.PathfindingGrid.SetWalkableAt(actualPos.x, actualPos.y, false);
                }

                //newBuilding.transform.parent = parent;

            }
        }

        Vector3Int MouseToCellPos()
        {
            Vector3 mousePos = Input.mousePosition;
            var worldPos = Camera.ScreenToWorldPoint(mousePos);
            return GroundMap.WorldToCell(worldPos);
        }

        bool IsValidPlacement(Vector3Int origin, List<Vector2Int> layout, Tilemap tilemap)
        {
            foreach (var tile in layout)
            {
                var potentialSpot = tilemap.GetTile(origin + new Vector3Int(tile.x, tile.y, 0));
                if (potentialSpot is not null) return false;
            }
            return true;
        }

        void PlaceTilePatternOnLayer(Vector3Int origin, List<Vector2Int> layout, Tilemap tilemap, Tile tilebase, Color colorMask)
        {
            // Place the building otherwise
            foreach (var tile in layout)
            {
                var tilebaseCopy = tilebase;
                if (tilebaseCopy)
                    tilebaseCopy.color = colorMask;
                tilemap.SetTile(origin + new Vector3Int(tile.x, tile.y, 0), tilebaseCopy);
            }
        }

        void CheckSwitch()
        {
            for (int i = 1; i <= GameManager.Instance.EnabledPlaceables.Count; i++)
            {
                if (Input.GetKeyDown($"{i}"))
                {
                    EffectMap.ClearAllTiles();
                    currentTileLayout = i-1;
                }
            }
        }
    }
}