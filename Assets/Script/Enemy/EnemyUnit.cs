using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : PoolableObject
{
    [Header("Base Stats")]
    [SerializeField] int health = 1;
    [SerializeField] float movespeed = 1f;

    [Header("Debugging")]
    [SerializeField] List<Transform> _targetOptions;
    [SerializeField] Transform _target;

    [SerializeField] Weapon _weapon;
    Transform _transform;
    Rigidbody2D _rb;
    TargetSensor _targetSensor;
    ParticleSystem _ps;

    Quaternion rotation;
    float attackRange;
    float attackRate;
    float cooldownTimer = Mathf.Infinity;
    float damage;


    private void Awake()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody2D>();
        _targetSensor = GetComponentInChildren<TargetSensor>();
        _ps = GetComponentInChildren<ParticleSystem>();
        LoadWeapon();
    }
    void LoadWeapon()
    {
        attackRange = _weapon.attackRange;
        damage = _weapon.damage;
        attackRate = _weapon.attackRate;
    }

    private void Update()
    {
        CooldownTimer();
        Turn();
        if (IsInRange())
        {
            Attack();
        }

    }
    private void FixedUpdate()
    {
        if (!IsInRange())
        {
            Move();
        }
    }

    void Move()
    {
        if (_target == null || Vector2.Distance(_transform.position, _target.position) > attackRange)
        {
            _rb.velocity = _transform.right * movespeed;
        }
        else
        {
            _rb.velocity = Vector2.zero;
        }

        Debug.DrawRay(_transform.position, _transform.right * attackRange, Color.blue);
    }
    void Turn()
    {
        if (_target == null) return;
        Vector2 direction = (Vector2)_target.position - (Vector2)_rb.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        _transform.rotation = Quaternion.Slerp(_transform.rotation, rotation, 10f);
    }

    void Attack()
    {
        if (cooldownTimer >= attackRate)
        {
            Debug.Log("Attacking");
            _ps.Play();
            cooldownTimer = 0f;
            Physics.SphereCast(transform.position, 1f, transform.forward, out RaycastHit hit);
            if (hit.collider != null)
            {
                Debug.Log("Deal " + damage + " Damage to " + hit.collider.name);
            }
        }
    }

    void CooldownTimer()
    {
        cooldownTimer += Time.deltaTime;
    }


    // private void OnEnable()
    // {
    //     _targetSensor.OnTargetInSensorRange += AddTarget;
    //     _targetSensor.OnTargetOutOfSensorRange += RemoveTarget;
    // }
    // public override void OnDisable()
    // {
    //     base.OnDisable();
    //     _target = null;
    //     _targetOptions.Clear();
    //     _targetSensor.OnTargetInSensorRange -= AddTarget;
    //     _targetSensor.OnTargetOutOfSensorRange -= RemoveTarget;
    // }

    private void AddTarget(Transform target)
    {
        _targetOptions.Add(target);
        if (_target == null)
        {
            UpdateTarget();
        }
    }
    private void RemoveTarget(Transform target)
    {
        _targetOptions.Remove(target);
        if (_target == target)
        {
            UpdateTarget();
        }
    }

    private void UpdateTarget()
    {
        if (_targetOptions.Count == 0)
        {
            Debug.Log("No Target");
            _target = null;
            return;
        }
        if (_targetOptions.Count == 1)
        {
            Debug.Log("1 Target");
            _target = _targetOptions[0];
            return;
        }

        Debug.Log("Many Targets");
        //find closest target - could prioritize by aggro
        float closestDistance = Mathf.Infinity;
        foreach (Transform target in _targetOptions)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                _target = target;
            }
        }
    }
    bool IsInRange()
    {
        if (_target == null) return false;
        return Vector2.Distance(_transform.position, _target.position) <= attackRange;
    }
}
