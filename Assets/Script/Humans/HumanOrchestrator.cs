using Assets.Script.Buildings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Script.Humans
{
    public class HumanOrchestrator : MonoBehaviour
    {
        [SerializeField] GameObject HumanObject;

        Dictionary<Human, Job> humans = new();
        List<Human> outsideHumans = new List<Human>();

        public void AssignJobToHuman(Job j, Human h)
        {
            humans[h] = j;
            j.StartJob();
        }

        public void AddTaskToJob(Task t, Human h)
        {
            humans[h].AddTaskToJob(t, true);
        }

        // Start is called before the first frame update
        void Start()
        {
            var humanList = GetComponentsInChildren<Human>();
            foreach (var h in humanList)
            {
                humans[h] = new Job(h, "Wander", new List<Task>() { new Wander(h) }, true);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}