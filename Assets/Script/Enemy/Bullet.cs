using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private ParticleSystem ImpactSystem;
    private Rigidbody2D rb;

    public delegate void OnDisableCallBack(Bullet Instance);
    public OnDisableCallBack Disable;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot(Vector2 position, Vector2 direction, float speed)
    {
        rb.velocity = Vector2.zero;
        transform.position = position;
        transform.forward = direction;
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ImpactSystem.transform.forward = -1 * transform.forward;
        ImpactSystem.Play();
        rb.velocity = Vector2.zero;
    }
    private void OnParticleSystemStopped()
    {
        Disable?.Invoke(this);
    }
}
