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

        Dictionary<Human, Queue<Job>> humans = new();
        List<Human> outsideHumans = new List<Human>();

        public void AssignJobToHuman(Job j, Human h, bool clearCurrentJob)
        {
            if (clearCurrentJob)
            {
                humans[h].Peek().StopJob();
                humans[h].Dequeue();
            }
            humans[h].Enqueue(j);
            if (humans[h].Count == 1)
            {
                j.StartJob();
                j.onJobComplete += onJobComplete;
            }
        }

        public void AddTaskToCurrentJob(Task t, Human h)
        {
            humans[h].Peek().AddTaskToJob(t, false);
        }

        // Start is called before the first frame update
        void Start()
        {
            var humanList = GetComponentsInChildren<Human>();
            foreach (var h in humanList)
            {
                var queue = new Queue<Job>();
                queue.Enqueue(new Job(h, "Wander", new List<Task>() { new Wander(h) }, true));
                humans[h] = queue;
            }
        }

        // Update is called once per frame
        void Update()
        { 
        }

        void onJobComplete(Human h)
        {
            // Repeat a job only if its repeatable AND we dont have another job queued up after.
            if (!humans[h].Peek().IsRepeated || humans[h].Peek().IsRepeated && humans[h].Count > 1)
            {
                humans[h].Peek().onJobComplete -= onJobComplete;
                humans[h].Dequeue();
                humans[h].Peek().onJobComplete += onJobComplete;
                humans[h].Peek().StartJob();
            }
        }
    }
}