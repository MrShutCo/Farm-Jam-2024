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
    Job _job;
    Transform _target;
    [SerializeField] NPCBehaviour npcBehaviour = NPCBehaviour.CivilianMelee;
    Vector2 startPos;
    float refreshInterval = .25f;
    float refreshTimer = 0;

    private void Awake()
    {
        human = GetComponent<Human>();
        targetSensor = GetComponentInChildren<TargetSensor>();
        targetSensor.SetSensorRange(npcType.SightRange);
        targetSensor.gameObject.SetActive(false);
        _job = new Job(human, npcType.ToString(), new List<Task>(), true);
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
        if (_target != null)
        {
            CheckForChange();
        }
        else
        {
            ChangeToIdle();
        }
        _job.StartJob();
    }

    void SwitchTools(bool active)
    {
        selectionCollider.gameObject.SetActive(!active);
        weaponParent.gameObject.SetActive(active);
        targetSensor.gameObject.SetActive(active);
    }
    private void Update()
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer > refreshInterval)
        {
            refreshTimer = 0;
            if (_target != null)
            {
                CheckForChange();
            }
        }
    }
    public void SetTarget(Transform target, bool overrideCurrent = false)
    {
        if (!overrideCurrent && _target != null) return;
        _target = target;
        ChangeToTargetMode();
    }

    bool IsOutsideRange(float range)
    {
        return Vector2.Distance(transform.position, _target.position) > range;
    }
    bool IsInsideSightRange(float range)
    {
        return Vector2.Distance(transform.position, _target.position) < range;
    }

    void ClearTarget()
    {
        _target = null;
        ChangeToIdle();
    }

    void CheckForChange()
    {
        if (_target != null)
        {
            if (IsOutsideRange(npcType.DisengageRange)) ClearTarget();
            else ChangeToTargetMode();
        }
    }

    void ChangeToIdle()
    {
        switch (npcBehaviour)
        {
            case NPCBehaviour.CivilianRanged:
                _job.AddTaskToJob(new Wander(human), true);
                break;
            case NPCBehaviour.CivilianMelee:
                _job.AddTaskToJob(new Wander(human), true);
                break;
            case NPCBehaviour.Defensive:
                _job.AddTaskToJob(new DefensiveIdle(), true);
                break;
            case NPCBehaviour.Assault:
                _job.AddTaskToJob(new Patrol(startPos), true);
                break;
        }
    }
    void ChangeToTargetMode()
    {
        switch (npcBehaviour)
        {
            case NPCBehaviour.CivilianRanged:
                _job.AddTaskToJob(new FleeAndFire(_target), true);
                break;
            case NPCBehaviour.CivilianMelee:
                _job.AddTaskToJob(new ApproachAndAttack(_target), true);
                break;
            case NPCBehaviour.Assault:
                _job.AddTaskToJob(new CloseRangeAssault(_target), true);
                break;
            case NPCBehaviour.Defensive:
                _job.AddTaskToJob(new DefensiveAttack(_target), true);
                break;
        }
    }
}
