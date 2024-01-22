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

        // Use this for initialization
        void Start()
        {
            GameManager.Instance.onResourceChange += onResourceUpdate;
            GameManager.Instance.onCarriedHumansChange += OnCarriedHumansUpdate;
            GameManager.Instance.onCarriedResourcesChange += OnCarriedResourcesUpdate;
        }

        private void OnDisable()
        {
            GameManager.Instance.onResourceChange -= onResourceUpdate;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void onResourceUpdate()
        {
            var resources = GameManager.Instance.Resources;
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
    }
}