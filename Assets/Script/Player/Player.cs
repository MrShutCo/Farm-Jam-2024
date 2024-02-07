using System.Collections;
using Assets.Script.Humans;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Player : MonoBehaviour
{
    public event Action<Vector2> onMove;
    public event Action<Vector2, LayerMask> onAttack;
    public event Action<Vector2, LayerMask> onCollect;
    public event Action<Vector2, LayerMask> onGrab;
    public event Action<Vector2, LayerMask> onDrop;

    public Vector2 Facing = Vector2.down;


    [SerializeField] LayerMask collectableLayers;
    [SerializeField] LayerMask grabbableLayers;
    [SerializeField] LayerMask hittableLayers;
    AttackAction attackAction;
    CollectAction collectAction;
    GrabAction grabAction;
    DropAction dropAction;
    Vector2 moveDirection;
    Collider2D col;


    public bool showDebug { get; private set; } = true;
    public float GetHalfExtent()
    {
        return col.bounds.extents.x;
    }

    private void Awake()
    {
        Initializeation();
    }
    void Initializeation()
    {
        col = GetComponents<Collider2D>();
        attackAction = gameObject.AddComponent<AttackAction>();
        collectAction = gameObject.AddComponent<CollectAction>();
        dropAction = gameObject.AddComponent<DropAction>();
    }
    private void Start()
    {
        collectAction.SetGrabLayers(collectableLayers);
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
            onAttack?.Invoke(Facing, hittableLayers);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            onCollect?.Invoke(Facing, collectableLayers);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            onDrop?.Invoke(Facing, collectableLayers);
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
        if (!showDebug)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Facing);
    }
}
