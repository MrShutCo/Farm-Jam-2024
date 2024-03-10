using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Humans;
using Script.Buildings;

namespace Assets.Script.Buildings
{
	public class HaulerBuilding : CollectionBuilding
	{
		List<(ResourceBuilding building, Package p)> packagesToBePickedUp;
		[SerializeField] private Animator _chestAnimator;

		[SerializeField] GameObject TipToDestroy;
		BoxCollider2D _boundingBox;

		// Use this for initialization
		void Start() {
			packagesToBePickedUp = new();
			_boundingBox = GetComponent<BoxCollider2D>();
			jobName = "Haul";
			// Instantly try to assign a package
			GameManager.Instance.onPackageCreate += (Building b, Package p) =>
			{
				if (b is ResourceBuilding building && !TryPickUp(building, p))
					packagesToBePickedUp.Add((building, p));
			};
			actionTimer.OnTrigger += () =>
			{
				packagesToBePickedUp.RemoveAll(p => TryPickUp(p.building, p.p));
			};
		}

		bool TryPickUp(ResourceBuilding b, Package p)
		{
			var closestFreeHauler = GetNearestFreeHuman(b.PickupLocation.position);

			if (closestFreeHauler == null)
				return false;

			var job = new Job(closestFreeHauler, jobName, new List<Task>()
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

        Vector3 GetRandomPosition()
        {
            var b = _boundingBox.bounds;
            var (x, y) = (Random.Range(b.min.x, b.max.x), Random.Range(b.min.y, b.max.y));
            return new Vector3(x, y, 0);
        }

        public override void AssignHuman(Human human, Vector2 mouseWorldPosition)
		{
			base.AssignHuman(human, mouseWorldPosition);
			if (TipToDestroy != null && workingHumans.Count > 0)
				Destroy(TipToDestroy);
		}
	}
}
