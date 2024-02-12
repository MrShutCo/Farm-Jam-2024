using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using System;
using UnityEngine;

[RequireComponent(typeof(Player))]
public abstract class PlayerAction : MonoBehaviour
{
    protected Player player;
    protected Transform _transform;

    Vector2 hitBoxSize = new Vector2(3, 1);
    protected float halfExtent;

    protected bool showDebug;


    protected virtual void Awake()
    {
        _transform = transform;
        player = GetComponent<Player>();
        showDebug = player.showDebug;
        halfExtent = player.GetHalfExtent();
    }
    public abstract void Action(Vector2 direction, LayerMask targetLayers);

    protected Collider2D[] GetHits(Vector2 direction, LayerMask targetLayers)
    {
        if (direction == Vector2.left || direction == Vector2.right)
        {
            return Physics2D.OverlapBoxAll((Vector2)_transform.position + direction * halfExtent, hitBoxSize, 90, targetLayers);
        }
        else if (direction == Vector2.up || direction == Vector2.down)
        {
            return Physics2D.OverlapBoxAll((Vector2)_transform.position + direction * halfExtent, hitBoxSize, 0, targetLayers);
        }
        return null;
    }
    protected Collider2D FirstHit(Collider2D[] hits)
    {
        if (hits.Length > 0)
        {
            return hits[0];
        }
        return null;
    }
}

public class AttackAction : PlayerAction
{
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }
    public override void Action(Vector2 direction, LayerMask targetLayers)
    {
        //Animate Attack
        Debug.Log("Attack");

        Collider2D[] hits = new Collider2D[GetHits(direction, targetLayers).Length];
        hits = GetHits(direction, targetLayers);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<Collider2D>().gameObject.TryGetComponent(out HealthBase health))
            {
                health.TakeDamage(1);
                //Animate Hit
                //Play Hit Sound
            }
        }
    }
}


[RequireComponent(typeof(Carrier))]
public class CollectAction : PlayerAction
{
    RaycastHit2D hit;
    Carrier carrier;

    protected override void Awake()
    {
        base.Awake();
        carrier = GetComponent<Carrier>();
    }

    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }
    public override void Action(Vector2 direction, LayerMask targetLayers)
    {
        //Animate Collect
        Debug.Log("Collect");

        hit = Physics2D.BoxCast(_transform.position, new Vector2(1, 1), 0, Vector2.zero, 0, targetLayers);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.TryGetComponent(out Human human))
            {
                if (carrier.AddCarriedHumans(human))
                {
                    //move sprite to bag or tendril
                }
                else { Debug.Log("No room for human"); }
            }
            else if (hit.collider.gameObject.TryGetComponent(out EResource resource))
            {
                carrier.AddCarriedResources(resource, 1);
                Destroy(hit.collider.gameObject);
            }
        }
    }
}


[RequireComponent(typeof(Carrier))]
public class DropAction : PlayerAction
{
    Carrier carrier;
    protected override void Awake()
    {
        base.Awake();
        carrier = GetComponent<Carrier>();
    }
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }
    public override void Action(Vector2 direction, LayerMask targetLayers)
    {
        //Animate Drop
        Debug.Log("Drop");

        if (carrier.CarriedHumans.Count > 0)
        {
            carrier.RemoveCarriedHumans(carrier.CarriedHumans[0]);
        }
    }
}
public class DodgeAction : PlayerAction
{
    public event Action<bool> onDodge;
    Rigidbody2D rb;
    Collider2D col;
    float dodgeDuration = 0.5f;
    float dodgeTimer;
    bool dodging;
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }
    public override void Action(Vector2 direction, LayerMask targetLayers)
    {
        if (dodgeTimer <= 0)
        {
            dodging = true;

            onDodge?.Invoke(dodging);
            col.enabled = false;
            Debug.Log("Dodge in direction: " + direction);
            dodgeTimer = dodgeDuration;
            rb.velocity = Vector2.zero;
            rb.AddForce(direction * 15f, ForceMode2D.Impulse);
        }
        //Animate Dodge
    }
    void Update()
    {
        if (dodgeTimer > 0)
        {
            dodgeTimer -= Time.deltaTime;
        }
        else if (dodging)
        {
            col.enabled = true;
            rb.velocity = Vector2.zero;
            dodging = false;
            onDodge?.Invoke(dodging);
        }

    }
}
