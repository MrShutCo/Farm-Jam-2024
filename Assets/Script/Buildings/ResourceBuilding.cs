using System;
using System.Linq;
using System.Collections.Generic;
using Assets.Script.Humans;
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

        const float internalBufferCapacity = 3;

        public EResource HarvestedResouce;
        public float TimeToCollect;
        public int Level;

        float internalBuffer;
        public int NumFinishedPackets { get; private set; }

        [SerializeField] GameObject packagePrefab;
        [SerializeField] FloatingStatusBar internalBufferObject;
        [SerializeField] TextMeshProUGUI capacityText;
        [SerializeField] GameObject packages;
        Queue<GameObject> packageObjects;

        List<FlaySubsection> workingHumans;
        Timer workTimer;

        public ResourceBuilding()
		{
            workingHumans = new();
            workTimer = new Timer(2, true);
            packageObjects = new Queue<GameObject>();

            workingHumans = new List<FlaySubsection>()
            {
                new FlaySubsection(new Vector2(0, 0.5f), new (){ new Vector2(-1.25f, 0), new Vector2(1.25f, 0) })
            };
        }

        public void Start()
        {
            workTimer.OnTrigger += OnWork;
            GameManager.Instance.onHumanDie += onHumanDie;
        }

        public void OnDisable()
        {
            workTimer.OnTrigger -= OnWork;
            GameManager.Instance.onHumanDie -= onHumanDie;
        }

        public void Update()
        {
            workTimer.Update(Time.deltaTime);
            //capacityText.text = $"{CurrHumans}/{MaxCapacity}";
        }

        public void RemovePacket(Human h)
        {
            //packageObjects
            var removed = packageObjects.Dequeue();
            removed.transform.parent = h.transform;
            NumFinishedPackets--;
        }

        void AddPacket()
        {
            var newPackage = Instantiate(packagePrefab);
            newPackage.transform.parent = packages.transform;
            newPackage.transform.position = transform.position + new Vector3(NumFinishedPackets * 0.6f, -0.5f);
            newPackage.GetComponent<Package>().SetPackage(HarvestedResouce, (int)internalBufferCapacity);
           
            packageObjects.Enqueue(newPackage);
            NumFinishedPackets = Math.Min(NumFinishedPackets + 1, packageObjects.Count);
            GameManager.Instance.onPackageCreate?.Invoke(this, newPackage.GetComponent<Package>());
        }

        public bool IsPackagesFull() => NumFinishedPackets >= 4;

        void OnWork()
        {
            float totalResourceGained = 0.0f;
            foreach (var group in workingHumans)
            {
                if (group.Flayee is not null && group.IsBeingWorked() && !IsPackagesFull())
                {
                    totalResourceGained += group.Flayers.Sum(f => f == null ? 0 : f.GetWorkingRate(HarvestedResouce));
                    group.Flayee.TakeDamage(1);
                    Console.WriteLine("Flayed damage taken");
                }
            }

            internalBuffer += totalResourceGained;
            if (internalBuffer >= internalBufferCapacity)
            {
                AddPacket();
                internalBuffer -= internalBufferCapacity;
            }
            internalBufferObject.UpdateStatusBar(internalBuffer, (int)internalBufferCapacity);
        }

        void onHumanDie(Human h)
        {
            foreach (var group in workingHumans)
            {
                if (group.Flayee?.currentHealth <= 0)
                {
                    group.Flayee = null;
                }
            }
        }

        public void AssignHuman(Human human, Vector2 mouseWorldPosition)
        {
            foreach (var subsection in workingHumans)
            {
                // assign to flayee
                if (IsOver(subsection.FlayeePosition, mouseWorldPosition) && subsection.Flayee == null)
                {
                    subsection.Flayee = human.GetComponent<HealthBase>();
                    human.transform.position = GetWorldPosition(subsection.FlayeePosition);
                }
                // assign to flayer
                for (int i = 0; i < subsection.FlayerPositions.Count; i++)
                {
                    if (IsOver(subsection.FlayerPositions[i], mouseWorldPosition) && subsection.Flayers[i] == null)
                    {
                        subsection.Flayers[i] = human;
                        human.transform.position = GetWorldPosition(subsection.FlayerPositions[i]);
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

