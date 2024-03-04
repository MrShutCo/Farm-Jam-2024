using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TilePathFollowers : MonoBehaviour, IGrabStoppable
{
    [SerializeField] LayerMask followMask;
    [SerializeField] float speed = 3;
    [SerializeField] float checkDistance;
    [SerializeField] Vector2 FacingDirection;
    float distanceMoved = 0;

    Transform _transform;
    Vector2 previousPosition;
    Vector2 lastViableDirection;
    RaycastHit2D[] directionHits;
    Rigidbody2D rb;
    Collider2D col;
    GrabbableObjectBase grabbable;
    Animator animator;
    bool active = true;

    public void SetFacingDirection(Vector2 direction)
    {
        FacingDirection = direction;
    }

    private void Awake()
    {
        _transform = transform;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        active = true;
        grabbable = GetComponent<GrabbableObjectBase>();
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        if (grabbable != null)
        {
            grabbable.onGrabbed += () => active = false;
        }
        // SelectDirection(CheckViableDirections());

    }
    private void OnDisable()
    {
        if (grabbable != null)
        {
            grabbable.onGrabbed -= () => active = false;
        }
    }

    private void FixedUpdate()
    {
        /// for every 3 units moved, check for new direction

        // if (distanceMoved >= 3)
        // {
        //     SelectDirection(CheckViableDirections());
        //     distanceMoved = 0;
        // }
        // else
        if (active)
        {
            Flip();
            if (IsRoadClear())
                Move();
        }
    }
    void Flip()
    {
        if (rb.velocity.x > 0.01f)
        {
            _transform.localScale = new Vector3(1f, 1f, 1f);
            animator.SetBool("Vertical", false);
        }
        else if (rb.velocity.x < -0.01f)
        {
            _transform.localScale = new Vector3(-1f, 1f, 1f);
            animator.SetBool("Vertical", false);
        }
        else if (rb.velocity.y > 0.01f)
        {
            _transform.localScale = new Vector3(1f, 1f, 1f);
            animator.SetBool("Vertical", true);
        }
        else if (rb.velocity.y < -0.01f)
        {
            _transform.localScale = new Vector3(-1f, 1f, 1f);
            animator.SetBool("Vertical", true);
        }
    }

    void Move()
    {
        rb.velocity = FacingDirection * speed;
        distanceMoved += Vector2.Distance(previousPosition, rb.position);
        previousPosition = rb.position;
    }

    bool IsRoadClear()
    {
        Vector2 rayOrigin = (Vector2)transform.position + (col.bounds.extents.x * FacingDirection);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, FacingDirection, checkDistance, followMask);

        return hit.collider == null;
    }

    // void SelectDirection(List<Vector2> direction)
    // {
    //     rb.velocity = Vector2.zero;
    //     if (direction.Count > 0)
    //     {
    //         FacingDirection = direction[Random.Range(0, direction.Count)];
    //         _transform.rotation = Quaternion.LookRotation(Vector3.forward, FacingDirection);
    //         lastViableDirection = FacingDirection;
    //     }
    //     else
    //     {
    //         FacingDirection = -lastViableDirection;
    //         _transform.rotation = Quaternion.LookRotation(Vector3.forward, FacingDirection);
    //     }
    // }

    // List<Vector2> CheckViableDirections()
    // {
    //     if (FacingDirection == Vector2.zero)
    //     {
    //         FacingDirection = Vector2.right;
    //     }
    //     List<Vector2> directions = new()//only current facing plus 2 direcitons (left and right)
    //     {
    //         FacingDirection,
    //         new Vector2 (FacingDirection.y, -FacingDirection.x),
    //         new Vector2 (-FacingDirection.y, FacingDirection.x)
    //     };

    //     directionHits = new RaycastHit2D[3];

    //     for (int i = 0; i < directions.Count; i++)
    //     {
    //         Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)_transform.position + (directions[i] * checkDistance), 0.1f, followMask);

    //         if (colliders.Length > 0)
    //         {
    //             directionHits[i] = Physics2D.Raycast(_transform.position, directions[i], checkDistance, followMask);
    //         }
    //         else
    //         {
    //             Debug.Log("No Hit " + directions[i]);
    //             directions.RemoveAt(i);
    //         }
    //     }
    //     return directions;
    // }

    // private void OnDrawGizmos()
    // {
    //     // playmode only

    //     if (!Application.isPlaying)
    //     {
    //         return;
    //     }
    //     Gizmos.color = Color.magenta;
    //     if (Physics2D.OverlapCircle((Vector2)_transform.position + (FacingDirection * checkDistance), 0.1f, followMask))
    //     {
    //         Gizmos.color = Color.white;
    //         Gizmos.DrawSphere((Vector2)_transform.position + (FacingDirection * checkDistance), 0.1f);

    //     }
    //     Gizmos.color = Color.magenta;
    //     if (Physics2D.OverlapCircle((Vector2)_transform.position + (new Vector2(FacingDirection.y, -FacingDirection.x) * checkDistance), 0.1f, followMask))
    //     {
    //         Gizmos.color = Color.white;
    //         Gizmos.DrawSphere((Vector2)_transform.position + (new Vector2(FacingDirection.y, -FacingDirection.x) * checkDistance), 0.1f);

    //     }
    //     Gizmos.color = Color.magenta;
    //     if (Physics2D.OverlapCircle((Vector2)_transform.position + (new Vector2(FacingDirection.y, FacingDirection.x) * checkDistance), 0.1f, followMask))
    //     {
    //         Gizmos.color = Color.white;
    //         Gizmos.DrawSphere((Vector2)_transform.position + (new Vector2(-FacingDirection.y, FacingDirection.x) * checkDistance), 0.1f);

    //     }

    // }

}
