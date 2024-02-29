using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponSelector : MonoBehaviour
{
    [SerializeField] private Transform WeaponParent;
    [SerializeField] private WeaponScriptableObject Weapon;

    [Space]
    [Header("Runtime Filled")]
    public WeaponScriptableObject ActiveWeapon;

    private void Start()
    {
        if (Weapon == null)
        {
            Debug.LogError("No WeaponScriptable Object found for Weapon");
            return;
        }
        WeaponScriptableObject weapon = Instantiate(Weapon);

        if (weapon == null)
        {
            Debug.LogError($"No WeaponScriptable Object  found for WeaponType: {weapon}");
            return;
        }

        ActiveWeapon = weapon;
        weapon.Spawn(WeaponParent, this);
    }
    public void ActivateWeeapon(bool active)
    {
        ActiveWeapon.WeaponModelActive(active);
    }
}
