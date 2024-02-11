using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] Portal destination;
    [SerializeField] bool isEntrance;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isEntrance)
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<Rigidbody2D>();
                GameManager.Instance.onTeleport?.Invoke(true);
                player.simulated = false;
                player.transform.position = destination.transform.position;
                player.simulated = true;
                GameManager.Instance.onTeleport?.Invoke(false);
            }
    }

    public void SetDestination(Portal portal)
    {
        destination = portal;
    }

}
