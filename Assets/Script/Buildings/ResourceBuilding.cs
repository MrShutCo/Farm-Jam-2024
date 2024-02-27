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
	public class ResourceBuilding : Building
	{
        internal class FlaySubsection
        {
            public HealthBase Flayee { get; set; }
            public List<Human> Flayers;
            public Vector2 FlayeePosition;
            public List<Vector2> FlayerPositions;

            public FlaySubsection(Vector2 flayeePosition, List<Vector2> flayerPositions)
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
        [SerializeField] FloatingStatusBar internalBufferObject;
        [SerializeField] GameObject packages;
        [SerializeField] SpriteRenderer _spriteRenderer;
        
        Stack<GameObject> _packageObjects;
        List<FlaySubsection> _workingHumans;
        Timer _workTimer;
        float _internalBuffer;

        public ResourceBuilding()
		{
            _workingHumans = new();
            _workTimer = new Timer(1, true);
            _packageObjects = new Stack<GameObject>();

            _workingHumans = new List<FlaySubsection>()
            {
                new FlaySubsection(new Vector2(0, 0.5f), new() { new Vector2(-1.25f, 0), new Vector2(1.25f, 0) })
            };
        }

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

        void AddPacket()
        {
            var newPackage = Instantiate(packagePrefab, packages.transform);
            newPackage.transform.localPosition = new Vector3(-1 + NumFinishedPackets * 0.6f, 2.5f);
            newPackage.GetComponent<Package>().SetPackage(buildingData.resource, (int)buildingData.internalBufferCapacity);
           
            _packageObjects.Push(newPackage);
            NumFinishedPackets = Math.Min(NumFinishedPackets + 1, _packageObjects.Count);
            GameManager.Instance.onPackageCreate?.Invoke(this, newPackage.GetComponent<Package>());
        }

        public bool IsPackagesFull() => NumFinishedPackets >= buildingData.GetMaxPackages();

        void OnWork()
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

        void onHumanDie(Human h)
        {
            foreach (var group in _workingHumans)
            {
                if (group.Flayee?.currentHealth <= 0)
                {
                    group.Flayee = null;
                }
            }
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

        bool IsOver(Vector2 position, Vector2 mousePosition)
        {
            var a = new Rect(GetWorldPosition(position) - new Vector2(0.5f, 0.5f), new Vector2(1, 1));
            return a.Contains(mousePosition);
        }

        Vector2 GetWorldPosition(Vector2 localPostion)
        {
            return new Vector2(transform.position.x, transform.position.y) + localPostion;
        }
    }
}

