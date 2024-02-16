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
        public Human FlayedHuman { get; private set; }

        public ResourceBuilding()
		{
            workingHumans = new List<Human>();
            
        }

        public void Start()
        {
            GameManager.Instance.onHumanDie += onHumanDie;
        }

        public void OnDisable()
        {
            GameManager.Instance.onHumanDie -= onHumanDie;
        }

        public Job? AssignHuman(Human h)
        {
            //var a = humans.Single(h => h.human == currentlySelect);
            Job task;
            if (AtCapacity()) return null;

            var workingPosition = workingPositions[CurrHumans];

            if (FlayedHuman is null)
            {
                task = GetFlayed(h);
                FlayedHuman = h;
            }
            else
            {
                task = Flay(h, workingPosition.position);
                workingHumans.Add(h);
            }
            CurrHumans++;
            
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

        Job Flay(Human h, Vector3 pos)
        {
            return new Job(h, "Work", new List<Task>()
            {
                    new MoveToTask(pos),
                    new WorkTask(this, FlayedHuman),
                    new MoveToTask(Vector3.zero),
                    new DropoffResources(HarvestedResouce, 5)
            }, true);
        }

        Job WaitForNewFlay(Human h, Vector3 pos)
        {
            return new Job(h, "Wait for flay", new List<Task>()
            {
                new MoveToTask(pos),
                new Idle()
            }, false);
        }

        Job GetFlayed(Human h)
        {
            return new Job(h, "Move to be flayed", new List<Task>()
                {
                    new MoveToTask(workingPositions[0].position),
                }, false);
        }

        void onHumanDie(Human h)
        {
            if (h == FlayedHuman)
            {
                FlayedHuman = null;
                for (int i = 0; i < workingHumans.Count; i++)
                    GameManager.Instance.HumanOrchestrator.AssignJobToHuman(WaitForNewFlay(workingHumans[i], workingPositions[i].position), workingHumans[i], false);
            }
        }
    }
}

