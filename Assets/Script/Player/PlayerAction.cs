using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using Unity.VisualScripting;
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
        player = GetComponent<Player>();
        showDebug = player.showDebug;
        halfExtent = player.GetHalfExtent();
    }
    protected abstract void Action(Vector2 direction, LayerMask targetLayers);

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
        player.onAttack += Action;
    }
    private void OnDisable()
    {
        player.onAttack -= Action;
    }
    protected override void Action(Vector2 direction, LayerMask targetLayers)
    {
        //Animate Attack
        Debug.Log("Attack");

        Collider2D[] hits = GetHits(direction, targetLayers);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<Collider>().gameObject.TryGetComponent(out HealthBase health))
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
        player.onCollect += Action;
    }
    private void OnDisable()
    {
        player.onCollect -= Action;
    }
    protected override void Action(Vector2 direction, LayerMask targetLayers)
    {
        //Animate Grab
        Debug.Log("Grab");

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
public class GrabAction : PlayerAction
{
    Vector2 hitBoxSize = new Vector2(3, 1);
    RaycastHit2D hit;

    private void OnEnable()
    {
        player.onGrab += Action;
    }
    private void OnDisable()
    {
        player.onGrab -= Action;
    }

    protected override void Action(Vector2 direction, LayerMask targetLayers)
    {
        //Animate Grab
        Debug.Log("Grab");

        hit = Physics2D.BoxCast(_transform.position, new Vector2(1, 1), 0, Vector2.zero, 0, targetLayers);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.TryGetComponent(out Human human))
            {
                //move sprite to bag or tendril
            }
            else if (hit.collider.gameObject.TryGetComponent(out EResource resource))
            {
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
        player.onDrop += Action;
    }
    private void OnDisable()
    {
        player.onDrop -= Action;
    }
    protected override void Action(Vector2 direction, LayerMask targetLayers)
    {
        //Animate Drop
        Debug.Log("Drop");

        if (carrier.CarriedHumans.Count > 0)
        {
            carrier.RemoveCarriedHumans(carrier.CarriedHumans[0]);
        }
    }
}
