using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObjectBase : MonoBehaviour
{
    public event Action onGrabbed;
    public event Action onThrown;
    public event Action onLanded;
    [SerializeField] PhysicsMaterial2D frictionLess;
    [SerializeField] PhysicsMaterial2D frictionNormal;
    [SerializeField] LayerMask exclusionLayers;
    [SerializeField] LayerMask explisionLayers;
    [SerializeField] protected bool destroyOnLanding;
    [SerializeField] ParticleSystem impactVFXPrefab;

    protected Transform _transform;
    protected Rigidbody2D _rb;
    protected Collider2D _col;
    protected ParticleSystem impactVFX;
    protected Collider2D[] explosionHits;

    bool thrown;
    public bool Thrown { get { return thrown; } }


    private void Awake()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody2D>();
        _rb.sharedMaterial = frictionNormal;
        _col = GetComponent<Collider2D>();
        if (impactVFXPrefab != null)
        {
            impactVFX = Instantiate(impactVFXPrefab, _transform.position, Quaternion.identity);
        }
    }
    private void Update()
    {
        if (!_rb.simulated && transform.localScale.x < 1.25f)
        {
            transform.localScale += new Vector3(0.01f, 0.01f, 0);
        }
    }

    public virtual GrabbableObjectBase Grabbing(Transform grabParent)
    {
        Debug.Log("Grabbing: " + gameObject.name);
        _rb.simulated = false;
        _col.enabled = false;
        _transform.SetParent(grabParent);
        transform.localPosition = Vector3.zero;
        onGrabbed?.Invoke();

        return this;
    }
    public virtual void Throw(Vector2 direction)
    {
        Debug.Log("Throwing: " + gameObject.name);

        _col.excludeLayers = exclusionLayers;
        thrown = true;
        _transform.SetParent(null);
        _rb.simulated = true;
        _col.enabled = true;

        _rb.AddForce(direction * 300, ForceMode2D.Impulse);
        StartCoroutine(ThrowingCR());
    }
    IEnumerator ThrowingCR()
    {
        onThrown?.Invoke();
        _rb.sharedMaterial = frictionLess;
        yield return new WaitForSeconds(.5f);
        Landing();
    }
    public void Reset()
    {
        Debug.Log("Resetting: " + gameObject.name);
        _rb.simulated = true;
        _col.enabled = true;
        _transform.SetParent(null);
    }

    protected void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.TryGetComponent(out HealthBase health);
            if (health != null)
            {
                health.TakeDamage(100);
                Landing();
            }
        }
    }
    protected virtual void CollisionImpact()
    {
        if (impactVFX != null)
        {
            impactVFX.transform.position = _transform.position;
            explosionHits = Physics2D.OverlapCircleAll(_transform.position, 2, explisionLayers);
            foreach (var hit in explosionHits)
            {
                if (hit.gameObject.TryGetComponent(out HealthBase health))
                {
                    health.TakeDamage(75);
                }
            }
            impactVFX.Play();
        }
        if (destroyOnLanding)
            Destroy(gameObject);
        else thrown = false;
    }
    void Landing()
    {
        _rb.sharedMaterial = frictionNormal;
        CollisionImpact();
        onLanded?.Invoke();
    }

}
