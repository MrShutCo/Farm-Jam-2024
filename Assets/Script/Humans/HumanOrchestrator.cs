using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            var firstbuilding = GameManager.Instance.Buildings[0];
            humans = GetComponentsInChildren<Human>().ToList();
            humans[0].AddJob(new MoveToJob(firstbuilding.transform.position));
            humans[0].AddJob(new WorkJob(firstbuilding));
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                int i = (int)(Random.value * GameManager.Instance.Buildings.Count);
                var b = GameManager.Instance.Buildings[i];
                humans[0].StopCurrentJob();
                humans[0].AddJob(new MoveToJob(b.transform.position));
                humans[0].AddJob(new WorkJob(b));
            }
        }
    }
}