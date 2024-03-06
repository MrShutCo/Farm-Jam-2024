using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : HealthBase
{
    Player player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
        maxHealth = (int)player.Health;
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
        Invoke("ReturnHome", 2f);
        Debug.Log("Returning Home");
        if (currentHealth <= 0)
        {
            Debug.Log("Changing Game State");
            GameManager.Instance.SetGameState(EGameState.Death);
        }

        // if invoked do this
    }
    protected void ReturnHome()
    {
        transform.position = (Vector2)FindObjectOfType<DropPointBuilding>().transform.position + Vector2.up * 3f;
        Heal(maxHealth);
    }

}
