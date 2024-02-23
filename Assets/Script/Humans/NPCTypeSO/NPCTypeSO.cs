using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;

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

[CreateAssetMenu(fileName = "NPCType", menuName = "NPCType")]
public class NPCTypeSO : ScriptableObject
{

    [SerializeField] string npcName;
    [SerializeField] NPCBehaviour behaviour;

    [SerializeField] float sightRange;
    [SerializeField] float disengageRange;
    [SerializeField] float idealCombatRange;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;

    public string NPCName => npcName;
    public NPCBehaviour Behaviour => behaviour;
    public float SightRange => sightRange;
    public float DisengageRange => disengageRange;
    public float IdealCombatRange => idealCombatRange;
    public float MoveSpeed => moveSpeed;
    public float RotationSpeed => rotationSpeed;
}
