using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;

public class HumanWildBehaviour : MonoBehaviour
{
    public event Action<Job> onTargetFound;
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

    public void InitiateWildBehaviour()
    {
        var job = new Wander(human);
        human.AddJob(job);
    }
    public void SetTarget(Transform target, bool overrideCurrent = false)
    {
        if (!overrideCurrent && _target != null) return;

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
        onTargetFound?.Invoke(job);
        _target = target;
    }
}
