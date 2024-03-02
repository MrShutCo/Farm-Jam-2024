using Assets.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Script.Buildings;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Assets.Script.Buildings
{
    public class Builder : MonoBehaviour
    {
        [SerializeField] private Tile[] buildableOnTiles;

        [SerializeField] Tilemap GroundMap;
        [SerializeField] Tilemap EffectMap;
        [SerializeField] Tilemap ColliderMap;
        [SerializeField] Tile WalkableTile;
        [SerializeField] Tile UnwalkableTile;
        [SerializeField] private ResourceBuildingDataSO[] buildings;
        [SerializeField] private GameObject buildingPrefab;
        [SerializeField] LayerMask gridLayer;

        public int SelectedBuilding = -1;


        bool isBuildMode;
        Vector3 currMouseTile;
        Transform parent;
        GameObject ghostBuilding;
        private ResourceBuilding ghostResourceBuilding;
        private Camera _camera;

        SoundRequest selectBuildingSound;
        SoundRequest cannotBuildSound;
        SoundRequest constructionCompleteSound;


        private void Start()
        {
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            isBuildMode = false;
            GameManager.Instance.onGameStateChange += onStateChange;
            //Camera = camera.GetComponent<Camera>();
        }

        private void OnDisable()
        {
            GameManager.Instance.onGameStateChange -= onStateChange;
        }

        private void Awake()
        {
            parent = GameObject.Find("Buildings")?.transform;
            if (parent == null)
                parent = new GameObject("Buildings").transform;

            InitializeSounds();
        }
        void InitializeSounds()
        {
            selectBuildingSound = new SoundRequest
            {
                SoundSource = ESoundSource.UI,
                RequestingObject = gameObject,
                SoundType = ESoundType.buttonClick,
                RandomizePitch = false,
                Loop = false
            };
            cannotBuildSound = new SoundRequest
            {
                SoundSource = ESoundSource.UI,
                RequestingObject = gameObject,
                SoundType = ESoundType.badSelection,
                RandomizePitch = false,
                Loop = false
            };
            constructionCompleteSound = new SoundRequest
            {
                SoundSource = ESoundSource.UI,
                RequestingObject = gameObject,
                SoundType = ESoundType.constructionComplete,
                RandomizePitch = false,
                Loop = false
            };
        }

        public void OnClickBuildingImage(int buildingIdx)
        {
            if (ghostBuilding && SelectedBuilding != buildingIdx) Destroy(ghostBuilding);
            SelectedBuilding = buildingIdx;
            Debug.Log($"Selected building {buildingIdx}");

            buildingPrefab.GetComponent<ResourceBuilding>().buildingData = buildings[SelectedBuilding];
            buildingPrefab.GetComponent<BoxCollider2D>().enabled = false;
            ghostBuilding = Instantiate(buildingPrefab, parent);
            ghostResourceBuilding = ghostBuilding.GetComponent<ResourceBuilding>();
            GameManager.Instance.onPlaySound?.Invoke(selectBuildingSound);
        }

        // Update is called once per frame
        void Update()
        {
            if (GameManager.Instance.GameState == EGameState.Build && SelectedBuilding != -1)
            {
                if (Input.GetMouseButtonDown(1))
                    AttemptToPlace();

                if (ghostBuilding)
                {
                    var placeable = buildings[SelectedBuilding];
                    // Show outline of current 
                    currMouseTile = MouseToWorldPos();
                    ghostBuilding.transform.position = currMouseTile;
                    if (!IsValidPlacement(GetTileOverMouse(placeable), placeable.Layout))
                    {
                        ghostResourceBuilding.SetNotBuildable();
                    }
                    else
                    {
                        ghostResourceBuilding.SetBuildable();
                    }
                }
            }

            if (GameManager.Instance.GameState == EGameState.Build && Input.GetKeyDown(KeyCode.Escape))
                GameManager.Instance.SetGameState(EGameState.Normal);
        }

        void AttemptToPlace()
        {
            var placeable = buildings[SelectedBuilding];
            var v = GetTileOverMouse(placeable);

            if (IsValidPlacement(v, placeable.Layout))
            {
                Debug.Log(v.ToString());
                PlaceTilePatternOnLayer(v, placeable.Layout, UnwalkableTile);
                ghostBuilding.GetComponent<BoxCollider2D>().enabled = true;
                ghostBuilding = null;
                GameManager.Instance.onPlaySound?.Invoke(constructionCompleteSound);

                SelectedBuilding = -1;
                /*for (int x = 0; x < placeable.Layout.x; x++)
                    for (int y = 0; y < placeable.Layout.y; y++)
                    {
                        var actualPos = new Vector3Int(x, y, 0) + v;
                        GameManager.Instance.PathfindingGrid.SetWalkableAt(actualPos.x, actualPos.y, false);
                    }*/
            }
            else
            {
                GameManager.Instance.onPlaySound?.Invoke(cannotBuildSound);
            }
        }

        void onStateChange(EGameState state)
        {
            if (state != EGameState.Build && ghostBuilding != null)
            {
                Destroy(ghostBuilding);
                SelectedBuilding = -1;
            }
        }

        Vector3Int GetTileOverMouse(ResourceBuildingDataSO placeable)
        {
            var mid = placeable.GetMidpoint();
            return MouseToCellPos() - new Vector3Int(mid.x, mid.y);
        }

        Vector3 MouseToWorldPos()
        {
            var pos = MouseToCellPos();
            var p = GroundMap.GetCellCenterWorld(pos);
            p.z = 0;
            return p;
        }

        Vector3Int MouseToCellPos()
        {
            Vector3 mousePos = Input.mousePosition;
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            return GroundMap.WorldToCell(worldPos);
        }

        bool IsValidPlacement(Vector3Int origin, Vector2Int layout)
        {
            origin.z = 0;
            for (int x = 0; x < layout.x; x++)
                for (int y = 0; y < layout.y; y++)
                {
                    // TODO!!!!!! -1 on x and y only works if building is 3x3 but its okay because we rushin 
                    var potentialSpot = ColliderMap.GetTile(origin + new Vector3Int(x, y, 0));
                    var potentialGroundSpot = GroundMap.GetTile(origin + new Vector3Int(x, y, 0));
                    if (!buildableOnTiles.Contains(potentialGroundSpot) || potentialSpot is not null) return false;
                }
            return true;
        }

        void PlaceTilePatternOnLayer(Vector3Int origin, Vector2Int layout, Tile tilebase)
        {
            for (int x = 0; x < layout.x; x++)
                for (int y = 0; y < layout.y; y++)
                {
                    tilebase.color = new Color(0, 0, 0, 1);
                    ColliderMap.SetTile(origin + new Vector3Int(x, y, 0), tilebase);
                }
        }
    }
}