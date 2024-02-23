using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;


public class HumanWildBehaviour : MonoBehaviour
{
    public NPCTypeSO npcType;

    [SerializeField] Transform selectionCollider;
    [SerializeField] Transform weaponParent;
    TargetSensor targetSensor;
    
    Human human;
    Transform _target;
    Vector2 startPos;
    float refreshInterval = .25f;
    float refreshTimer = 0;

    private void Awake()
    {
        human = GetComponent<Human>();
        targetSensor = GetComponentInChildren<TargetSensor>();
        targetSensor.SetSensorRange(npcType.SightRange);
        targetSensor.gameObject.SetActive(false);
        
    }
    void OnEnable()
    {
        SwitchTools(true);
        startPos = transform.position;
        var newJob = new Job(human, npcType.ToString(), new List<Task>(), true);
        if (_target != null)
        {
            newJob.AddTaskToJob(CheckForChange(), true);
        }
        else
        {
            newJob.AddTaskToJob(ChangeToIdle(), true);
        }
        human.StopAllJobs();
        human.AddJob(newJob);
    }
    void OnDisable()
    {
        ClearTarget();
        SwitchTools(false);
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
                human.AddTaskToJob(CheckForChange(), true);
            }
        }
    }
    public Task SetTarget(Transform target, bool overrideCurrent = false)
    {
        if (!overrideCurrent && _target != null) return new Idle();
        _target = target;
        return ChangeToTargetMode();
    }

    bool IsOutsideRange(float range)
    {
        return Vector2.Distance(transform.position, _target.position) > range;
    }

    Task ClearTarget()
    {
        _target = null;
        return ChangeToIdle();
    }

    Task CheckForChange() => IsOutsideRange(npcType.DisengageRange) ? ClearTarget() : ChangeToTargetMode();
    

    Task ChangeToIdle()
    {
        human.StopCurrentJob();
        return npcType.Behaviour switch
        {
            NPCBehaviour.CivilianRanged => new Wander(human),
            NPCBehaviour.CivilianMelee => new Wander(human),
            NPCBehaviour.Defensive => new DefensiveIdle(),
            NPCBehaviour.Assault => new Patrol(startPos),
            _ => new Idle()
        };
    }
    Task ChangeToTargetMode()
    {
        return npcType.Behaviour switch
        {
            NPCBehaviour.Defensive => new FleeAndFire(_target),
            NPCBehaviour.Assault => new CloseRangeAssault(_target),
            NPCBehaviour.CivilianMelee => new ApproachAndAttack(_target),
            NPCBehaviour.CivilianRanged => new FleeAndFire(_target)
        };
    }
}
