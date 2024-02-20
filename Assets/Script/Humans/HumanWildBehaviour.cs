using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEditor;
using UnityEngine;


public class HumanWildBehaviour : MonoBehaviour
{
    public event Action<Task> onTargetFound;
    public enum NPCBehaviour
    {
        CivilianMelee,
        CivilianRanged,
        Defensive,
        Assault,
        Tank1,
        Tank2,
        Boss1
    }

    public NPCTypeSO npcType;

    [SerializeField] Transform selectionCollider;
    [SerializeField] Transform weaponParent;
    TargetSensor targetSensor;

    Human human;
    Transform _target;
    [SerializeField] NPCBehaviour npcBehaviour = NPCBehaviour.CivilianMelee;
    Vector2 startPos;

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
        startPos = transform.position;
    }
    void OnDisable()
    {
        SwitchTools(false);
    }
    public void InitiateWildBehaviour()
    {
        targetSensor.gameObject.SetActive(true);
        Job job = new Job(human);
        

        if (_target != null)
        {
            switch (npcBehaviour)
            {
                case NPCBehaviour.CivilianRanged:
                    if (_target == null && human.CurrentJobs.Count == 0)
                    {
                        if (human.CurrentJobs.Count == 0)
                        {
                            job.AddTaskToJob(new Wander(human), true);
                            human.AddJob(job);
                        }
                        else return;
                    }

                    if (Vector2.Distance(transform.position, _target.position) > npcType.DisengageRange)
                    {
                        ClearTarget();
                        return;
                    }
                    else
                    {
                        job.AddTaskToJob(new FleeAndFire(_target), true);
                        human.AddJob(job);
                    }
                    break;
                case NPCBehaviour.CivilianMelee:

                    if (_target == null && human.CurrentJobs.Count == 0)
                    {
                        if (human.CurrentJobs.Count == 0)
                        {
                            job.AddTaskToJob(new Wander(human), true);
                            human.AddJob(job);
                        }
                        else return;
                    }

                    if (Vector2.Distance(transform.position, _target.position) > npcType.DisengageRange)
                    {
                        ClearTarget();
                        return;
                    }
                    else
                    {
                        job.AddTaskToJob(new ApproachAndAttack(_target), true);
                        human.AddJob(job);
                    }


                    break;

                case NPCBehaviour.Assault:
                    if (_target == null && human.CurrentJobs.Count == 0)
                    {
                        if (human.CurrentJobs.Count == 0)
                        {
                            job.AddTaskToJob(new Patrol(startPos), true);
                            human.AddJob(job);
                        }
                        else return;
                    }
                    else
                    {
                        if (Vector2.Distance(transform.position, _target.position) > npcType.DisengageRange)
                        {
                            ClearTarget();
                            return;
                        }
                        if (Vector3.Distance(transform.position, _target.position) < npcType.SightRange)
                        {
                            job.AddTaskToJob(new CloseRangeAssault(_target), true);
                            human.AddJob(job);
                        }
                    }
                    break;
                case NPCBehaviour.Defensive:
                    if (_target == null && human.CurrentJobs.Count == 0)
                    {
                        if (human.CurrentJobs.Count == 0)
                        {
                            job.AddTaskToJob(new DefensiveIdle(), true);
                            human.AddJob(job);
                        }
                        else return;
                    }
                    else
                    {
                        if (Vector2.Distance(transform.position, _target.position) > npcType.DisengageRange)
                        {
                            ClearTarget();
                            return;
                        }
                        if (Vector3.Distance(transform.position, _target.position) < npcType.SightRange)
                        {
                            job.AddTaskToJob(new DefensiveAttack(_target), true);
                            human.AddJob(job);
                        }
                    }
                    break;
            }
        }
        else
        {
            if (human.CurrentJobs.Count == 0)
            {
                switch (npcBehaviour)
                {
                    case NPCBehaviour.CivilianRanged:
                        job.AddTaskToJob(new Wander(human), true);
                        human.AddJob(job);
                        break;
                    case NPCBehaviour.CivilianMelee:
                        job.AddTaskToJob(new Wander(human), true);
                        human.AddJob(job);
                        break;
                    case NPCBehaviour.Defensive:
                        job.AddTaskToJob(new DefensiveIdle(), true);
                        human.AddJob(job);
                        break;
                    case NPCBehaviour.Assault:
                        job.AddTaskToJob(new Patrol(startPos), true);
                        human.AddJob(job);
                        break;
                }
            }
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
        return;
        if (human.CurrentJobs.Count == 0)
        {
            InitiateWildBehaviour();
        }
    }
    public void SetTarget(Transform target, bool overrideCurrent = false)
    {
        if (!overrideCurrent && _target != null) return;

        Task job = null;

        switch (npcBehaviour)
        {
            case NPCBehaviour.CivilianRanged:
                job = new FleeAndFire(target);
                break;
            case NPCBehaviour.Assault:
                job = new CloseRangeAssault(target);
                break;
            case NPCBehaviour.Defensive:
                job = new DefensiveAttack(target);
                break;
            case NPCBehaviour.CivilianMelee:
                job = new ApproachAndAttack(target);
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
