using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Script.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI resourceTexts;

        // Use this for initialization
        void Start()
        {
            GameManager.Instance.onResourceChange += onResourceUpdate;
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
    }
}