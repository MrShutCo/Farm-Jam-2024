using System;
using System.Collections.Generic;
using Assets.Script.Humans;
using TMPro;
using UnityEngine;

namespace Assets.Script.Buildings
{
	public class ResourceBuilding : Building
	{
        public EResource HarvestedResouce;
        public float TimeToCollect;

        public float TimeToHarvestResource;
        float timeCollectingResource;

        [SerializeField] TextMeshProUGUI capacityText;
        [SerializeField] Transform[] workingPositions;

        List<Human> workingHumans;

        public ResourceBuilding()
		{
            workingHumans = new List<Human>();  
        }

        public Job? AssignHuman(Human h)
        {
            //var a = humans.Single(h => h.human == currentlySelect);
            Job task;
            if (AtCapacity()) return null;

            var workingPosition = workingPositions[CurrHumans];

            if (!CanBeWorked())
            {
                task = new Job(h, "Move to be flayed", new List<Task>()
                {
                    new MoveToTask(workingPosition.position),
                }, false);
            }
            else
            {
                task = new Job(h, "Work", new List<Task>()
                {
                    new MoveToTask(workingPosition.position),
                    new WorkTask(this),
                    new MoveToTask(Vector3.zero),
                    new DropoffResources(HarvestedResouce, 5)
                }, true);
                GameManager.Instance.HumanOrchestrator.AddTaskToJob(new GetFlayed(), workingHumans[0]);
            }
            CurrHumans++;
            workingHumans.Add(h);
            return task;
        }

        public void UnAssignHuman(Human h)
        {
            workingHumans.Remove(h);
        }

        public void Update()
        {
            if (CurrHumans == MaxCapacity)
            {
                timeCollectingResource += Time.deltaTime;
            }

            capacityText.text = $"{CurrHumans}/{MaxCapacity}";
        }


    }
}

