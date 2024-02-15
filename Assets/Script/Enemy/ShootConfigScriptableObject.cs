using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShootConfig", menuName = "Weapons/ShootConfig", order = 2)]
public class ShootConfigScriptableObject : ScriptableObject
{
    public LayerMask HitMask;
    public Vector2 Spread = new Vector2(0.1f, 0.1f);
    public float FireRate = 0.25f;
    public int Damage = 1;
    public int BulletsPerShot = 1;

    public float KnockbackForce = 0f;
}
