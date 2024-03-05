using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;

public class HumanHealth : HealthBase
{
    Human human;
    Collider2D col;
    [SerializeField] PickUpItem pickUpItemPrefab;

    protected override void Awake()
    {
        base.Awake();
        human = GetComponent<Human>();
        col = GetComponent<Collider2D>();
    }


    protected override void Die()
    {
        human.StopAllJobs();
        human.enabled = false;
        if (human.WildBehaviour.enabled)
        {
            if (pickUpItemPrefab != null)
            {
                Transform pickup = Instantiate(pickUpItemPrefab, transform.position, Quaternion.identity).transform;
                pickup.SetParent(_transform.parent);
            }
            human.WildBehaviour.enabled = false;
        }
        col.enabled = false;
        Debug.Log("Human died");
        base.Die();
        Invoke("DisableGameObject", 1f);//change length of time to match death animation
    }
    protected override void DisableGameObject()
    {
        base.DisableGameObject();
    }
}
