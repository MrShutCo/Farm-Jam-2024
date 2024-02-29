using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;

public class HumanHealth : HealthBase
{
    Human human;
    Collider2D col;

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
        human.WildBehaviour.enabled = false;
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
