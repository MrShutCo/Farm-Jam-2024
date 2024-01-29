using System;
using Assets.Script.Humans;
using TMPro;
using UnityEngine;

namespace Assets.Script.Buildings
{
	public class ResourceBuilding : Building
	{
        public EResource HarvestedResouce;
        public float TimeForOneSkillPoint;

        public float TimeToHarvestResource;
        float timeCollectingResource;

        [SerializeField] TextMeshProUGUI capacityText;

        public ResourceBuilding()
		{
           
        }

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

