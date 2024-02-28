using System;
using System.Linq;
using System.Collections.Generic;
using Assets.Script.Humans;
using Script.Buildings;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Script.Buildings
{
	public abstract class ResourceBuilding : Building
	{
        protected class WorkingSubsection
        {
            public HealthBase Flayee { get; set; }
            public List<Human> Flayers;
            public Vector2 FlayeePosition;
            public List<Vector2> FlayerPositions;

            public WorkingSubsection(Vector2 flayeePosition, List<Vector2> flayerPositions)
            {
                FlayeePosition = flayeePosition;
                FlayerPositions = flayerPositions;
                Flayers = new List<Human>(new Human[flayerPositions.Count]);
            }

            public bool IsBeingWorked() => Flayers.Any(f => f != null);
        }

        public ResourceBuildingDataSO buildingData; 
        public int NumFinishedPackets { get; private set; }

        [SerializeField] GameObject packagePrefab;
        [SerializeField] protected FloatingStatusBar internalBufferObject;
        [SerializeField] GameObject packages;
        [SerializeField] protected SpriteRenderer _spriteRenderer;
        
        protected Stack<GameObject> _packageObjects = new();
        protected List<WorkingSubsection> _workingHumans = new();
        protected Timer _workTimer;
        protected float _internalBuffer;

        public void Start()
        {
            _workTimer.OnTrigger += OnWork;
            GameManager.Instance.onHumanDie += onHumanDie;
            _spriteRenderer.sprite = buildingData.GetSprite();
        }

        public void OnDisable()
        {
            _workTimer.OnTrigger -= OnWork;
            GameManager.Instance.onHumanDie -= onHumanDie;
        }

        public void Update()
        {
            _workTimer.Update(Time.deltaTime);
        }

        public void RemovePacket(Human h)
        {
            _packageObjects.Pop();
            NumFinishedPackets--;
        }

        protected void AddPacket()
        {
            var newPackage = Instantiate(packagePrefab, packages.transform);
            newPackage.transform.localPosition = new Vector3(-1 + NumFinishedPackets * 0.6f, 2.5f);
            newPackage.GetComponent<Package>().SetPackage(buildingData.resource, (int)buildingData.internalBufferCapacity);
           
            _packageObjects.Push(newPackage);
            NumFinishedPackets = Math.Min(NumFinishedPackets + 1, _packageObjects.Count);
            GameManager.Instance.onPackageCreate?.Invoke(this, newPackage.GetComponent<Package>());
        }

        public bool IsPackagesFull() => NumFinishedPackets >= buildingData.GetMaxPackages();

        protected abstract void OnWork();

        protected void onHumanDie(Human h)
        {
            foreach (var group in _workingHumans)
            {
                if (group.Flayee?.currentHealth <= 0)
                {
                    group.Flayee = null;
                }
            }
        }

       

        protected bool IsOver(Vector2 position, Vector2 mousePosition)
        {
            var a = new Rect(GetWorldPosition(position) - new Vector2(0.5f, 0.5f), new Vector2(1, 1));
            return a.Contains(mousePosition);
        }

        protected Vector2 GetWorldPosition(Vector2 localPostion)
        {
            return new Vector2(transform.position.x, transform.position.y) + localPostion;
        }
    }
}

