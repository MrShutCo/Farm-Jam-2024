using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Buildings;
using Assets.Script.Humans;
using UnityEngine;

namespace Script.Buildings
{
    public class BodyPartBuilding : ResourceBuilding
    {

        public BodyPartBuilding()
        {
            _workTimer = new(1, true);
            _workingHumans = new List<WorkingSubsection>()
            {
                new (new Vector2(0, 0.5f), new() { new Vector2(-1.25f, 0), new Vector2(1.25f, 0) })
            };
        }
        
        public override void AssignHuman(Human human, Vector2 mouseWorldPosition)
        {
            foreach (var subsection in _workingHumans)
            {
                // assign to flayee
                if (IsOver(subsection.FlayeePosition, mouseWorldPosition) && subsection.Flayee == null)
                {
                    subsection.Flayee = human.GetComponent<HealthBase>();
                    human.transform.position = GetWorldPosition(subsection.FlayeePosition);
                    human.AddJob(new Job(human, "flay", new List<Task>(){new GetFlayed()},false));
                    human.GetComponent<CapsuleCollider2D>().enabled = false;
                    Debug.Log("Assigned flayee");
                }
                // assign to flayer
                for (int i = 0; i < subsection.FlayerPositions.Count; i++)
                {
                    if (IsOver(subsection.FlayerPositions[i], mouseWorldPosition) && subsection.Flayers[i] == null)
                    {
                        subsection.Flayers[i] = human;
                        human.transform.position = GetWorldPosition(subsection.FlayerPositions[i]);
                        human.GetComponent<CapsuleCollider2D>().enabled = false;
                        Debug.Log("Assigned flayer");
                    }
                }
            }
        }

        protected override void OnWork()
        {
            float totalResourceGained = 0.0f;
            foreach (var group in _workingHumans)
            {
                if (group.Flayee is not null && group.IsBeingWorked() && !IsPackagesFull())
                {
                    totalResourceGained += group.Flayers.Sum(f => f == null ? 0 : f.GetWorkingRate(buildingData.resource));
                    group.Flayee.TakeDamage(5);
                    Console.WriteLine("Flayed damage taken");
                }
            }

            _internalBuffer += totalResourceGained;
            if (_internalBuffer >= buildingData.internalBufferCapacity)
            {
                AddPacket();
                _internalBuffer -= buildingData.internalBufferCapacity;
            }
            internalBufferObject.UpdateStatusBar(_internalBuffer, (int)buildingData.internalBufferCapacity);
        }
    }
}