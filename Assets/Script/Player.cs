using System.Collections;
using Assets.Script.Humans;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Player : MonoBehaviour
{
    public event Action<Vector2> onMove;
    public event Action<Vector2> onAttack;
    public event Action<Vector2> onGrab;
    public event Action<Vector2> onDrop;
    [SerializeField] LayerMask grabbableLayers;
    public Vector2 Facing = Vector2.down;
    AttackAction attackAction;
    GrabAction grabAction;
    DropAction dropAction;
    Vector2 moveDirection;

    private void Awake()
    {
        attackAction = gameObject.AddComponent<AttackAction>();
        grabAction = gameObject.AddComponent<GrabAction>();
        dropAction = gameObject.AddComponent<DropAction>();
    }
    private void Start()
    {
        grabAction.SetGrabLayers(grabbableLayers);
    }
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            onAttack?.Invoke(Facing);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            onGrab?.Invoke(Facing);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            onDrop?.Invoke(Facing);
        }
    }
    private void FixedUpdate()
    {
        HandleMoveInput();
    }
    void HandleMoveInput()
    {
        var horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        var vertical = Input.GetAxisRaw("Vertical"); // -1 is down
        moveDirection = new Vector2(horizontal, vertical).normalized;
        onMove?.Invoke(moveDirection);
        UpdateFacing(moveDirection);
    }

    void UpdateFacing(Vector2 moveDirection)
    {
        if (moveDirection == Vector2.zero)
            return;

        if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
        {
            if (moveDirection.x > 0)
                Facing = Vector2.right;

            else if (moveDirection.x < 0)
                Facing = Vector2.left;
        }
        else if (Mathf.Abs(moveDirection.x) < Mathf.Abs(moveDirection.y))
        {
            if (moveDirection.y > 0)
                Facing = Vector2.up;
            else if (moveDirection.y < 0)
                Facing = Vector2.down;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "HomeBase")
        {
            GameManager.Instance.onEnterHomeBase?.Invoke();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "HomeBase")
        {
            GameManager.Instance.onExitHomeBase?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Facing);
    }
}
