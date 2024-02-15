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

    float AttackRange = 5;
    float SightRange = 20;
    Vector2 startPos;


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
        startPos = transform.position;
    }
    void OnDisable()
    {
        SwitchTools(false);
    }
    public void InitiateWildBehaviour()
    {
        targetSensor.gameObject.SetActive(true);
        Job job = null;

        if (_target != null)
        {
            switch (npcBehaviour)
            {
                case NPCBehaviour.CivilianRanged:
                    if (Vector3.Distance(transform.position, _target.position) < SightRange)
                    {
                        if (Vector3.Distance(transform.position, _target.position) < AttackRange)
                        {
                            job = new AttackTarget(_target);
                            human.AddJob(job);
                        }
                        else
                        {
                            job = new FleeTarget(_target);
                            human.AddJob(job);
                        }
                    }
                    else
                    {
                        job = new Wander(human);
                        human.AddJob(job);
                    }
                    break;
                case NPCBehaviour.Assault:
                    if (_target == null && human.CurrentJobs.Count == 0)
                    {
                        if (human.CurrentJobs.Count == 0)
                        {
                            job = new Patrol(startPos);
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
                            job = new CloseRangeAssault(_target);
                            human.AddJob(job);
                        }
                    }
                    break;
                case NPCBehaviour.Defensive:
                    if (_target == null && human.CurrentJobs.Count == 0)
                    {
                        if (human.CurrentJobs.Count == 0)
                        {
                            job = new Patrol(startPos);
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
                            job = new DefensiveAttack(_target);
                            human.AddJob(job);
                        }
                    }
                    break;
            }
        }
        else
        {
            job = new Wander(human);
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
            case NPCBehaviour.CivilianRanged:
                job = new FleeTarget(target);
                break;
            case NPCBehaviour.Assault:
                job = new ApproachTarget(target);
                break;
            case NPCBehaviour.Defensive:
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
