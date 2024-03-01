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

    Vector2 hitBoxSize = new Vector2(4, 4);
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
        float distanceAdjuster = 1.1f;
        if (direction == Vector2.left || direction == Vector2.right)
        {
            return (Vector2)col.bounds.center + direction * distanceAdjuster * halfExtent;
        }
        else if (direction == Vector2.up || direction == Vector2.down)
        {
            return (Vector2)_transform.position + direction * distanceAdjuster * halfExtent;
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
    protected void Cooldown(float multiplier = 1)
    {
        cooldownTimer = cooldownDuration * multiplier;
    }

    protected void AnimateInDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        vfxAnimator.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}

public class AttackAction : PlayerAction

{
    Collider2D[] hits;
    int hitIndex = 0;
    int maxHits = 3;
    float comboWindow = 0.5f;
    float comboTimer;

    SoundRequest attackSound;
    SoundRequest attackHitSound;
    SoundRequest attackCritSound;
    SoundRequest attackHitCritSound;

    protected override void Awake()
    {
        base.Awake();
        attackSound = new SoundRequest
        {
            SoundType = ESoundType.playerAttack,
            RequestingObject = gameObject,
            SoundSource = ESoundSource.Player,
            RandomizePitch = true,
            Loop = false
        };
        attackHitSound = new SoundRequest
        {
            SoundType = ESoundType.playerAttackHit,
            RequestingObject = gameObject,
            SoundSource = ESoundSource.Player,
            RandomizePitch = true,
            Loop = false
        };
        attackCritSound = new SoundRequest
        {
            SoundType = ESoundType.playerAttackCrit,
            RequestingObject = gameObject,
            SoundSource = ESoundSource.Player,
            RandomizePitch = true,
            Loop = false
        };
        attackHitCritSound = new SoundRequest
        {
            SoundType = ESoundType.playerAttackHitCrit,
            RequestingObject = gameObject,
            SoundSource = ESoundSource.Player,
            RandomizePitch = true,
            Loop = false
        };

    }
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }

    bool CoolingDown()
    {
        if (hitIndex < maxHits)
        {
            return cooldownTimer > cooldownDuration * .5f;
        }
        else
            return cooldownTimer * 1.5 > 0;
    }
    bool ComboValid()
    {
        if (hitIndex == 0)
        {
            return true;
        }
        if (hitIndex < maxHits && comboTimer > 0)
        {
            return true;
        }
        return false;
    }
    public override void Action(Vector2 direction, LayerMask targetLayers)
    {
        if (CoolingDown()) return;
        if (ComboValid()) { hitIndex++; }
        else hitIndex = 1;
        comboTimer = comboWindow;
        Debug.Log("Hit Index: " + hitIndex);
        Cooldown();

        if (hitIndex == maxHits)
        {
            GameManager.Instance.onPlaySound?.Invoke(attackCritSound);
            animator.SetTrigger("AttackTrigger");
            vfxAnimator.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            vfxAnimator.transform.position = (Vector2)col.bounds.center + direction * halfExtent * 2;
            AnimateInDirection(direction);
            vfxAnimator.SetTrigger("AttackTrigger");
        }
        else
        {
            GameManager.Instance.onPlaySound?.Invoke(attackSound);
            animator.SetTrigger("AttackTrigger");
            vfxAnimator.transform.localScale = new Vector3(1, 1, 1);
            vfxAnimator.transform.position = (Vector2)col.bounds.center + direction * halfExtent * 2;
            AnimateInDirection(direction);
            vfxAnimator.SetTrigger("AttackTrigger");
        }

        hits = GetHits(direction, targetLayers);
        if (hits != null && hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (hit.gameObject.TryGetComponent(out HealthBase health))
                {
                    if (hitIndex == maxHits)
                    {
                        health.TakeDamage((int)player.BaseDamage * 2);
                        GameManager.Instance.onPlaySound?.Invoke(attackHitCritSound);
                        StartCoroutine(KnockBack(health));
                    }
                    else
                    {
                        health.TakeDamage((int)player.BaseDamage);
                        GameManager.Instance.onPlaySound?.Invoke(attackHitSound);
                    }
                }
            }
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



    protected override void Update()
    {
        base.Update();
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
        }
        else if (hitIndex < maxHits)
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
    SoundRequest collectTry;

    protected override void Awake()
    {
        base.Awake();
        carrier = GetComponent<Carrier>();
        collectTry = new SoundRequest
        {
            SoundType = ESoundType.playerCollectTry,
            RequestingObject = gameObject,
            SoundSource = ESoundSource.Player,
            RandomizePitch = true,
            Loop = false
        };
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
        animator.SetTrigger("AttackTrigger");
        vfxAnimator.transform.position = (Vector2)col.bounds.center + direction * halfExtent * 2;
        AnimateInDirection(direction);
        vfxAnimator.SetTrigger("CollectTrigger");



        hits = GetHits(direction, targetLayers);
        if (hits != null && hits.Length > 0)
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
                    else
                    {
                        GameManager.Instance.onHumanCarrierFull?.Invoke();
                        Debug.Log("No room for human");
                    }
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
    SoundRequest dodgeSound;
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        chargesAvailable = maxCharges;
        dodgeSound = new SoundRequest
        {
            SoundType = ESoundType.playerDodge,
            RequestingObject = gameObject,
            SoundSource = ESoundSource.Player,
            RandomizePitch = true,
            Loop = false
        };
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
            GameManager.Instance.onPlaySound?.Invoke(dodgeSound);
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
