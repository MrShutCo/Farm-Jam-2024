using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon", order = 0)]
public class Weapon : ScriptableObject
{
    [SerializeField] private string WeaponName = "Weapon";
    public string weaponName { get { return WeaponName; } private set { WeaponName = value; } }

    [SerializeField] private float Damage = 10f;
    public float damage { get { return Damage; } private set { Damage = value; } }

    [SerializeField] private float AttackRange = 2f;
    public float attackRange { get { return AttackRange; } private set { AttackRange = value; } }

    [SerializeField] private float AttackRate = 1f;
    public float attackRate { get { return AttackRate; } private set { AttackRate = value; } }
}
