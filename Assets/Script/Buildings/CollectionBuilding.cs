using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Buildings;
using Assets.Script.Humans;
using JetBrains.Annotations;
using UnityEngine;

namespace Script.Buildings
{
    public class CollectionBuilding : Building
    {
        protected List<Human> workingHumans;
        protected Timer actionTimer;

        [SerializeField] protected string jobName;
        [SerializeField] private float timeToCheck;

        public CollectionBuilding()
        {
            workingHumans = new List<Human>();
            actionTimer = new Timer(timeToCheck, false);
        }

        private void Update()
        {
            actionTimer.Update(Time.deltaTime);
        }

        public override void AssignHuman(Human human, Vector2 mouseWorldPosition)
        {
            if (workingHumans.Contains(human) || workingHumans.Count >= MaxCapacity) return;
            human.StopAllJobs();
            workingHumans.Add(human);
        }
        
        public override bool TryUnassignHuman(Human human)
        {
            for (int i = 0; i < workingHumans.Count; i++)
            {
                if (workingHumans[i] == human)
                {
                    workingHumans.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        [CanBeNull]
        protected Human GetNearestFreeHuman(Vector3 position)
        {
            return workingHumans.
                Where(h => h != null && !h.CurrentJobs.Any() || h.CurrentJobs.Peek().Name != jobName).
                OrderBy(h => Vector3.Distance(h.transform.position, position)).
                FirstOrDefault();
        }
    }
}