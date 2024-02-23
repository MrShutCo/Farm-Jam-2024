using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Buildings;
using Assets.Script.Humans;

public class HaulerManager : MonoBehaviour
{

	List<(Building building, Package p)> packagesToBePickedUp;
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
			if (!TryPickUp(b, p))
				packagesToBePickedUp.Add((b, p));
		};

		// Try to assign someone to all packages once a second
		checkPickupsTimer = new Timer(1, true);
		checkPickupsTimer.OnTrigger += () => {
			packagesToBePickedUp.RemoveAll(p => TryPickUp(p.building, p.p));
		};
	}

	bool TryPickUp(Building b, Package p)
	{
		var closestFreeHauler = haulers.
			Where(h => h.CurrentJobs.Count() == 0 || h.CurrentJobs.Peek().Name != "Haul").
			OrderBy(h => Vector3.Distance(h.transform.position, b.transform.position)).
			FirstOrDefault();

		if (closestFreeHauler == null)
            return false;

		var job = new Job(closestFreeHauler, "Haul", new List<Task>()
			{
				new MoveToTask(b.transform.position),
				new InstantTask("Pickup", () => { closestFreeHauler.HoldPackage(p); }),
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

