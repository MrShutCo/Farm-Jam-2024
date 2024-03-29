using System.Collections;
using System.Collections.Generic;
using Assets.Script.Buildings;
using Assets.Script.Humans;
using UnityEngine;

public class DropPointBuilding : Building
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("DropOff");
            other.gameObject.GetComponent<Carrier>().DropOff();
            HealthBase health = other.gameObject.GetComponent<HealthBase>();
            health.Heal(health.MaxHealth);
        }
    }

    public override void AssignHuman(Human human, Vector2 mouseWorldPosition)
    {
        
    }

    public override bool TryUnassignHuman(Human human)
    {
        return false;
    }
}
