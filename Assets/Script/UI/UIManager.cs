using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using TMPro;
using UnityEngine;

namespace Assets.Script.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("HomeBase Elements")]
        [SerializeField]
        RectTransform resourcesHome;

        [SerializeField] private RectTransform resourcesBuild;

        [Header("Player Elements")]
        [SerializeField] RectTransform carriedHumansParent;
        [SerializeField] TextMeshProUGUI carriedResourcesTexts;
        [SerializeField] RectTransform carriedHumanBackGround;
        Coroutine carriedHumanFlash;
        [SerializeField] RectTransform carriedResourceBackGround;
        [SerializeField] GameObject playerHealthPanel;
        [SerializeField] FloatingStatusBar playerHealthBar;

        [Header("Performance Elements")]
        [SerializeField] TextMeshProUGUI performanceTexts;

        [Header("Death Elements")]
        [SerializeField] RectTransform humansLostText;
        [SerializeField] RectTransform humansLostDisplay;
        [SerializeField] RectTransform resourcesLostText;
        [SerializeField] RectTransform resourcesLostDisplay;
        [SerializeField] RectTransform deathButton;

        [Header("Icons")]
        [SerializeField] Icon iconPrefab;
        List<Icon> homeResourceIcons;
        private List<Icon> buildResourceIcons;
        List<Icon> playerResourceIcons;

        [SerializeField] bool showFPS;


        [SerializeField] private Canvas normalCanvas;
        [SerializeField] private Canvas buildCanvas;
        [SerializeField] private Canvas deathCanvas;
        [SerializeField] private Canvas dialogueCanvas;
        [SerializeField] private Canvas wildCanvas;

        private void Awake()
        {

            playerResourceIcons = new List<Icon>();
            homeResourceIcons = PopulateResourceIcons(resourcesHome);
            buildResourceIcons = PopulateResourceIcons(resourcesBuild);
            PopulatePlayerCarried();
        }
        void OnEnable()
        {
            Debug.Log("Game Manager: " + GameManager.Instance);
            GameManager.Instance.onHealthChange += OnHealthUpdate;
            GameManager.Instance.onResourceChange += onResourceUpdate;
            GameManager.Instance.onCarriedHumansChange += OnCarriedHumansUpdate;
            GameManager.Instance.onCarriedResourcesChange += OnCarriedResourcesUpdate;
            GameManager.Instance.onHumanCarrierFull += OnHumanCarrierFull;
            buildCanvas.gameObject.SetActive(false);
            deathCanvas.gameObject.SetActive(false);
            wildCanvas.gameObject.SetActive(false);
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
                    case EGameState.Wild:
                        disableAllCanvas();
                        normalCanvas.gameObject.SetActive(true);
                        wildCanvas.gameObject.SetActive(true);
                        break;
                    case EGameState.Death:
                        disableAllCanvas();
                        normalCanvas.gameObject.SetActive(true);
                        deathCanvas.gameObject.SetActive(true);
                        wildCanvas.gameObject.SetActive(true);
                        StartCoroutine(DeathUpdate());
                        break;
                    case EGameState.Dialogue:
                        disableAllCanvas();
                        normalCanvas.gameObject.SetActive(true);
                        dialogueCanvas.gameObject.SetActive(true);
                        Debug.Log("Activating DIalogue canvas: " + dialogueCanvas.gameObject.activeSelf);

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
            wildCanvas.gameObject.SetActive(false);
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

        List<Icon> PopulateResourceIcons(RectTransform parent)
        {
            var resources = EResource.GetValues(typeof(EResource));
            var icons = new List<Icon>();
            for (int i = 0; i < resources.Length; i++)
            {
                var resource = (EResource)resources.GetValue(i);
                var iconHome = Instantiate(iconPrefab, parent);
                iconHome.SetIcon(resource, 0);
                iconHome.transform.localPosition = new Vector3(0, -i * 15f);
                icons.Add(iconHome);
            }

            return icons;
        }

        void PopulatePlayerCarried()
        {
            var resources = EResource.GetValues(typeof(EResource));
            for (int i = 0; i < resources.Length; i++)
            {
                var resource = (EResource)resources.GetValue(i);
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

            foreach (var resource in resources)
            {
                homeResourceIcons[i].SetIcon(resource.Key, resource.Value);
                buildResourceIcons[i].SetIcon(resource.Key, resource.Value);
                i++;
            }

        }

        #region  Carrier UI

        void OnHumanCarrierFull()
        {
            if (carriedHumanFlash == null)
                carriedHumanFlash = StartCoroutine(FlashBackground(carriedHumanBackGround));
        }
        IEnumerator FlashBackground(RectTransform background)
        {
            var color = background.GetComponent<UnityEngine.UI.Image>().color;
            var originalColor = color;
            for (int i = 0; i < 3; i++)
            {
                color.a = 1;
                color = Color.red;
                background.GetComponent<UnityEngine.UI.Image>().color = color;
                yield return new WaitForSeconds(.25f);
                color.a = 0;
                background.GetComponent<UnityEngine.UI.Image>().color = color;
                yield return new WaitForSeconds(.25f);
            }
            background.GetComponent<UnityEngine.UI.Image>().color = originalColor;
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
            if (resources == null) return;
            var text = "";
            //find the playrResourceIcons that matcht he updated resources and update them
            foreach (var resource in resources)
            {
                for (int i = 0; i < playerResourceIcons.Count; i++)
                {
                    if (playerResourceIcons[i].assignedResource == resource.Key)
                    {
                        playerResourceIcons[i].SetIcon(resource.Key, resource.Value);
                    }
                }
            }
            carriedResourcesTexts.text = text;
        }

        void UpdateCarrierBackgroundSize(RectTransform carrierBackGround, int qty, float width = 20.0f)
        {
            var size = carrierBackGround.sizeDelta;
            size.x = qty * width;
            carrierBackGround.sizeDelta = size;
        }

        #endregion
        void OnPerformanceUpdate()
        {
            //show fps as a whole number that only updates once per second
            performanceTexts.text = $"FPS: {Mathf.Round(1 / Time.deltaTime)} \n Time: {(int)Time.timeSinceLevelLoad}";
        }
        void OnHealthUpdate(float currentHealth, float maxHealth)
        {
            if (currentHealth < (float)(maxHealth * .4f))
                playerHealthBar.ActivateFlash(true, (float)currentHealth / maxHealth);
            else playerHealthBar.ActivateFlash(false);
            playerHealthBar.UpdateStatusBar(currentHealth, maxHealth);
        }

        #region Death UI
        void OnShowHumansLost()
        {
            var lostHumans = GameManager.Instance.Carrier.LoseHumans();
            for (int i = 0; i < lostHumans.Count; i++)
            {
                var human = lostHumans[i];
                human.transform.SetParent(humansLostDisplay.transform);
                human.transform.position = humansLostDisplay.position + new Vector3(i, 0);
                human.GetComponentInChildren<Renderer>().sortingOrder = 150;
            }
        }
        void OnShowResourcesLost()
        {
            var lostResources = GameManager.Instance.Carrier.LoseResources();
            int xPos = 0;
            int separation = 3;
            foreach (var resource in lostResources)
            {
                if (resource.Value <= 0) continue;
                var icon = Instantiate(iconPrefab, resourcesLostDisplay.transform);
                icon.transform.position = resourcesLostDisplay.position + new Vector3(xPos * separation, 0);
                icon.SetIcon(resource.Key, resource.Value);
                icon.GetComponent<Renderer>().sortingOrder = 150;
                xPos++;
            }
            GameManager.Instance.onCarriedResourcesChange?.Invoke(GameManager.Instance.Carrier.CarriedResources);
        }

        IEnumerator DeathUpdate()
        {
            deathButton.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            humansLostText.gameObject.SetActive(false);
            resourcesLostText.gameObject.SetActive(false);
            WaitForSeconds wait = new WaitForSeconds(.5f);
            yield return wait;
            humansLostText.gameObject.SetActive(true);
            OnShowHumansLost();
            yield return wait;
            resourcesLostText.gameObject.SetActive(true);
            OnShowResourcesLost();
            yield return wait;
            deathButton.transform.localScale = new Vector3(1, 1, 1);
        }
        #endregion
    }
}