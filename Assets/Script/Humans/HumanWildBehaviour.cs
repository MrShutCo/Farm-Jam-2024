using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;

public class HumanWildBehaviour : MonoBehaviour
{
    public event Action<Task> onTargetFound;
    public enum NPCBehaviour
    {
        Cowardly,
        Aggressive
    }

    TargetSensor targetSensor;
    Human human;
    Transform _target;
    NPCBehaviour npcBehaviour;

    private void Awake()
    {
        human = GetComponent<Human>();
        targetSensor = GetComponentInChildren<TargetSensor>();
        targetSensor.gameObject.SetActive(false);
    }
    void OnEnable()
    {

    }
    void OnDisable()
    {
        DeactivateWildBehaviour();
    }
    private void Start()
    {

    }
    public void InitiateWildBehaviour()
    {
        targetSensor.gameObject.SetActive(true);
        var job = new Wander(human);
        //human.AddJob(job);
    }
    public void DeactivateWildBehaviour()
    {
        targetSensor.gameObject.SetActive(false);
    }
    public void SetTarget(Transform target, bool overrideCurrent = false)
    {
        if (!overrideCurrent && _target != null) return;

        Task job = null;

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
