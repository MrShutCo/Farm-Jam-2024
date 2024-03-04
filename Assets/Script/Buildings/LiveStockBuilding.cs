using System;
using System.Linq;
using System.Collections.Generic;
using Assets.Script.Humans;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;



namespace Assets.Script.Buildings
{
    public class LiveStockBuilding : Building
    {
        [SerializeField] TextMeshProUGUI capacityText;

        public void Update()
        {
            //capacityText.text = $"{CurrHumans}/{MaxCapacity}";
        }
        public override void AssignHuman(Human human, Vector2 mouseWorldPosition)
        {
            human.StopAllJobs();
        }

    }
}

