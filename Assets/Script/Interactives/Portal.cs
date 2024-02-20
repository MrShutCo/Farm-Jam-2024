using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public event System.Action onPortalComplete;
    enum PortalDestinationType
    {
        Specify,
        HomeA,
        HomeB,
        HomeC,
    }
    [SerializeField] PortalDestinationType destinationType;
    [SerializeField] Portal destination;
    [SerializeField] bool isEntrance;
    bool deactivate = false;

    private void OnEnable()
    {
        if (destinationType == PortalDestinationType.HomeA)
        {
            SetDestination(GameObject.Find("Portal-Home-A").GetComponent<Portal>());
        }
        else if (destinationType == PortalDestinationType.HomeB)
        {
            SetDestination(GameObject.Find("Portal-Home-B").GetComponent<Portal>());
        }
        else if (destinationType == PortalDestinationType.HomeC)
        {
            SetDestination(GameObject.Find("Portal-Home-C").GetComponent<Portal>());
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (deactivate) return;
        if (isEntrance)
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<Rigidbody2D>();
                GameManager.Instance.onTeleport?.Invoke(true, destination.transform.position);
                player.simulated = false;
                player.transform.position = destination.transform.position;
                player.simulated = true;
                GameManager.Instance.onTeleport?.Invoke(false, destination.transform.position);
                onPortalComplete?.Invoke();
            }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (isEntrance)
            if (other.CompareTag("Player"))
            {
                deactivate = false;
            }
    }

    public void SetDestination(Portal portal)
    {
        destination = portal;
        portal.Deactivate();
    }
    public void Deactivate()
    {
        deactivate = true;
    }

}
