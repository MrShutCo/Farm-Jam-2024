using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;

[RequireComponent(typeof(Player))]
public abstract class PlayerAction : MonoBehaviour
{
    protected Player player;

    protected virtual void Awake()
    {
        player = GetComponent<Player>();
    }
    protected abstract void Action(Vector2 direction);
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
    protected override void Action(Vector2 direction)
    {
        //Animate Attack
        Debug.Log("Attack");
    }
}

[RequireComponent(typeof(Carrier))]
public class GrabAction : PlayerAction
{
    RaycastHit2D hit;
    LayerMask grabLayers;
    Carrier carrier;

    protected override void Awake()
    {
        base.Awake();
        carrier = GetComponent<Carrier>();
    }

    private void OnEnable()
    {
        player.onGrab += Action;
    }
    private void OnDisable()
    {
        player.onGrab -= Action;
    }

    public void SetGrabLayers(LayerMask grabLayers)
    {
        this.grabLayers = grabLayers;
    }

    protected override void Action(Vector2 direction)
    {
        //Animate Grab
        Debug.Log("Grab");

        hit = Physics2D.BoxCast(transform.position, new Vector2(1, 1), 0, Vector2.zero, 0, grabLayers);
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
        player.onDrop += Action;
    }
    private void OnDisable()
    {
        player.onDrop -= Action;
    }
    protected override void Action(Vector2 direction)
    {
        //Animate Drop
        Debug.Log("Drop");

        if (carrier.CarriedHumans.Count > 0)
        {
            carrier.RemoveCarriedHumans(carrier.CarriedHumans[0]);
        }
    }
}
