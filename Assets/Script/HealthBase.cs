using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthBase : MonoBehaviour
{
    [SerializeField] protected FloatingStatusBar healthBarPrefab;
    protected FloatingStatusBar healthBar;
    [SerializeField] protected Transform healthPanel;


    [SerializeField] protected int maxHealth = 5;
    [SerializeField] protected int currentHealth;

    Transform _transform;


    protected virtual void Awake()
    {
        _transform = transform;
        currentHealth = maxHealth;
    }
    protected virtual void Start()
    {
        healthBar = Instantiate(healthBarPrefab, healthPanel.transform);
        UpdateHealth();
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        UpdateHealth();
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public virtual void Heal(int heal)
    {
        currentHealth += heal;
        UpdateHealth();
    }
    public virtual void SetMaxHealth(int health)
    {
        maxHealth = health;
        currentHealth = maxHealth;
        UpdateHealth();
    }

    public virtual void Die()
    {
        Debug.Log(_transform.name + " died");

        // Play Death Anim
        //Play Death Sound
        //Play Death Particle
        Invoke("DisableGameObject", 1f);//change length of time to match death animation
    }
    void DisableGameObject()
    {
        gameObject.SetActive(false);
    }

    protected virtual void UpdateHealth()
    {
        healthBar.UpdateStatusBar(currentHealth, maxHealth);
    }



}
