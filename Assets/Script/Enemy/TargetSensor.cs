using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;

public class TargetSensor : MonoBehaviour
{
    [SerializeField] CircleCollider2D sensorCollider;
    [SerializeField] LayerMask targetLayer;

    HumanWildBehaviour humanBehaviour;
    private Human human;

    public void SetSensorRange(float range)
    {
        sensorCollider.radius = range;
    }

    private void Awake()
    {
        humanBehaviour = GetComponentInParent<HumanWildBehaviour>();
        human = GetComponentInParent<Human>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {
            human.AddTaskToJob(humanBehaviour.SetTarget(other.transform, true), true);
        }
    }
}
