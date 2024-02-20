using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OnTriggerDealDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryGetComponent(out HealthBase health);
        if (health != null)
        {
            health.TakeDamage(1);
        }
    }
}
