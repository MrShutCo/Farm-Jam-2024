using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Humans;

namespace Assets.Script.Buildings
{
	public class HaulerBuilding : Building
	{
		List<(ResourceBuilding building, Package p)> packagesToBePickedUp;
		List<Human> haulers;
		Timer checkPickupsTimer;
		[SerializeField] private Animator _chestAnimator;

		[SerializeField] GameObject TipToDestroy;
		BoxCollider2D _boundingBox;

		// Use this for initialization
		void Start()
		{
			haulers = new();
			packagesToBePickedUp = new();
			_boundingBox = GetComponent<BoxCollider2D>();

			// Instantly try to assign a package
			GameManager.Instance.onPackageCreate += (Building b, Package p) =>
			{
				if (b is ResourceBuilding building && !TryPickUp(building, p))
					packagesToBePickedUp.Add((building, p));
			};

			// Try to assign someone to all packages once a second
			checkPickupsTimer = new Timer(1, true);
			checkPickupsTimer.OnTrigger += () =>
			{
				packagesToBePickedUp.RemoveAll(p => TryPickUp(p.building, p.p));
			};
		}

		bool TryPickUp(ResourceBuilding b, Package p)
		{
			var closestFreeHauler = haulers.
				Where(h => h != null && !h.CurrentJobs.Any() || h.CurrentJobs.Peek().Name != "Haul").
				OrderBy(h => Vector3.Distance(h.transform.position, b.PickupLocation.position)).
				FirstOrDefault();

			if (closestFreeHauler == null)
				return false;

			var job = new Job(closestFreeHauler, "Haul", new List<Task>()
			{
				new MoveToTask(b.PickupLocation.position),
				new InstantTask("Pickup", () => { closestFreeHauler.HoldPackage(p); b.RemovePacket(closestFreeHauler); }),
				new MoveToTask(transform.position),
				new InstantTask("Dropoff", () => {
					_chestAnimator.SetTrigger("ChestTrigger"); 
					closestFreeHauler.DropoffPackage();
					if (packagesToBePickedUp.Count == 0)
						closestFreeHauler.AddTaskToJob(new MoveToTask(GetRandomPosition()), false);

				})
				
			}, false);
			closestFreeHauler.AddJob(job);
			return true;
		}

		// Update is called once per frame
		void Update()
		{
			checkPickupsTimer.Update(Time.deltaTime);
		}

        Vector3 GetRandomPosition()
        {
            var b = _boundingBox.bounds;
            var (x, y) = (Random.Range(b.min.x, b.max.x), Random.Range(b.min.y, b.max.y));
            return new Vector3(x, y, 0);
        }

        public override void AssignHuman(Human human, Vector2 mouseWorldPosition)
		{
			if (!haulers.Contains(human))
			{
				human.StopAllJobs();
				haulers.Add(human);
			}
			if (TipToDestroy != null && haulers.Count > 0)
				Destroy(TipToDestroy);
		}

		public override bool TryUnassignHuman(Human human)
		{
			for (int i = 0; i < haulers.Count(); i++)
			{
				if (haulers[i] == human)
				{
					haulers.RemoveAt(i);
					return true;
				}
			}

			return false;
		}
	}
}
