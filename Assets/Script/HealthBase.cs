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
    [SerializeField] public int currentHealth { get; private set; }

    Transform _transform;
    Rigidbody2D rb;


    protected virtual void Awake()
    {
        _transform = transform;
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }
    protected virtual void Start()
    {
        healthBar = Instantiate(healthBarPrefab, healthPanel.transform);
        UpdateHealth();
    }

    public virtual void TakeDamage(int damage)
    {
        if (IsInvoking())
        {
            return;
        }
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

    protected virtual void Die()
    {
        Debug.Log(_transform.name + " died");
        rb.velocity = Vector2.zero;

        // Play Death Anim
        //Play Death Sound
        //Play Death Particle
        Invoke("DisableGameObject", 1f);//change length of time to match death animation
        DisableGameObject();
    }
    protected virtual void DisableGameObject()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    protected virtual void UpdateHealth()
    {
        healthBar.UpdateStatusBar(currentHealth, maxHealth);
    }



}
