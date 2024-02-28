using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObjectHealth : HealthBase
{
    [SerializeField] DestructibleDataSO destructibleData;
    Collider2D col;

    protected override void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = destructibleData.Sprites[0];
        col = GetComponent<Collider2D>();
        maxHealth = destructibleData.Health;
        base.Awake();
    }

    protected override void Die()
    {
        if (_spriteRenderer.sprite == destructibleData.Sprites[1]) return;
        col.enabled = false;
        Debug.Log("Spawning");
        Instantiate(destructibleData.PickUpPrefab, _transform.position, Quaternion.identity);
        _spriteRenderer.sprite = destructibleData.Sprites[1];
    }
    protected override void DisableGameObject()
    {


    }
}
