using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabSensor : MonoBehaviour
{
    [SerializeField] LayerMask targetLayers;
    List<GrabbableObjectBase> grabbables = new List<GrabbableObjectBase>();
    GrabbableObjectBase closestGrabbable;
    Transform _transform;

    float updateRate = 0.1f;
    float nextUpdate;

    private void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        if (Time.time > nextUpdate)
        {
            nextUpdate = Time.time + updateRate;
            HighlightClosestGrabbable(_transform.position);
        }

    }

    public void HighlightClosestGrabbable(Vector2 position)
    {
        if (closestGrabbable != null)
        {
            // closestGrabbable.Highlight(false);
        }

        closestGrabbable = GetClosestGrabbable(_transform.position);

        if (closestGrabbable != null)
        {
            //  closestGrabbable.Highlight(true);
        }
    }

    public GrabbableObjectBase GetClosestGrabbable(Vector2 position)
    {
        float closestDistance = float.MaxValue;
        GrabbableObjectBase closestGrabbable = null;
        foreach (GrabbableObjectBase grabbable in grabbables)
        {
            float distance = Vector2.Distance(position, grabbable.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestGrabbable = grabbable;
            }
        }
        return closestGrabbable;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GrabbableObjectBase grabbable = other.GetComponent<GrabbableObjectBase>();
        if (grabbable != null)
        {
            grabbables.Add(grabbable);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        GrabbableObjectBase grabbable = other.GetComponent<GrabbableObjectBase>();
        if (grabbable != null)
        {
            grabbables.Remove(grabbable);
        }
    }
}
