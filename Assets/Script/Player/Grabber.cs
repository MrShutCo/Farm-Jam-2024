using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    public event Action onObjectInRange;
    public event Action onObjectGrabbed;
    public event Action onObjectThrown;
    [SerializeField] LayerMask targetLayers;
    [SerializeField] Tentacle tentaclePrefab;
    [SerializeField] int maxTentacles = 1;

    [SerializeField] Tentacle[] tentacles;
    [SerializeField] SpriteRenderer highlightGrabbable;
    List<GrabbableObjectBase> grabbables = new List<GrabbableObjectBase>();
    [SerializeField] GrabbableObjectBase closestGrabbable;
    Transform _transform;
    Player player;
    Vector2 activeDirection;

    int grabberIndex = 0;

    private void Awake()
    {
        _transform = transform;
        highlightGrabbable.enabled = false;
        transform.parent.TryGetComponent(out player);
    }
    private void Start()
    {
        SpawnTentacles();
    }
    private void OnEnable()
    {
        player.onMove += ChangeDirection;
        GameManager.Instance.onGameStateChange += onGameStateChange;
    }
    private void OnDisable()
    {
        player.onMove -= ChangeDirection;
    }

    void onGameStateChange(EGameState state)
    {
        if (state == EGameState.Normal)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    void ChangeDirection(Vector2 direction, float speed)
    {
        activeDirection = direction;
        UpdateTentaclePositions();
    }
    private void Update()
    {
        UpdateTentaclePositions();
        HighlightClosestGrabbable(_transform.position);
    }
    void SpawnTentacles()
    {
        tentacles = new Tentacle[maxTentacles];
        for (int i = 0; i < maxTentacles; i++)
        {
            var tentacle = Instantiate(tentaclePrefab, _transform.position, Quaternion.identity, _transform);
            tentacle.name = "Tentacle " + i;
            tentacles[i] = tentacle;
        }
    }
    void UpdateTentaclePositions()
    {
        float distanceBetweenTentacles = 1f;
        float totalDist = tentacles.Length * distanceBetweenTentacles;
        float backDistance = 3;
        bool multiTentacles = false;

        Vector2 moveDirection = activeDirection;
        Vector2 offset;
        if (multiTentacles)
        {
            for (int i = 0; i < tentacles.Length; i++)
            {
                offset = Vector2.zero;

                if (moveDirection == Vector2.up)
                    offset = -(transform.right * (totalDist / 2)) + (transform.right * i * distanceBetweenTentacles) - (transform.up * backDistance);
                else if (moveDirection == Vector2.down)
                    offset = (transform.right * (totalDist / 2)) - (transform.right * i * distanceBetweenTentacles) + (transform.up * backDistance);
                else if (moveDirection == Vector2.left)
                    offset = (transform.up * (totalDist / 2)) - (transform.up * i * distanceBetweenTentacles) + (transform.right * backDistance);
                else if (moveDirection == Vector2.right)
                    offset = -(transform.up * (totalDist / 2)) + (transform.up * i * distanceBetweenTentacles) - (transform.right * backDistance);
                tentacles[i].SetBaseOffset(offset);
            }
        }
        else
        {
            offset = new Vector2(moveDirection.x * -backDistance, moveDirection.y * -backDistance);

            tentacles[0].SetBaseOffset(offset);
        }


    }

    public void GrabAction(Vector2 facing)
    {
        if (tentacles[grabberIndex].transform.childCount > 0)
        {
            var heldObject = tentacles[grabberIndex].GetComponentInChildren<GrabbableObjectBase>();
            heldObject.Throw(facing);
            onObjectThrown?.Invoke();
            grabbables.Remove(heldObject);
        }

        else if (closestGrabbable != null && !tentacles[grabberIndex].Grabbing)
        {
            tentacles[grabberIndex].GrabObject(closestGrabbable);
            onObjectGrabbed?.Invoke();
        }
    }

    public void HighlightClosestGrabbable(Vector2 position)
    {
        if (tentacles[grabberIndex].transform.childCount > 0 || tentacles[grabberIndex].Grabbing)
        {
            closestGrabbable = null;
            highlightGrabbable.enabled = false;
            return;
        }

        var previousHighlighted = closestGrabbable;
        closestGrabbable = GetClosestGrabbable(_transform.position);

        if (closestGrabbable != previousHighlighted)
        {
            if (closestGrabbable != null)
            {
                highlightGrabbable.enabled = closestGrabbable != null;
                highlightGrabbable.transform.position = closestGrabbable.transform.position;
                highlightGrabbable.size = closestGrabbable.GetComponent<Collider2D>().bounds.size;
            }
            else
            {
                highlightGrabbable.enabled = false;
            }
        }
        else if (closestGrabbable != null)
        {
            highlightGrabbable.transform.position = closestGrabbable.transform.position;
        }
        else
        {
            highlightGrabbable.enabled = false;
        }
    }
    public GrabbableObjectBase GetClosestGrabbable(Vector2 position)
    {
        float closestDistance = float.MaxValue;
        GrabbableObjectBase closestGrabbable = null;
        foreach (GrabbableObjectBase grabbable in grabbables)
        {
            float distance = Vector2.Distance(position, grabbable.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestGrabbable = grabbable;
            }
        }
        onObjectInRange?.Invoke();
        return closestGrabbable;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GrabbableObjectBase grabbable = other.GetComponent<GrabbableObjectBase>();
        if (grabbable != null && !grabbable.Thrown)
        {
            grabbables.Add(grabbable);
        }

    }
    private void OnTriggerExit2D(Collider2D other)
    {
        GrabbableObjectBase grabbable = other.GetComponent<GrabbableObjectBase>();
        if (grabbable != null)
        {
            grabbables.Remove(grabbable);
        }
    }
}
