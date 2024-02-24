using Assets.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Script.Buildings;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Assets.Script.Buildings
{
    public class Builder : MonoBehaviour
    {
        [SerializeField] Tilemap GroundMap;
        [SerializeField] Tilemap EffectMap;
        [SerializeField] Tilemap ColliderMap;
        [SerializeField] Tile WalkableTile;
        [SerializeField] Tile UnwalkableTile;
        [SerializeField] private ResourceBuildingDataSO[] buildings;
        [SerializeField] private GameObject buildingPrefab;
        
        public Camera Camera;
        public int SelectedBuilding = -1;
        
        bool isBuildMode;
        Vector3Int currMouseTile;
        Transform parent;
        GameObject ghostBuilding;

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

        public void OnClickBuildingImage(int buildingIdx)
        {
            if (ghostBuilding && SelectedBuilding != buildingIdx) Destroy(ghostBuilding);
            SelectedBuilding = buildingIdx;
            Debug.Log($"Selected building {buildingIdx}");

            buildingPrefab.GetComponent<ResourceBuilding>().buildingData = buildings[SelectedBuilding];
            buildingPrefab.GetComponent<BoxCollider2D>().enabled = false;
            ghostBuilding = Instantiate(buildingPrefab, parent);
        }

        // Update is called once per frame
        void Update()
        {
            if (GameManager.Instance.GameState == EGameState.Build && SelectedBuilding != -1)
            {
                if (Input.GetMouseButtonDown(1))
                    AttemptToPlace();

                // Show outline of current 
                currMouseTile = MouseToCellPos();
                if (ghostBuilding)
                {
                    ghostBuilding.transform.position = currMouseTile;
                }
            }
        }

        void AttemptToPlace()
        {
            var placeable = buildings[SelectedBuilding];
            var mid = placeable.GetMidpoint();
            var v = MouseToCellPos() - new Vector3Int(mid.x, mid.y);

            if (IsValidPlacement(v, placeable.Layout, ColliderMap))
            {
                PlaceTilePatternOnLayer(v, placeable.Layout, ColliderMap, placeable.IsWalkable ? WalkableTile : UnwalkableTile);
                ghostBuilding.GetComponent<BoxCollider2D>().enabled = true;
                ghostBuilding = null;
                SelectedBuilding = -1;
                for (int x = 0; x < placeable.Layout.x; x++)
                for (int y = 0; y < placeable.Layout.y; y++)
                {
                    var actualPos = new Vector3Int(x, y, 0) + v;
                    GameManager.Instance.PathfindingGrid.SetWalkableAt(actualPos.x, actualPos.y, false);
                }
            }
        }

        Vector3Int MouseToCellPos()
        {
            Vector3 mousePos = Input.mousePosition;
            var worldPos = Camera.ScreenToWorldPoint(mousePos);
            return GroundMap.WorldToCell(worldPos);
        }

        bool IsValidPlacement(Vector3Int origin, Vector2Int layout, Tilemap tilemap)
        {
            for (int x = 0; x < layout.x; x++)
            for (int y = 0; y < layout.y; y++)
            {
                var potentialSpot = tilemap.GetTile(origin + new Vector3Int(x,y, 0));
                if (potentialSpot is not null) return false;
            }
            return true;
        }

        void PlaceTilePatternOnLayer(Vector3Int origin, Vector2Int layout, Tilemap tilemap, Tile tilebase)
        {
            for (int x = 0; x < layout.x; x++)
            for (int y = 0; y < layout.y; y++)
            {
                tilebase.color = new Color(0, 0, 0, 0);
                tilemap.SetTile(origin + new Vector3Int(x, y, 0), tilebase);
            }
        }
    }
}