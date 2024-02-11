using Assets.Script.Buildings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Script.Humans
{
    public class HumanOrchestrator : MonoBehaviour
    {
        [SerializeField] GameObject HumanObject;

        List<Human> humans = new List<Human>();
        List<Human> outsideHumans = new List<Human>();

        // Start is called before the first frame update
        void Start()
        {
            //var firstbuilding = GameManager.Instance.Buildings[0];
            humans = GetComponentsInChildren<Human>().ToList();
        }

        // Update is called once per frame
        void Update()
        {
            var currentlySelect = GameManager.Instance.CurrentlySelectedHuman;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                int i = (int)(Random.value * GameManager.Instance.Buildings.Count);
                var b = GameManager.Instance.Buildings[i] as ResourceBuilding;
                currentlySelect.StopCurrentJob();
                currentlySelect.AddJob(new MoveToJob(b.transform.position));
                currentlySelect.AddJob(new WorkJob(b));
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                AssignIdleWorkers();
            }

            if (Input.GetKeyDown(KeyCode.J)) //Test Wander Behaviour
            {
                currentlySelect.StopCurrentJob();
                currentlySelect.ClearCurrentJobs();
                currentlySelect.AddJob(new Wander(currentlySelect));
            }
            if (Input.GetKeyDown(KeyCode.K)) //Test Approach player (as target)
            {
                currentlySelect.StopCurrentJob();
                currentlySelect.ClearCurrentJobs();
                currentlySelect.AddJob(new ApproachTarget(GameManager.Instance.Player.position));
            }
        }

        void AssignIdleWorkers()
        {
            List<Human> idleHumans = GetIdleHumans().ToList();
            List<Building> nonFullBuildings = GameManager.Instance.Buildings.Where(b => b.CurrHumans < b.MaxCapacity).ToList();
            List<int> capacities = nonFullBuildings.Select(b => b.CurrHumans).ToList();

            // Whileloop was empty. Commented out. I was getting some crashes
            //while (idleHumans.Count > 0 && nonFullBuildings.Count() > 0)
            //{
            /*var nextBuilding = nonFullBuildings.First();
            var bestHumanForJob = GetHighestSkilledHuman(idleHumans, nextBuilding.HarvestedResouce);
            bestHumanForJob.AddJob(new MoveToJob(nextBuilding.transform.position));
            bestHumanForJob.AddJob(new WorkJob(nextBuilding));
            idleHumans.Remove(bestHumanForJob);
            capacities[0]++;
            if (nonFullBuildings[0].MaxCapacity == capacities[0])
            {
                nonFullBuildings.RemoveAt(0);
                capacities.RemoveAt(0);
            }*/
            //}
        }

        IEnumerable<Human> GetIdleHumans() => humans.Where(h => h.IsIdle());
        Human GetHighestSkilledHuman(IEnumerable<Human> possibleHumans, EResource resource)
        {
            var bestHuman = possibleHumans.First();
            foreach (var human in possibleHumans)
                bestHuman = human.Skills[resource] > bestHuman.Skills[resource] ? human : bestHuman;
            return bestHuman;
        }

    }
}