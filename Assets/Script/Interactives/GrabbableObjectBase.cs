using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObjectBase : MonoBehaviour
{
    [SerializeField] PhysicsMaterial2D frictionLess;
    [SerializeField] PhysicsMaterial2D frictionNormal;
    [SerializeField] LayerMask exclusionLayers;
    protected Transform _transform;
    protected Rigidbody2D rb;
    protected Collider2D col;

    bool thrown;
    public bool Thrown { get { return thrown; } }


    private void Awake()
    {
        _transform = transform;
        rb = GetComponent<Rigidbody2D>();
        rb.sharedMaterial = frictionNormal;
        col = GetComponent<Collider2D>();
    }

    public virtual GrabbableObjectBase Grabbing(Transform grabParent)
    {
        Debug.Log("Grabbing: " + gameObject.name);
        rb.simulated = false;
        col.enabled = false;
        _transform.parent = grabParent;
        return this;
    }

    public virtual void Throw(Vector2 direction)
    {
        Debug.Log("Throwing: " + gameObject.name);

        col.excludeLayers = exclusionLayers;
        thrown = true;
        _transform.SetParent(null);
        rb.simulated = true;
        col.enabled = true;

        rb.AddForce(direction * 300, ForceMode2D.Impulse);
        StartCoroutine(ThrowingCR());
    }
    IEnumerator ThrowingCR()
    {
        rb.sharedMaterial = frictionLess;
        yield return new WaitForSeconds(.1f);
        rb.sharedMaterial = frictionNormal;
    }
    public void Reset()
    {
        Debug.Log("Resetting: " + gameObject.name);
        rb.simulated = true;
        col.enabled = true;
        _transform.SetParent(null);
    }

    protected void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            other.gameObject.TryGetComponent(out HealthBase health);
            if (health != null)
            {
                health.TakeDamage(1);
                CollisionImpact(other);
            }
        }
    }
    protected virtual void CollisionImpact(Collision2D other)
    {
        Debug.Log("Collision Impact: " + other.gameObject.name);

        Destroy(gameObject);
    }

}
