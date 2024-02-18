using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthBase : MonoBehaviour
{
    [SerializeField] FloatingStatusBar healthBarPrefab;
    FloatingStatusBar healthBar;
    [SerializeField] Transform healthPanel;


    [SerializeField] int maxHealth = 5;
    [SerializeField] public int currentHealth { get; private set; }

    Transform _transform;


    private void Awake()
    {
        _transform = transform;
        currentHealth = maxHealth;
    }
    private void Start()
    {
        healthBar = Instantiate(healthBarPrefab, healthPanel.transform);
        healthBar.UpdateStatusBar(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        healthBar.UpdateStatusBar(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void Heal(int heal)
    {
        currentHealth += heal;
        healthBar.UpdateStatusBar(currentHealth, maxHealth);
    }
    public void SetMaxHealth(int health)
    {
        maxHealth = health;
        currentHealth = maxHealth;
        healthBar.UpdateStatusBar(currentHealth, maxHealth);
    }

    public void Die()
    {
        Debug.Log(_transform.name + " died");

        // Play Death Anim
        //Play Death Sound
        //Play Death Particle
        Invoke("DisableGameObject", 1f);//change length of time to match death animation
        DisableGameObject();
    }
    void DisableGameObject()
    {
        gameObject.SetActive(false);
        
    }



}
