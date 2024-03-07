using Assets.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Humans;
using Cinemachine;
using Script.Buildings;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Unity.VisualScripting;

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
        [SerializeField] private Image[] buildingImage;
        [SerializeField] private GameObject buildingPrefab;
        [SerializeField] LayerMask gridLayer;

        public int SelectedBuilding = -1;

        bool isBuildMode;
        Vector3 currMouseTile;
        Transform parent;
        GameObject ghostBuilding;
        private BodyPartBuilding ghostResourceBuilding;
        private Camera _camera;

        SoundRequest selectBuildingSound;
        SoundRequest cannotBuildSound;
        SoundRequest constructionCompleteSound;

        [Header("Building Costs")]
        [Range(1, 4)]
        [SerializeField] private float costRatioMultiplier;
        [SerializeField] private float baseCost;
        [SerializeField] private TextMeshProUGUI bloodCostText;
        [SerializeField] private TextMeshProUGUI boneCostText;
        [SerializeField] private TextMeshProUGUI organCostText;

        private Dictionary<EResource, float> costRatios;
        private void Start()
        {
            _camera = Camera.main;
            costRatios = new Dictionary<EResource, float>()
            {
                { EResource.Blood, 1}, { EResource.Organs, 1}, { EResource.Bones, 1 }
            };
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
            if (!HasEnough(getTypeOfBuilding(buildingIdx)))
            {
                StartCoroutine(FlashImage(buildingIdx, Color.red, 2));
                GameManager.Instance.onPlaySound?.Invoke(cannotBuildSound);
                return;
            }

            if (ghostBuilding && SelectedBuilding != buildingIdx) Destroy(ghostBuilding);
            SelectedBuilding = buildingIdx;
            Debug.Log($"Selected building {buildingIdx}");

            buildingPrefab.GetComponent<ResourceBuilding>().buildingData = buildings[SelectedBuilding];
            buildingPrefab.GetComponent<BoxCollider2D>().enabled = false;
            ghostBuilding = Instantiate(buildingPrefab, parent);
            ghostResourceBuilding = ghostBuilding.GetComponent<BodyPartBuilding>();
            ghostResourceBuilding.SetLevel(GameManager.Instance.BaseBuildLevel[getTypeOfBuilding(buildingIdx)]);
            GameManager.Instance.onPlaySound?.Invoke(selectBuildingSound);
            StartCoroutine(FlashImage(buildingIdx, Color.green, 1));
        }
        IEnumerator FlashImage(int buildingIdx, Color color, int flashes, float duration = 0.1f)
        {
            WaitForSeconds wait = new WaitForSeconds(duration);
            var image = buildingImage[buildingIdx];
            var originalColor = image.color;

            for (int i = 0; i < flashes; i++)
            {
                image.color = color;
                yield return wait;
                image.color = originalColor;
                yield return wait;
            }
        }


        EResource getTypeOfBuilding(int buildingIdx)
        {
            if (buildingIdx == 0) return EResource.Blood;
            if (buildingIdx == 1) return EResource.Bones;
            if (buildingIdx == 2) return EResource.Organs;
            return EResource.Blood;
        }

        bool HasEnough(EResource resource)
        {
            return GameManager.Instance.Resources[EResource.Wood] >= baseCost * costRatios[resource];
        }

        void SetCostText(EResource resource)
        {
            var amount = (int)(baseCost * costRatios[resource]);
            if (resource == EResource.Blood) bloodCostText.text = $"{amount} Wood";
            if (resource == EResource.Bones) boneCostText.text = $"{amount} Wood";
            if (resource == EResource.Organs) organCostText.text = $"{amount} Wood";
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

            if (GameManager.Instance.GameState == EGameState.Build && (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Backspace)))
                GameManager.Instance.SetGameState(EGameState.Normal);
        }

        void AttemptToPlace()
        {
            var placeable = buildings[SelectedBuilding];
            var v = GetTileOverMouse(placeable);

            if (IsValidPlacement(v, placeable.Layout))
            {
                Debug.Log(v.ToString());
                PlaceBuildingOnCollisionLayer(v, placeable.Layout);
                ghostBuilding.GetComponent<BoxCollider2D>().enabled = true;
                ghostBuilding = null;
                GameManager.Instance.onPlaySound?.Invoke(constructionCompleteSound);

                var typeofBuilding = getTypeOfBuilding(SelectedBuilding);
                GameManager.Instance.AddResource(EResource.Wood, (int)(-baseCost * costRatios[typeofBuilding]));
                costRatios[typeofBuilding] *= costRatioMultiplier;
                SetCostText(typeofBuilding);
                GameManager.Instance.Buildings.Add(ghostResourceBuilding);
                SelectedBuilding = -1;
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
            return MouseToCellPos() - new Vector3Int(mid.x, mid.y, 0);
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
            Debug.Log($"Checking is valid 3x3 at {origin}");
            origin.z = 0;
            for (int x = 0; x < layout.x; x++)
                for (int y = 0; y < layout.y; y++)
                {
                    var potentialSpot = ColliderMap.GetTile(origin + new Vector3Int(x, y, 0));
                    var potentialGroundSpot = GroundMap.GetTile(origin + new Vector3Int(x, y, 0));
                    if (!buildableOnTiles.Contains(potentialGroundSpot) || potentialSpot is not null) return false;
                }
            return true;
        }

        void PlaceBuildingOnCollisionLayer(Vector3Int origin, Vector2Int layout)
        {
            Debug.Log($"Placed 3x3 at {origin}");
            origin.z = 0;
            for (int x = 0; x < layout.x; x++)
                for (int y = 0; y < layout.y; y++)
                {
                    WalkableTile.color = new Color(0, 0, 0, 1);
                    ColliderMap.SetTile(origin + new Vector3Int(x, y, 0), WalkableTile);
                }
            ColliderMap.SetTile(origin + new Vector3Int(1, 1, 0), UnwalkableTile);
            GameManager.Instance.PathfindingGrid.SetWalkableAt(origin.x + 1, origin.y + 1, false);
        }
    }
}