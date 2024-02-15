using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;

public class TargetSensor : MonoBehaviour
{
    [SerializeField] CircleCollider2D sensorCollider;
    [SerializeField] LayerMask targetLayer;

    HumanWildBehaviour human;

    public void SetSensorRange(float range)
    {
        sensorCollider.radius = range;
    }

    private void Awake()
    {
        human = GetComponentInParent<HumanWildBehaviour>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {
            Debug.Log("TargetSensor: Target found");
            human.SetTarget(other.transform, true);
        }
    }
}
