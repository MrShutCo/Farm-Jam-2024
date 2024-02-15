using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponSelector : MonoBehaviour
{
    [SerializeField] private WeaponType Weapon;
    [SerializeField] private Transform WeaponParent;
    [SerializeField] private List<WeaponScriptableObject> Weapons;

    [Space]
    [Header("Runtime Filled")]
    public WeaponScriptableObject ActiveWeapon;

    private void Start()
    {
        WeaponScriptableObject weapon = Instantiate(Weapons.Find(w => w.Type == Weapon));

        if (weapon == null)
        {
            Debug.LogError($"No WeaponScriptable Object  found for WeaponType: {weapon}");
            return;
        }

        ActiveWeapon = weapon;
        weapon.Spawn(WeaponParent, this);
    }
}
