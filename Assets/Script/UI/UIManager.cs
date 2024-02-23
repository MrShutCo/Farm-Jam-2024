using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using TMPro;
using UnityEngine;

namespace Assets.Script.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI resourceTexts;
        [SerializeField] TextMeshProUGUI carriedHumansTexts;
        [SerializeField] TextMeshProUGUI carriedResourcesTexts;
        [SerializeField] TextMeshProUGUI performanceTexts;
        [SerializeField] GameObject playerHealthPanel;
        [SerializeField] FloatingStatusBar playerHealthBar;


        [SerializeField] bool showFPS;

        [SerializeField] private Canvas normalCanvas;
        [SerializeField] private Canvas buildCanvas;

        void OnEnable()
        {
            Debug.Log("Game Manager: " + GameManager.Instance);
            GameManager.Instance.onHealthChange += OnHealthUpdate;
            GameManager.Instance.onResourceChange += onResourceUpdate;
            GameManager.Instance.onCarriedHumansChange += OnCarriedHumansUpdate;
            GameManager.Instance.onCarriedResourcesChange += OnCarriedResourcesUpdate;
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
                };
            };
        }

        private void disableAllCanvas()
        {
            normalCanvas.gameObject.SetActive(false);
            buildCanvas.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            GameManager.Instance.onResourceChange -= onResourceUpdate;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                GameManager.Instance.SetGameState(EGameState.Normal);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                GameManager.Instance.SetGameState(EGameState.Build);
            }
            if (showFPS)
                OnPerformanceUpdate();
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
                text += $"{resource.Key.ToString()}: {resource.Value}\n";
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
                text += $"{human.Name}\n";
                i++;
            }
            carriedHumansTexts.text = text;
        }
        void OnCarriedResourcesUpdate(Dictionary<EResource, int> resources)
        {
            int i = 0;
            var text = "";
            foreach (var resource in resources)
            {
                text += $"{resource.Key.ToString()}: {resource.Value}\n";
                i++;
            }
            carriedResourcesTexts.text = text;
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
    }
}