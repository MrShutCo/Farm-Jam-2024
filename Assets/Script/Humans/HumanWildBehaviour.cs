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
    private Timer refreshTimer;

    private void Awake()
    {
        human = GetComponent<Human>();
        targetSensor = GetComponentInChildren<TargetSensor>();
        targetSensor.SetSensorRange(npcType.SightRange);
        targetSensor.gameObject.SetActive(false);
        refreshTimer = new Timer(0.25f, true);
        refreshTimer.OnTrigger += () =>
        {
            if (_target != null)
            {
                if (human.CurrentJobs.Count == 0)
                    human.AddTaskToJob(CheckForChange(), true);
            }
        };
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
        if (human.WeaponSelector.ActiveWeapon != null)
            human.WeaponSelector.ActivateWeeapon(true);
    }
    void OnDisable()
    {
        ClearTarget();
        human.WeaponSelector.ActivateWeeapon(false);
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
        if (_target != null)
            refreshTimer.Update(Time.deltaTime);
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
        //human.StopCurrentJob();
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
