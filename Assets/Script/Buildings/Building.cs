using Assets.Script.Humans;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Script.Buildings
{
    public abstract class Building : MonoBehaviour
    {
        public int CurrHumans;
        public int MaxCapacity;

        public EResource HarvestedResouce;
        public float TimeForOneSkillPoint;

        public float TimeToHarvestResource;
        float timeCollectingResource;

        [SerializeField] TextMeshProUGUI capacityText;

        public void Update()
        {
            if (CurrHumans == MaxCapacity)
            {
                timeCollectingResource += Time.deltaTime;
            }

            if (timeCollectingResource >= TimeToHarvestResource)
            {
                GameManager.Instance.AddResource(HarvestedResouce, 5);
                timeCollectingResource = 0;
            }

            capacityText.text = $"{CurrHumans}/{MaxCapacity}";
        }
    }
}