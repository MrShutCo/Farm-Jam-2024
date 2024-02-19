using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerHealth : HealthBase
{
    Player player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }
    protected override void Start()
    {
        UpdateHealth();
    }
    protected override void UpdateHealth()
    {
        GameManager.Instance.onHealthChange?.Invoke(currentHealth, maxHealth);
    }
    protected override void Die()
    {
        base.Die();
        player.enabled = false;
        Invoke("ReturnHome", .5f);

        // if invoked do this
    }
    protected void ReturnHome()
    {
        transform.position = new Vector3(0, 0, 0);
        player.enabled = true;
    }

}
