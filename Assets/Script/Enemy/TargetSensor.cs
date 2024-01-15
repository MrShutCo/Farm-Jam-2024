using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSensor : MonoBehaviour
{
    public event System.Action<Transform> OnTargetInSensorRange;
    public event System.Action<Transform> OnTargetOutOfSensorRange;
    [SerializeField] LayerMask targetLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {
            Debug.Log("Target detected");
            OnTargetInSensorRange?.Invoke(other.transform);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {
            Debug.Log("Target lost");
            OnTargetOutOfSensorRange?.Invoke(other.transform);
        }
    }
}
