using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthBase : MonoBehaviour
{
    public event Action<int, int> OnHealthChanged;

    [SerializeField] int maxHealth = 5;
    [SerializeField] int currentHealth;

    Transform _transform;


    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    public void Heal(int heal)
    {
        currentHealth += heal;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    public void SetMaxHealth(int health)
    {
        maxHealth = health;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Die()
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



}
