using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody2D body;

    float moveLimiter = 0.7f;

    Vector2 moveDirection;
    public float runSpeed = 20.0f;


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
    }

    void Update()
    {
        // Gives a value between -1 and 1
        var horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        var vertical = Input.GetAxisRaw("Vertical"); // -1 is down
        moveDirection = new Vector2(horizontal, vertical).normalized;
        //moveDirection = Input.GetAxis.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        body.velocity = moveDirection * runSpeed;
    }
}
