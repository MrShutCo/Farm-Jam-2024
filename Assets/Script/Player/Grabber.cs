using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    [SerializeField] LayerMask targetLayers;
    [SerializeField] Tentacle tentaclePrefab;
    [SerializeField] int maxTentacles = 1;

    [SerializeField] Tentacle[] tentacles;
    [SerializeField] SpriteRenderer highlightGrabbable;
    List<GrabbableObjectBase> grabbables = new List<GrabbableObjectBase>();
    [SerializeField] GrabbableObjectBase closestGrabbable;
    Transform _transform;
    Player player;

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
        player.onChangeDirection += ChangeDirection;
    }
    private void OnDisable()
    {
        player.onChangeDirection -= ChangeDirection;
    }
    void ChangeDirection(Vector2 direction)
    {
        UpdateTentaclePositions();
    }
    private void Update()
    {
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
        UpdateTentaclePositions();
    }
    void UpdateTentaclePositions()
    {
        float distanceBetweenTentacles = 1f;
        float totalDist = tentacles.Length * distanceBetweenTentacles;
        float backDistance = 3;

        for (int i = 0; i < tentacles.Length; i++)
        {
            if (player.Facing == Vector2.up)
                tentacles[i].SetBaseOffset(-(transform.right * (totalDist / 2)) + (transform.right * i * distanceBetweenTentacles) + (transform.up * backDistance));
            if (player.Facing == Vector2.down)
                tentacles[i].SetBaseOffset((transform.right * (totalDist / 2)) - (transform.right * i * distanceBetweenTentacles) - (transform.up * backDistance));
            if (player.Facing == Vector2.left)
                tentacles[i].SetBaseOffset((transform.up * (totalDist / 2)) - (transform.up * i * distanceBetweenTentacles) + (transform.right * backDistance));
            if (player.Facing == Vector2.right)
                tentacles[i].SetBaseOffset(-(transform.up * (totalDist / 2)) + (transform.up * i * distanceBetweenTentacles) - (transform.right * backDistance));
            tentacles[i].ReturnToBasePosition(false);
        }
    }
    public void GrabAction(Vector2 facing)
    {
        if (tentacles[grabberIndex].transform.childCount > 0)
        {
            var heldObject = tentacles[grabberIndex].GetComponentInChildren<GrabbableObjectBase>();
            heldObject.Throw(facing);
            grabbables.Remove(heldObject);
        }

        else if (closestGrabbable != null && !tentacles[grabberIndex].Grabbing)
        {
            tentacles[grabberIndex].GrabObject(closestGrabbable);
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
