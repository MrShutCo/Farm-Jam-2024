using System.Collections.Generic;
using System.Linq;
using Assets.Script.Buildings;
using Assets.Script.Humans;
using UnityEngine;

namespace Script.Humans
{
	public class HaulerManager : MonoBehaviour
	{

		List<(ResourceBuilding building, Package p)> packagesToBePickedUp;
		List<Human> haulers;
		Timer checkPickupsTimer;

		// Use this for initialization
		void Start()
		{
			haulers = new();
			packagesToBePickedUp = new();

			// Instantly try to assign a package
			GameManager.Instance.onPackageCreate += (Building b, Package p) =>
			{
				if (b is ResourceBuilding building && !TryPickUp(building, p))
					packagesToBePickedUp.Add((building, p));
			};

			// Try to assign someone to all packages once a second
			checkPickupsTimer = new Timer(1, true);
			checkPickupsTimer.OnTrigger += () => {
				packagesToBePickedUp.RemoveAll(p => TryPickUp(p.building, p.p));
			};
		}

		bool TryPickUp(ResourceBuilding b, Package p)
		{
			var closestFreeHauler = haulers.
				Where(h => h.CurrentJobs.Count() == 0 || h.CurrentJobs.Peek().Name != "Haul").
				OrderBy(h => Vector3.Distance(h.transform.position, b.PickupLocation.position)).
				FirstOrDefault();

			if (closestFreeHauler == null)
				return false;

			var job = new Job(closestFreeHauler, "Haul", new List<Task>()
			{
				new MoveToTask(b.PickupLocation.position),
				new InstantTask("Pickup", () => { closestFreeHauler.HoldPackage(p); b.RemovePacket(closestFreeHauler); }),
				new MoveToTask(new Vector3(0, -4)),
				new InstantTask("Dropoff", () => { closestFreeHauler.DropoffPackage(); })
			}, false);
			closestFreeHauler.AddJob(job);
			return true;
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.I))
			{
				haulers.Add(GameManager.Instance.CurrentlySelectedHuman);
			}
			checkPickupsTimer.Update(Time.deltaTime);
		}
	}
}

