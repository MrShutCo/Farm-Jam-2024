using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player))]
public class Movement : MonoBehaviour
{
    Rigidbody2D body;
    Player player;

    //float moveLimiter = 0.7f;
    private SpriteRenderer _spriteRenderer;
    private bool lastFacingLeft;
    
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        player.onMove += Move;
    }

    private void OnDisable()
    {
        player.onMove -= Move;
    }
    void Move(Vector2 moveDirection, float runSpeed)
    {
        body.velocity = moveDirection * runSpeed;
    }
}
