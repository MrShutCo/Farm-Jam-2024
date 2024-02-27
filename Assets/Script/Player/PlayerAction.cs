using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using System;
using UnityEngine;
using Unity.VisualScripting;


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
    protected float cooldownTimer;
    protected float cooldownDuration = 0.5f;

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
    virtual protected void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
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
    protected void Cooldown()
    {
        cooldownTimer = cooldownDuration;
    }
}

public class AttackAction : PlayerAction

{
    Collider2D[] hits;
    int hitIndex = 0;
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
        vfxAnimator.transform.position = (Vector2)col.bounds.center + direction * halfExtent;
        //the animation is currently oriented towards the right, how, so the vfx needs to be adjusted for that
        vfxAnimator.transform.LookAt(vfxAnimator.transform.position + new Vector3(direction.x, direction.y, 0));

        vfxAnimator.SetTrigger("AttackTrigger");

        hits = GetHits(direction, targetLayers);
        if (hits != null && hits.Length > 0)
        {
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
                if (rb == null) yield break;
                rb.isKinematic = true;
                rb.velocity = Vector2.zero;
            }
        }
    }
    protected override void Update()
    {
        base.Update();
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
        }
        else
        {
            hitIndex = 0;
        }
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
        if (cooldownTimer > 0) return;
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
        Cooldown();

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
    public event Action<int> onUpdateCurrentCharges;
    Rigidbody2D rb;
    float dodgeDuration = 0.25f;
    float dodgeTimer;
    bool dodging;
    float chargeDuration = .75f;
    int maxCharges = 2;
    int chargesAvailable;
    float timeOfLastCharge;
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        chargesAvailable = maxCharges;
    }
    private void OnEnable()
    {
        player.onUpdateMaxDodgeCharges += (charges) => maxCharges = charges;
        chargesAvailable = maxCharges;
        onUpdateCurrentCharges?.Invoke(chargesAvailable);
    }
    private void OnDisable()
    {
        player.onUpdateMaxDodgeCharges -= (charges) => maxCharges = charges;
    }
    protected override void Start()
    {
        base.Start();
        chargesAvailable = maxCharges;
        onUpdateCurrentCharges?.Invoke(chargesAvailable);
    }
    public override void Action(Vector2 direction, LayerMask targetLayers)
    {
        //if (cooldownDuration > 0) return;
        if (chargesAvailable <= 0)
        {
            return;
        }
        if (dodgeTimer <= 0)
        {
            dodging = true;

            onDodge?.Invoke(dodging);
            col.enabled = false;
            dodgeTimer = dodgeDuration;
            rb.velocity = Vector2.zero;
            rb.AddForce(direction * 15f, ForceMode2D.Impulse);
            chargesAvailable--;
            onUpdateCurrentCharges?.Invoke(chargesAvailable);
            timeOfLastCharge = Time.time;
        }
        //Animate Dodge
    }
    protected override void Update()
    {
        base.Update();
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
        if (chargesAvailable < maxCharges)
        {
            if (timeOfLastCharge + chargeDuration < Time.time)
            {
                chargesAvailable++;
                onUpdateCurrentCharges?.Invoke(chargesAvailable);
                timeOfLastCharge = Time.time;
            }

        }
    }
}
