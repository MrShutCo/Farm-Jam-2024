using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : HealthBase
{

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        UpdateHealth();
    }


    protected override void UpdateHealth()
    {
        GameManager.Instance.onHealthChange?.Invoke(currentHealth, maxHealth);
    }

}
