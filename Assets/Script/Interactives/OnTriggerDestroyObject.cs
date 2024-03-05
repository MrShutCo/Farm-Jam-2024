using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerDestroyObject : MonoBehaviour
{
    [SerializeField] GameObject objectToDestroy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (objectToDestroy != null)
                Destroy(objectToDestroy);
        }
    }
}
