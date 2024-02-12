using System.Collections;
using Assets.Script.Humans;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Player : MonoBehaviour
{
    public event Action<Vector2> onMove;
    public event Action<Vector2> onChangeDirection;
    public Vector2 Facing = Vector2.down;
    public Vector2 lastDirectionPressed;
    private Dictionary<Vector2, float> keyPressTimes = new Dictionary<Vector2, float>
    {
    { Vector2.up, -1 },
    { Vector2.left, -1 },
    { Vector2.down, -1 },
    { Vector2.right, -1 }
    };

    [SerializeField] LayerMask collectableLayers;
    [SerializeField] LayerMask hittableLayers;
    [SerializeField] Grabber grabber;
    AttackAction attackAction;
    CollectAction collectAction;
    DodgeAction dodgeAction;
    Vector2 moveDirection;
    Collider2D col;

    bool moveActive = true;

    [Header("VFX")]
    [SerializeField] ParticleSystem attackVFX;
    [SerializeField] ParticleSystem collectVFX;
    [SerializeField] ParticleSystem dodgeVFX;



    public bool showDebug = true;
    public float GetHalfExtent()
    {
        return col.bounds.extents.x;
    }

    private void Awake()
    {
        Initialization();
    }
    void Initialization()
    {
        col = GetComponent<Collider2D>();
        attackAction = gameObject.AddComponent<AttackAction>();
        collectAction = gameObject.AddComponent<CollectAction>();
        dodgeAction = gameObject.AddComponent<DodgeAction>();
    }
    private void OnEnable()
    {
        dodgeAction.onDodge += new Action<bool>((bool isActive) => moveActive = !isActive);
        dodgeAction.onDodge += new Action<bool>((bool isActive) => PlayVFX(dodgeVFX, isActive));
    }
    private void OnDisable()
    {
        dodgeAction.onDodge -= new Action<bool>((bool isActive) => moveActive = !isActive);
        dodgeAction.onDodge -= new Action<bool>((bool isActive) => PlayVFX(dodgeVFX, isActive));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            attackAction.Action(Facing, hittableLayers);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            collectAction.Action(Facing, collectableLayers);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            grabber.GrabAction(Facing);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            dodgeAction.Action(Facing, hittableLayers);
        }
    }
    private void FixedUpdate()
    {
        if (moveActive)
            HandleMoveInput();
    }

    void HandleMoveInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            lastDirectionPressed = Vector2.up;
            keyPressTimes[Vector2.up] = Time.time;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            UpdateLastDirectionPressed();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            lastDirectionPressed = Vector2.left;
            keyPressTimes[Vector2.left] = Time.time;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            UpdateLastDirectionPressed();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            lastDirectionPressed = Vector2.down;
            keyPressTimes[Vector2.down] = Time.time;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            UpdateLastDirectionPressed();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            lastDirectionPressed = Vector2.right;
            keyPressTimes[Vector2.right] = Time.time;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            UpdateLastDirectionPressed();
        }
        else
        {
            UpdateLastDirectionPressed();
        }
        Debug.Log("LastDirection Pressed :" + lastDirectionPressed + " " + Facing);

        var horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        var vertical = Input.GetAxisRaw("Vertical"); // -1 is down
        moveDirection = new Vector2(horizontal, vertical).normalized;
        onMove?.Invoke(moveDirection);
        UpdateFacing();
    }
    // Helper method to get the KeyCode for a direction
    KeyCode KeyCodeForDirection(Vector2 direction)
    {
        if (direction == Vector2.up) return KeyCode.W;
        if (direction == Vector2.left) return KeyCode.A;
        if (direction == Vector2.down) return KeyCode.S;
        if (direction == Vector2.right) return KeyCode.D;
        return KeyCode.None;
    }
    void UpdateLastDirectionPressed()
    {
        // Find the most recently pressed key
        Vector2 mostRecentKey = Vector2.zero;
        float mostRecentTime = -1;
        foreach (var pair in keyPressTimes)
        {
            if (Input.GetKey(KeyCodeForDirection(pair.Key)) && pair.Value > mostRecentTime)
            {
                mostRecentKey = pair.Key;
                mostRecentTime = pair.Value;
            }
        }

        // Update lastDirectionPressed
        lastDirectionPressed = mostRecentKey;
    }
    void UpdateFacing()
    {
        if (lastDirectionPressed != Facing)
        {
            Debug.Log("UpdateFacing: " + lastDirectionPressed + " " + Facing);
            Facing = lastDirectionPressed;
            onChangeDirection?.Invoke(Facing);
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

    private void OnDrawGizmosSelected()
    {
        if (!showDebug)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Facing);
    }

    void PlayVFX(ParticleSystem vfx, bool play)
    {
        if (play)
            vfx.Play();
        else
            vfx.Stop();
    }
}
