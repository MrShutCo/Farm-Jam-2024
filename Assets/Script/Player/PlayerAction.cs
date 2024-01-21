using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;

public abstract class PlayerAction : MonoBehaviour
{
    public abstract void Action();
}

public class GrabAction : PlayerAction
{
    LayerMask grabbableLayers;
    RaycastHit2D hit;
    public override void Action()
    {
        //Animate Grab
        Debug.Log("Grab");

        hit = Physics2D.BoxCast(transform.position, new Vector2(1, 1), 0, Vector2.zero, 0, grabbableLayers);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.TryGetComponent(out Human human))
            {
                if (GameManager.Instance.Carrier.AddCarriedHumans(human))
                {
                    //either move sprite to bag or tendril
                    //or disable
                }
                else { Debug.Log("No room for human"); }
            }
            else if (hit.collider.gameObject.TryGetComponent(out EResource resource))
            {
                GameManager.Instance.Carrier.AddCarriedResources(resource, 1);
                Destroy(hit.collider.gameObject);
            }

        }

    }
}
