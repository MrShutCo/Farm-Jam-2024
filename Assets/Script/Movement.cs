using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player))]
public class Movement : MonoBehaviour
{
    [SerializeField] float runSpeed = 20.0f;

    Rigidbody2D body;
    Player player;

    //float moveLimiter = 0.7f;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        player.onMove += Move;
    }

    private void OnDisable()
    {
        player.onMove -= Move;
    }
    void Move(Vector2 moveDirection)
    {
        body.velocity = moveDirection * runSpeed;
    }
}
