using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Script.Humans;

public class HealthBase : MonoBehaviour
{
    [SerializeField] protected FloatingStatusBar healthBarPrefab;
    protected FloatingStatusBar healthBar;
    [SerializeField] protected Transform healthPanel;

    [SerializeField] protected float initHealth = 5;
    public float InitHealth => initHealth;
    [SerializeField] protected float maxHealth = 5;
    public float MaxHealth => maxHealth;
    [SerializeField] public float currentHealth { get; private set; }

    protected Transform _transform;
    protected SpriteRenderer _spriteRenderer;
    Rigidbody2D _rb;


    protected virtual void Awake()
    {
        _transform = transform;
        currentHealth = maxHealth;
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (healthPanel == null) return;
        healthBar = Instantiate(healthBarPrefab, healthPanel.transform);
    }
    protected virtual void Start()
    {
        UpdateHealth();
    }

    public virtual void TakeDamage(float damage)
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
        else
        {
            if (_spriteRenderer == null) return;
            StartCoroutine(FlashRed());
        }
    }
    IEnumerator FlashRed()
    {
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.color = Color.white;
    }

    public virtual void Heal(float heal)
    {
        currentHealth = Mathf.Clamp(currentHealth + heal, currentHealth + heal, maxHealth);
        UpdateHealth();
    }
    public virtual void SetMaxHealth(float health)
    {
        maxHealth = health;
        currentHealth = maxHealth;
        UpdateHealth();
    }

    protected virtual void Die()
    {
        Debug.Log(_transform.name + " died");
        if (_rb != null)
            _rb.velocity = Vector2.zero;

        // Play Death Anim
        //Play Death Sound
        //Play Death Particle
        Invoke("DisableGameObject", 1f);//change length of time to match death animation
        DisableGameObject();
    }
    protected virtual void DisableGameObject()
    {
        if (_spriteRenderer != null)
            _spriteRenderer.color = Color.white;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    protected virtual void UpdateHealth()
    {
        if (healthBar == null) return;
        healthBar.UpdateStatusBar(currentHealth, maxHealth);
    }



}
