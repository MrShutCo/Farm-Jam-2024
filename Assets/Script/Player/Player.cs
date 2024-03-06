using System.Collections;
using Assets.Script.Humans;
using System.Collections.Generic;
using UnityEngine;
using System;
using Script.Stats_and_Upgrades;


public class Player : MonoBehaviour
{
    public event Action<Vector2, float> onMove;
    public event Action<int> onUpdateMaxDodgeCharges;
    public Vector2 Facing = Vector2.down;
    public Vector2 lastDirectionPressed;
    public Stats stats;

    [Header("InitStats")]
    [SerializeField] float baseDamage = 20;
    public float BaseDamage => baseDamage;
    [SerializeField] int carryingCapacityHuman = 4;
    [SerializeField] int carryingCapacityResources = -1;
    [SerializeField] float moveSpeed = 8;
    public float MoveSpeed => moveSpeed;
    [SerializeField] float health = 150;
    public float Health => health;
    [SerializeField] int dodgeCharges = 1;

    [Header("Outside Interactions")]

    [SerializeField] LayerMask collectableLayers;
    [SerializeField] LayerMask hittableLayers;
    [SerializeField] Grabber grabber;
    [SerializeField] PortalMaker portalMaker;

    Animator _animator;
    SpriteRenderer _spriteRenderer;
    Rigidbody2D _rb;
    public AttackAction attackAction { get; private set; }
    public CollectAction collectAction { get; private set; }
    public DodgeAction dodgeAction { get; private set; }
    Carrier _carrier;
    Vector2 moveDirection;
    Collider2D _col;

    bool moveActive = true;
    bool combatActive = true;
    float runSpeed;
    int maxDodgeCharges;
    public void SetMaxDodgeCharges(int value)
    {
        maxDodgeCharges = value;
        onUpdateMaxDodgeCharges?.Invoke(maxDodgeCharges);
    }
    int initOrderInLayer;
    public int InitOrderInLayer => initOrderInLayer;
    public void SetOrderInLayer(int value)
    {
        _spriteRenderer.sortingOrder = value;
    }

    [Header("VFX")]
    [SerializeField] ParticleSystem dodgeVFX;
    [SerializeField] Animator vfxAnimator;
    [Header("UpgradeMsg")]
    [SerializeField] Sprite AttackUp;
    [SerializeField] Sprite HealthUp;
    [SerializeField] Sprite CarryUp;
    [SerializeField] Sprite DashUp;

    public Animator Animator => _animator;
    public Animator VFXAnimator => vfxAnimator;
    public Collider2D Col => _col;


    private bool isFacingLeft;

    public bool showDebug = true;
    public float GetHalfExtent()
    {
        return _col.bounds.extents.x;
    }

    private void Awake()
    {
        Initialization();
    }
    void Initialization()
    {
        _col = GetComponent<Collider2D>();
        attackAction = gameObject.AddComponent<AttackAction>();
        collectAction = gameObject.AddComponent<CollectAction>();
        dodgeAction = gameObject.AddComponent<DodgeAction>();
        _rb = GetComponent<Rigidbody2D>();
        _carrier = GetComponent<Carrier>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        initOrderInLayer = _spriteRenderer.sortingOrder;
        _animator = _spriteRenderer.GetComponent<Animator>();
        runSpeed = MoveSpeed;
        _carrier.SetCarryCapacity(carryingCapacityHuman, carryingCapacityResources);
    }
    private void Start()
    {
        SetMaxDodgeCharges(dodgeCharges);
        portalMaker = GetComponentInChildren<PortalMaker>();
        lastDirectionPressed = Vector2.down;
    }


    private void OnEnable()
    {
        dodgeAction.onDodge += (isActive) => OnDodge(isActive);
        GameManager.Instance.onGameStateChange += onGameStateUpdate;
    }
    private void OnDisable()
    {
        dodgeAction.onDodge -= (isActive) => OnDodge(isActive);
        GameManager.Instance.onGameStateChange -= onGameStateUpdate;
    }
    private void OnDodge(bool isActive)
    {
        moveActive = !isActive;
        combatActive = !isActive;
        ParticleSystemRenderer psr = dodgeVFX.GetComponent<ParticleSystemRenderer>();
        psr.flip = new Vector3(_spriteRenderer.transform.localScale.x < 0 ? 1 : 0, 0, 0);
        PlayVFX(dodgeVFX, isActive);
    }

    private void Update()
    {
        if (!combatActive) return;
        if (HandlePortalInput()) return;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            attackAction.Action(lastDirectionPressed, hittableLayers);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            collectAction.Action(lastDirectionPressed, collectableLayers);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            grabber.GrabAction(lastDirectionPressed);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            dodgeAction.Action(lastDirectionPressed, hittableLayers);
        }

    }

    public void Upgrade(EUpgradeType upgrade)
    {
        GameObject go = new GameObject();
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = _spriteRenderer.sortingOrder + 1;

        go.transform.position = transform.position;
        switch (upgrade)
        {
            case EUpgradeType.AttackPlus:
                baseDamage += 10;
                sr.sprite = AttackUp;
                break;
            case EUpgradeType.HealthPlus:
                health += 25;
                GetComponent<PlayerHealth>().SetMaxHealth(health);
                sr.sprite = HealthUp;
                break;
            case EUpgradeType.CarryingCapacityPlus4:
                carryingCapacityHuman += 4;
                _carrier.SetCarryCapacity(carryingCapacityHuman, carryingCapacityResources);
                sr.sprite = CarryUp;
                break;
            case EUpgradeType.DashUp:
                SetMaxDodgeCharges(maxDodgeCharges + 1);
                sr.sprite = DashUp;
                break;
        }
        StartCoroutine(UpgradeAnim(go));
    }
    IEnumerator UpgradeAnim(GameObject go)
    {
        float time = 0;
        while (time < 3)
        {
            time += Time.deltaTime;
            go.transform.position += new Vector3(0, 0.01f, 0);
            yield return null;
        }
        Destroy(go);
    }

    private void FixedUpdate()
    {
        if (moveActive)
            HandleMoveInput();
    }

    private void HandleMoveInput()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(horizontal, vertical).normalized;
        if (moveDirection != Vector2.zero)
            lastDirectionPressed = moveDirection;
        _animator.SetBool("Moving", Mathf.Abs(moveDirection.magnitude) > 0.05f);
        onMove?.Invoke(moveDirection, runSpeed);
        if (moveDirection.x < 0) _spriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
        if (moveDirection.x > 0) _spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
    }

    private bool HandlePortalInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            portalMaker.enabled = true;
            return true;
        }
        if (Input.GetKey(KeyCode.P))
        {
            return true;
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            portalMaker.enabled = false;
            return false;
        }
        return false;
    }
    public void EnablePortal()
    {
        portalMaker.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "HomeBase")
        {
            GameManager.Instance.onEnterHomeBase?.Invoke();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "HomeBase")
        {
            GameManager.Instance.onExitHomeBase?.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebug)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Facing);
    }

    private void PlayVFX(ParticleSystem vfx, bool play)
    {
        if (play)
            vfx.Play();
        else
            vfx.Stop();
    }

    private void onGameStateUpdate(EGameState newGameState)
    {
        switch (newGameState)
        {
            case EGameState.Build:
                moveActive = false;
                combatActive = false;
                _rb.velocity = Vector2.zero;
                moveDirection = Vector2.zero;
                onMove?.Invoke(moveDirection, runSpeed);
                break;
            case EGameState.Death:
                moveActive = false;
                combatActive = false;
                _rb.velocity = Vector2.zero;
                //onMove?.Invoke(moveDirection, runSpeed);
                StartCoroutine(DeathSpiral());
                break;
            case EGameState.Normal:
                moveActive = true;
                combatActive = true;
                break;
            case EGameState.Wild:
                moveActive = true;
                combatActive = true;
                break;
        }
    }
    public IEnumerator DeathSpiral()
    {
        SetOrderInLayer(150);
        float scale = 1.25f;


        while (GameManager.Instance.GameState == EGameState.Death)
        {
            if (transform.localScale.x < scale)
                transform.localScale += new Vector3(0.01f, 0.01f, 0);
            transform.Rotate(Vector3.forward * 2);
            yield return null;
        }
        while (transform.localScale.x > 1)
        {
            if (transform.localScale.x > 1)
                transform.localScale -= new Vector3(0.05f, 0.05f, 0);
            yield return null;
        }
        transform.localScale = new Vector3(1, 1, 1);
        transform.rotation = Quaternion.identity;
        SetOrderInLayer(initOrderInLayer);
    }

}
