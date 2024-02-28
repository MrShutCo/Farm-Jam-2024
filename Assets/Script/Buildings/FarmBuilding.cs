using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Buildings;
using Assets.Script.Humans;
using UnityEngine;

namespace Script.Buildings
{
    public class FarmBuilding : ResourceBuilding
    {
        [SerializeField] float extraBufferToHarvest;
        
        public FarmBuilding()
        {
            _workTimer = new Timer(0.75f, false);
            _workingHumans = new List<WorkingSubsection>(new[] { new WorkingSubsection(Vector2.zero, new ()
                {
                    Vector2.zero
                }) });
            
        }

        private void OnEnable()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public override void AssignHuman(Human human, Vector2 mouseWorldPosition)
        {
            var farmer = _workingHumans[0].Flayers;
            if (farmer[0] == null)
            {
                farmer[0] = human;
                human.StopAllJobs();
                human.AddJob(new Job(human, "Farm", new List<Task>(), true));
                _workTimer.OnTrigger += OnWork;
                _workTimer.Toggle();
            }
        }

        protected override void OnWork()
        {
            if (!_workingHumans[0].IsBeingWorked()) return;
            var farmer = _workingHumans[0].Flayers.First();

            if (IsPackagesFull()) return;
            
            _internalBuffer += farmer.GetWorkingRate(EResource.Food);
            if (_internalBuffer >= buildingData.internalBufferCapacity)
            {
                _spriteRenderer.sprite = buildingData.GetSprite(1);
                if (_internalBuffer >= extraBufferToHarvest + buildingData.internalBufferCapacity)
                {
                    AddPacket();
                    _internalBuffer -= buildingData.internalBufferCapacity + extraBufferToHarvest;
                    _spriteRenderer.sprite = buildingData.GetSprite(0);
                }
                
            }
        }
    }
}