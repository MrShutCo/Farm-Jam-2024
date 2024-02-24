using System.Collections;
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
        [SerializeField] TextMeshProUGUI carriedHumansTexts;
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
                };
            };
        }

        private void disableAllCanvas()
        {
            normalCanvas.gameObject.SetActive(false);
            buildCanvas.gameObject.SetActive(false);
            deathCanvas.gameObject.SetActive(false);
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

        void onResourceUpdate()
        {
            if (homeResourceIcons != null)
            {
                foreach (var sprite in homeResourceIcons)
                {
                    Destroy(sprite.gameObject);
                }
            }
            homeResourceIcons = new List<Icon>();
            var resources = GameManager.Instance.Resources;
            if (resources == null)
            {
                return;
            }
            int i = 0;
            var text = "";
            foreach (var resource in resources)
            {
                if (resource.Value == 0)
                {
                    continue;
                }

                Icon icon = Instantiate(iconPrefab, resourceTexts.transform);
                icon.SetIcon(resource.Key, resource.Value);
                icon.transform.position = resourceTexts.transform.position + new Vector3(0, -i * 3f);
                homeResourceIcons.Add(icon);
                i++;
            }
            resourceTexts.text = text;
        }
        void OnCarriedHumansUpdate(List<Human> humans)
        {
            int i = 0;
            var text = "";
            foreach (var human in humans)
            {
                human.GetComponent<Renderer>().sortingOrder = 120;
                i++;
            }
            carriedHumansTexts.text = text;
            UpdateCarrierBackgroundSize(carriedHumanBackGround, humans.Count);
        }
        void OnCarriedResourcesUpdate(Dictionary<EResource, int> resources)
        {
            if (playerResourceIcons != null)
            {
                foreach (var sprite in playerResourceIcons)
                {
                    Destroy(sprite.gameObject);
                }
            }
            playerResourceIcons = new List<Icon>();
            int i = 0;
            var text = "";
            foreach (var resource in resources)
            {
                Icon icon = Instantiate(iconPrefab, carriedResourcesTexts.transform);
                icon.SetIcon(resource.Key, resource.Value);
                //make the icon line up with the text
                icon.transform.position = carriedResourcesTexts.transform.position + new Vector3(i * 2, 0);
                playerResourceIcons.Add(icon);
                i++;
            }
            carriedResourcesTexts.text = text;
            UpdateCarrierBackgroundSize(carriedResourceBackGround, resources.Count, 32);
        }

        void UpdateCarrierBackgroundSize(RectTransform carrierBackGround, int qty, float width = 20)
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