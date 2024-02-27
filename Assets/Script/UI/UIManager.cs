﻿using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Assets.Script.Humans;
using TMPro;
using UnityEngine;

namespace Assets.Script.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("HomeBase Elements")]
        [SerializeField]
        TextMeshProUGUI resourceTexts;

        [Header("Player Elements")]
        [SerializeField] RectTransform carriedHumansParent;
        [SerializeField] TextMeshProUGUI carriedResourcesTexts;
        [SerializeField] RectTransform carriedHumanBackGround;
        [SerializeField] RectTransform carriedResourceBackGround;
        [SerializeField] GameObject playerHealthPanel;
        [SerializeField] FloatingStatusBar playerHealthBar;

        [Header("Performance Elements")]
        [SerializeField] TextMeshProUGUI performanceTexts;

        [Header("Death Elements")]
        [SerializeField] RectTransform humansLostDisplay;
        [SerializeField] RectTransform resourcesLostDisplay;

        [Header("Icons")]
        [SerializeField] Icon iconPrefab;
        List<Icon> homeResourceIcons;
        List<Icon> playerResourceIcons;

        [SerializeField] bool showFPS;

        [SerializeField] private Canvas normalCanvas;
        [SerializeField] private Canvas buildCanvas;
        [SerializeField] private Canvas deathCanvas;
        [SerializeField] private Canvas dialogueCanvas;

        private void Awake()
        {
            homeResourceIcons = new List<Icon>();
            playerResourceIcons = new List<Icon>();
            PopulateResourceIcons();
        }
        void OnEnable()
        {
            Debug.Log("Game Manager: " + GameManager.Instance);
            GameManager.Instance.onHealthChange += OnHealthUpdate;
            GameManager.Instance.onResourceChange += onResourceUpdate;
            GameManager.Instance.onCarriedHumansChange += OnCarriedHumansUpdate;
            GameManager.Instance.onCarriedResourcesChange += OnCarriedResourcesUpdate;
            buildCanvas.gameObject.SetActive(false);
        }
        private void Start()
        {
            GameManager.Instance.onGameStateChange += state =>
            {
                switch (state)
                {
                    case EGameState.Build:
                        disableAllCanvas();
                        buildCanvas.gameObject.SetActive(true);
                        break;
                    case EGameState.Normal:
                        disableAllCanvas();
                        normalCanvas.gameObject.SetActive(true);
                        break;
                    case EGameState.Death:
                        disableAllCanvas();
                        deathCanvas.gameObject.SetActive(true);
                        OnShowHumansLost();
                        OnShowResourcesLost();
                        break;
                    case EGameState.Dialogue:
                        disableAllCanvas();
                        normalCanvas.gameObject.SetActive(true);
                        dialogueCanvas.gameObject.SetActive(true);
                        break;
                };
            };
        }

        private void disableAllCanvas()
        {
            normalCanvas.gameObject.SetActive(false);
            buildCanvas.gameObject.SetActive(false);
            deathCanvas.gameObject.SetActive(false);
            dialogueCanvas.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            GameManager.Instance.onResourceChange -= onResourceUpdate;
        }

        // Update is called once per frame
        void Update()
        {
            if (showFPS)
                OnPerformanceUpdate();
        }

        void PopulateResourceIcons()
        {
            var resources = EResource.GetValues(typeof(EResource));
            for (int i = 0; i < resources.Length; i++)
            {
                var resource = (EResource)resources.GetValue(i);
                var iconHome = Instantiate(iconPrefab, resourceTexts.transform);
                iconHome.SetIcon(resource, 0);
                iconHome.transform.localPosition = new Vector3(0, -i * 15f);
                homeResourceIcons.Add(iconHome);

                var iconCarried = Instantiate(iconPrefab, carriedResourcesTexts.transform);
                iconCarried.SetIcon(resource, 0);
                iconCarried.transform.localPosition = new Vector3(i * 36, 0);
                playerResourceIcons.Add(iconCarried);
                UpdateCarrierBackgroundSize(carriedResourceBackGround, playerResourceIcons.Count, 36);
            }
        }

        void onResourceUpdate()
        {
            var resources = GameManager.Instance.Resources;
            if (resources == null)
            {
                return;
            }
            int i = 0;
            var text = "";
            foreach (var resource in resources)
            {
                homeResourceIcons[i].SetIcon(resource.Key, resource.Value);
                i++;
            }
            resourceTexts.text = text;
        }
        void OnCarriedHumansUpdate(List<Human> humans)
        {
            int i = 0;
            foreach (var human in humans)
            {
                human.transform.SetParent(carriedHumansParent);
                i++;
            }
            OrganizeCarriedHumans(humans);
            UpdateCarrierBackgroundSize(carriedHumanBackGround, humans.Count);
        }

        void OrganizeCarriedHumans(List<Human> humans)
        {
            Vector2 humanTrackerPos = carriedHumansParent.position;
            //organize humans on tracker
            for (int i = 0; i < humans.Count; i++)
            {
                humans[i].transform.position = humanTrackerPos + new Vector2(i, .5f);
            }
        }
        void OnCarriedResourcesUpdate(Dictionary<EResource, int> resources)
        {
            int i = 0;
            var text = "";
            foreach (var resource in resources)
            {
                playerResourceIcons[i].SetIcon(resource.Key, resource.Value);
                i++;
            }
            carriedResourcesTexts.text = text;
        }

        void UpdateCarrierBackgroundSize(RectTransform carrierBackGround, int qty, float width = 20.0f)
        {
            var size = carrierBackGround.sizeDelta;
            size.x = qty * width;
            carrierBackGround.sizeDelta = size;
        }
        void OnPerformanceUpdate()
        {
            //show fps as a whole number that only updates once per second
            performanceTexts.text = $"FPS: {Mathf.Round(1 / Time.deltaTime)}";
        }
        void OnHealthUpdate(int currentHealth, int maxHealth)
        {
            playerHealthBar.UpdateStatusBar(currentHealth, maxHealth);
        }
        void OnShowHumansLost()
        {
            var lostHumans = GameManager.Instance.Carrier.LoseHumans();
            for (int i = 0; i < lostHumans.Count; i++)
            {
                var human = lostHumans[i];
                human.transform.SetParent(humansLostDisplay.transform);
                human.transform.position = humansLostDisplay.position + new Vector3(i, 0);
                human.GetComponent<Renderer>().sortingOrder = 120;
            }
        }
        void OnShowResourcesLost()
        {
            var lostResources = GameManager.Instance.Carrier.LoseResources();
            int xPos = 0;
            int separation = 3;
            foreach (var resource in lostResources)
            {
                var icon = Instantiate(iconPrefab, resourcesLostDisplay.transform);
                icon.transform.position = resourcesLostDisplay.position + new Vector3(xPos * separation, 0);
                icon.SetIcon(resource.Key, resource.Value);
                xPos++;
            }
        }


    }
}