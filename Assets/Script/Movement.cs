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
    void Move(Vector2 moveDirection, float runSpeed)
    {
        body.velocity = moveDirection * runSpeed;
    }
}
