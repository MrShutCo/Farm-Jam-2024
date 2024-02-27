using Assets.Script.Buildings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Script.Humans
{
    public class HumanOrchestrator : MonoBehaviour
    {
        List<Human> humans = new();

        // Start is called before the first frame update
        void Start()
        {
            var humanList = GetComponentsInChildren<Human>();
            foreach (var h in humanList)
            {
                var v = new Job(h, "Wander", new List<Task>() { new Wander(h) }, true);
                
                
            }
        }
    }
}