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
        Aggressive,
        Cautious
    }

    [SerializeField] Transform selectionCollider;
    [SerializeField] Transform weaponParent;
    TargetSensor targetSensor;

    Human human;
    Transform _target;
    [SerializeField] NPCBehaviour npcBehaviour = NPCBehaviour.Aggressive;

    float AttackRange = 5;
    float SightRange = 20;


    /// <summary>
    /// Cautious:
    /// If distance is < 10, flee
    /// If distance is < attack range, attack
    /// if distance is > sight range, hold ground
    /// 
    ///
    /// Agressive:
    /// If distance is < attack range attack
    /// If distance is less than sight range, approach
    /// If distance is greater than sight range, wander
    ///
    ///
    /// Cowardly:
    /// If distance is < sight range, flee
    /// If distance is > sight range, wander
    /// if distance is < attack range, attack
    ///
    ///
    ///
    /// </summary>

    private void Awake()
    {
        human = GetComponent<Human>();
        targetSensor = GetComponentInChildren<TargetSensor>();
        targetSensor.gameObject.SetActive(false);
    }
    private void Start()
    {
    }
    void OnEnable()
    {
        SwitchTools(true);
    }
    void OnDisable()
    {
        SwitchTools(false);
    }
    public void InitiateWildBehaviour()
    {
        targetSensor.gameObject.SetActive(true);

        if (_target != null)
        {
            switch (npcBehaviour)
            {
                case NPCBehaviour.Cowardly:
                    if (Vector3.Distance(transform.position, _target.position) < SightRange)
                    {
                        if (Vector3.Distance(transform.position, _target.position) < AttackRange)
                        {
                            var job = new AttackTarget(_target);
                            human.AddJob(job);
                        }
                        else
                        {
                            var job = new FleeTarget(_target);
                            human.AddJob(job);
                        }
                    }
                    else
                    {
                        var job = new Wander(human);
                        human.AddJob(job);
                    }
                    break;
                case NPCBehaviour.Aggressive:
                    if (Vector3.Distance(transform.position, _target.position) < AttackRange)
                    {
                        var job = new AttackTarget(_target);
                        human.AddJob(job);
                    }
                    else if (Vector3.Distance(transform.position, _target.position) < SightRange)
                    {
                        var job = new ApproachTarget(_target);
                        human.AddJob(job);
                    }
                    else
                    {
                        var job = new Wander(human);
                        human.AddJob(job);
                    }
                    break;
                case NPCBehaviour.Cautious:

                    if (Vector3.Distance(transform.position, _target.position) < AttackRange)
                    {
                        var job = new AttackTarget(_target);
                        human.AddJob(job);
                    }
                    else if (Vector3.Distance(transform.position, _target.position) < 10)
                    {
                        var job = new FleeTarget(_target);
                        human.AddJob(job);
                    }
                    else if (Vector3.Distance(transform.position, _target.position) > SightRange)
                    {
                        var job = new Wander(human);
                        human.AddJob(job);
                    }
                    break;
            }
        }
        else
        {
            var job = new Wander(human);
            human.AddJob(job);

        }
    }

    void SwitchTools(bool active)
    {
        selectionCollider.gameObject.SetActive(!active);
        weaponParent.gameObject.SetActive(active);
        targetSensor.gameObject.SetActive(active);
    }
    private void Update()
    {
        if (human.CurrentJobs.Count == 0)
        {
            InitiateWildBehaviour();
        }
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
            case NPCBehaviour.Cautious:
                job = new AttackTarget(target);
                break;
        }
        onTargetFound?.Invoke(job);
        _target = target;
    }

    void ClearTarget()
    {
        _target = null;
    }
}
