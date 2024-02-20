using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using System;
using UnityEngine;
using Unity.VisualScripting;
using UnityEditor;

[RequireComponent(typeof(Player))]
public abstract class PlayerAction : MonoBehaviour
{
    protected Player player;
    protected Transform _transform;
    protected Collider2D col;

    Vector2 hitBoxSize = new Vector2(3, 2);
    protected float halfExtent;

    protected bool showDebug;


    protected virtual void Awake()
    {
        _transform = transform;
        player = GetComponent<Player>();
        col = player.GetComponent<Collider2D>();
        showDebug = player.showDebug;
        halfExtent = player.GetHalfExtent();
    }
    public abstract void Action(Vector2 direction, LayerMask targetLayers);

    protected Collider2D[] GetHits(Vector2 direction, LayerMask targetLayers)
    {
        if (direction == Vector2.left || direction == Vector2.right)
        {
            return Physics2D.OverlapBoxAll((Vector2)col.bounds.center + direction * halfExtent, new Vector2(hitBoxSize.y, hitBoxSize.x), 0, targetLayers);
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
    void OnDrawGizmos()
    {
        if (showDebug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector2)col.bounds.center + Vector2.left * halfExtent, new Vector2(hitBoxSize.y, hitBoxSize.x));
            Gizmos.DrawWireCube((Vector2)col.bounds.center + Vector2.right * halfExtent, new Vector2(hitBoxSize.y, hitBoxSize.x));
            Gizmos.DrawWireCube((Vector2)col.bounds.center + Vector2.up * halfExtent, hitBoxSize);
            Gizmos.DrawWireCube((Vector2)col.bounds.center + Vector2.down * halfExtent, hitBoxSize);
        }
    }
}

public class AttackAction : PlayerAction
{
    int hitIndex = 0;
    float cooldownDuration = 0.5f;
    float cooldownTimer;

    float comboAvailable = 0.25f;
    float comboTimer;
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }
    public override void Action(Vector2 direction, LayerMask targetLayers)
    {
        if (cooldownTimer > 0) return;

        if (comboTimer > 0 && hitIndex < 2)
        {
            hitIndex++;
        }
        //Animate Attack
        Debug.Log("Attack");

        Collider2D[] hits = GetHits(direction, targetLayers);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<Collider2D>().gameObject.TryGetComponent(out HealthBase health))
            {
                if (hitIndex == 2)
                {
                    health.TakeDamage(player.BaseDamage * 2);
                    StartCoroutine(KnockBack(health));
                }
                else
                    health.TakeDamage(player.BaseDamage);
                //Animate Hit
                //Play Hit Sound
            }
        }
        if (hitIndex < 2)
        {
            comboTimer = comboAvailable;
        }
        if (hitIndex > 2)
        {
            Cooldown();
        }
        IEnumerator KnockBack(HealthBase health)
        {
            health.TryGetComponent(out Rigidbody2D rb);
            health.TryGetComponent(out Human human);
            if (human != null)
                human.ClearCurrentJobs();
            rb.isKinematic = false;
            rb.AddForce(direction * 8, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.25f);
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;

        }
    }
    void Update()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
        }
        else
        {
            hitIndex = 0;
        }
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
    void Cooldown()
    {
        cooldownTimer = cooldownDuration;
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
    float dodgeDuration = 0.25f;
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
