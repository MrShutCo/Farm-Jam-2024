using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;

public class HumanWildBehaviour : MonoBehaviour
{
    public enum NPCBehaviour
    {
        Cowardly,
        Aggressive
    }

    Human human;
    Transform _target;
    NPCBehaviour npcBehaviour;

    private void Awake()
    {
        human = GetComponent<Human>();
    }
    private void Start()
    {
        InitiateWildHuman();
    }

    void InitiateWildHuman()
    {
        var job = new Wander(human);
        human.AddJob(job);
    }
    public void SetTarget(Transform target, bool overrideCurrent = false)
    {
        if (!overrideCurrent && _target != null) return;

        Debug.Log("NewTarget Set");

        human.ClearCurrentJobs();

        Job job = null;

        switch (npcBehaviour)
        {
            case NPCBehaviour.Cowardly:
                job = new FleeTarget(target);
                break;
            case NPCBehaviour.Aggressive:
                job = new ApproachTarget(target);
                break;
        }
        human.AddJob(job);
        _target = target;
    }
}
