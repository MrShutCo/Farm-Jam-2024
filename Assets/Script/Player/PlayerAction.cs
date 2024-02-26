using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using System;
using UnityEngine;
using Unity.VisualScripting;
using UnityEditor;
using System.IO.Compression;

[RequireComponent(typeof(Player))]
public abstract class PlayerAction : MonoBehaviour
{
    protected Player player;
    protected Transform _transform;
    protected Collider2D col;
    protected Animator animator;
    protected Animator vfxAnimator;

    Vector2 hitBoxSize = new Vector2(3, 2);
    protected float halfExtent;

    protected bool showDebug;


    protected virtual void Awake()
    {
        _transform = transform;
        player = GetComponent<Player>();
    }
    protected virtual void Start()
    {
        col = player.Col;
        showDebug = player.showDebug;
        halfExtent = player.GetHalfExtent();
        animator = player.Animator;
        vfxAnimator = player.VFXAnimator;
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
    protected Vector2 HitPos(Vector2 direction)
    {
        if (direction == Vector2.left || direction == Vector2.right)
        {
            return (Vector2)col.bounds.center + direction * halfExtent;
        }
        else if (direction == Vector2.up || direction == Vector2.down)
        {
            return (Vector2)_transform.position + direction * halfExtent;
        }
        return Vector2.zero;
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
        animator.SetTrigger("AttackTrigger");
        vfxAnimator.transform.position = HitPos(direction);
        vfxAnimator.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        vfxAnimator.SetTrigger("AttackTrigger");

        Collider2D[] hits = GetHits(direction, targetLayers);
        foreach (var hit in hits)
        {
            if (hit.gameObject.TryGetComponent(out HealthBase health))
            {
                if (hitIndex == 2)
                {
                    health.TakeDamage((int)player.BaseDamage * 2);
                    StartCoroutine(KnockBack(health));
                }
                else
                    health.TakeDamage((int)player.BaseDamage);
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
            if (health == null) yield break;
            health.TryGetComponent(out Rigidbody2D rb);
            health.TryGetComponent(out Human human);
            if (rb == null) yield break;
            if (human != null)
                human.StopAllJobs();
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
    Collider2D[] hits;
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
        vfxAnimator.transform.position = HitPos(direction);
        vfxAnimator.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction + Vector2.up);
        vfxAnimator.SetTrigger("CollectTrigger");

        hits = GetHits(direction, targetLayers);
        foreach (var hit in hits)
        {
            if (hit.gameObject.TryGetComponent(out PickUpItem item))
            {
                item.PickUp(carrier);
            }
            else if (hit.gameObject.TryGetComponent(out Human human))
            {
                if (carrier.AddCarriedHumans(human))
                {
                    //move sprite to bag or tendril
                }
                else { Debug.Log("No room for human"); }
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
    float dodgeDuration = 0.25f;
    float dodgeTimer;
    bool dodging;
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
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
