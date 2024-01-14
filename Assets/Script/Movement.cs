using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    Rigidbody2D body;

    float moveLimiter = 0.7f;

    Vector2 moveDirection;
    public float runSpeed = 20.0f;
    PlayerControls playerControls;
    InputAction move;
    InputAction harvest;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
    }

    void Update()
    {
        // Gives a value between -1 and 1
        //horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        //vertical = Input.GetAxisRaw("Vertical"); // -1 is down
        moveDirection = move.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        body.velocity = moveDirection * runSpeed;
    }
}
