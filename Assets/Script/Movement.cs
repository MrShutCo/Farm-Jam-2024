using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FacingDirection
{
    Up,
    Down,
    Left,
    Right
}
public class Movement : MonoBehaviour
{
    Rigidbody2D body;

    float moveLimiter = 0.7f;

    Vector2 moveDirection;
    FacingDirection facingDirection = FacingDirection.Down;
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

        UpdateFacing();
    }

    void FixedUpdate()
    {
        body.velocity = moveDirection * runSpeed;
    }
    void UpdateFacing()
    {
        if (moveDirection == Vector2.zero)
            return;

        if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
        {
            if (moveDirection.x > 0)
                facingDirection = FacingDirection.Right;

            else if (moveDirection.x < 0)
                facingDirection = FacingDirection.Left;
        }
        else if (Mathf.Abs(moveDirection.x) < Mathf.Abs(moveDirection.y))
        {
            if (moveDirection.y > 0)
                facingDirection = FacingDirection.Up;
            else if (moveDirection.y < 0)
                facingDirection = FacingDirection.Down;
        }

        SetFacingDirection(facingDirection);
    }
    void SetFacingDirection(FacingDirection direction)
    {
        //animator.SetTrigger(direction.ToString());
        if (direction == FacingDirection.Up)
            Debug.DrawRay(transform.position, Vector3.up, Color.green);
        else if (direction == FacingDirection.Down)
            Debug.DrawRay(transform.position, Vector3.down, Color.green);
        else if (direction == FacingDirection.Left)
            Debug.DrawRay(transform.position, Vector3.left, Color.green);
        else if (direction == FacingDirection.Right)
            Debug.DrawRay(transform.position, Vector3.right, Color.green);
    }
}
