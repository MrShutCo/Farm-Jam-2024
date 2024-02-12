using System.Collections;
using System;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    public event Action<GrabbableObjectBase> onGrabComplete;
    [SerializeField] int linePositionCount = 10;
    Player player;
    Transform _transform;
    Vector2 baseOffset;
    Vector2 facing;
    LineRenderer line;
    Coroutine grabbingCR;

    [SerializeField] GrabbableObjectBase grabbedObject;

    float timeOffset;
    bool grabbing = false;
    public bool Grabbing { get { return grabbing; } }

    [Header("WiggleEffect")]
    [SerializeField] float wiggleAmount1 = 0.92f; // Adjust this to control the amount of the first wiggle
    [SerializeField] float wiggleSpeed1 = 4.0f; // Adjust this to control the speed of the first wiggle
    [SerializeField] float wiggleAmount2 = 0.1f; // Adjust this to control the amount of the second wiggle
    [SerializeField] float wiggleSpeed2 = 2.93f; // Adjust this to control the speed of the second wiggle


    private void Awake()
    {
        _transform = transform;
        line = GetComponent<LineRenderer>();
        transform.parent.TryGetComponent(out player);
    }
    private void Start()
    {
        SetupLine();
        timeOffset = UnityEngine.Random.Range(0.0f, 2 * Mathf.PI); // Random value between 0 and 2π

    }

    private void OnEnable()
    {
        if (player != null) player.onChangeDirection += ChangeDirection;
    }
    private void OnDisable()
    {
        if (player != null) player.onChangeDirection -= ChangeDirection;
    }
    private void Update()
    {
        if (!grabbing)
            RefreshLinePositions();
    }
    public void SetBaseOffset(Vector2 position)
    {
        if (facing == Vector2.zero || facing == Vector2.down)
        {
            position = new Vector2(position.x, -position.y);
        }
        if (facing == Vector2.up)
        {
            position = new Vector2(position.x, position.y);
        }
        if (facing == Vector2.left)
        {
            position = new Vector2(-position.x, position.y);
        }
        if (facing == Vector2.right)
        {
            position = new Vector2(position.x, position.y);
        }

        baseOffset = position;
    }
    public void ReturnToBasePosition(bool dropObjects = true)
    {
        if (dropObjects)
        {
            StopAllCoroutines();
        }
        _transform.position = (Vector2)_transform.parent.position + baseOffset;

        if (dropObjects)
        {
            for (int i = 0; i < _transform.childCount; i++)
            {
                _transform.GetChild(i).SetParent(null);
            }
            grabbingCR = null;
            grabbing = false;
        }
    }
    void SetupLine()
    {
        line.positionCount = linePositionCount;
        RefreshLinePositions();
    }
    void RefreshLinePositions()
    {
        Vector2 startPos = (Vector2)_transform.parent.position + baseOffset;
        Vector2 endPos = _transform.parent.position;
        Vector2 controlPoint1;
        Vector2 controlPoint2;

        Vector2 BezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector2 p = uuu * p0; // (1-t)^3 * P0
            p += 3 * uu * t * p1; // 3(1-t)^2 * t * P1
            p += 3 * u * tt * p2; // 3(1-t) * t^2 * P2
            p += ttt * p3; // t^3 * P3

            return p;
        }

        //curve towards the player
        float wiggle1 = Mathf.Sin((Time.time + timeOffset) * wiggleSpeed1) * wiggleAmount1;
        float wiggle2 = Mathf.Sin((Time.time + timeOffset + Mathf.PI) * wiggleSpeed2) * wiggleAmount2; // Add Mathf.PI to offset the second wave by half a period

        //set line positions for all points
        for (int i = 0; i < linePositionCount; i++)
        {
            float t = i / (float)linePositionCount;
            float scaledWiggle1 = wiggle1 * (1 - t); // Scale the wiggle amount by (1 - t)
            float scaledWiggle2 = wiggle2 * (1 - t); // Scale the wiggle amount by (1 - t)

            controlPoint1 = startPos + new Vector2(0, 0 + scaledWiggle1); // Control point 1 moves with the first wave
            controlPoint2 = endPos + new Vector2(0, 0 + scaledWiggle2); // Control point 2 moves with the second wave

            line.SetPosition(i, BezierCurve(startPos + new Vector2(0, wiggle1 / 2), controlPoint1, controlPoint2, endPos, t));
        }

    }

    public void GrabObject(GrabbableObjectBase grabbable)
    {
        if (grabbing) return;
        grabbing = true;
        if (grabbingCR != null) return;
        Debug.Log("Start GrabbingCR");
        grabbingCR = StartCoroutine(GrabObjectCR(grabbable));
    }
    IEnumerator GrabObjectCR(GrabbableObjectBase grabbable)
    {
        yield return StartCoroutine(ReachForTargetCR(grabbable.transform));
        Debug.Log("Reached For Target");

        grabbable.Grabbing(_transform);
        Debug.Log("GrabbingCR");


        yield return StartCoroutine(ReturnToBasePositionCR());
        Debug.Log("ReturnToBasePositionCR");

        onGrabComplete?.Invoke(grabbable);
        Debug.Log("GrabComplete Invoked");

        grabbing = false;
        grabbingCR = null;

    }

    IEnumerator ReachForTargetCR(Transform target)
    {
        float duration = .1f;
        float time = 0;
        Vector2 startPos = _transform.position;
        while (time < duration)
        {
            startPos = _transform.position;
            _transform.position = Vector2.Lerp(startPos, target.position, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = target.position;
    }

    IEnumerator ReturnToBasePositionCR()
    {
        float duration = .1f;
        float time = 0;
        Vector2 startPos = _transform.position;
        while (time < duration)
        {
            startPos = _transform.position;
            _transform.position = Vector2.Lerp(startPos, (Vector2)_transform.parent.position + baseOffset, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = (Vector2)_transform.parent.position + baseOffset;
    }

    private void ChangeDirection(Vector2 direction)
    {
        facing = direction;
        SetBaseOffset(baseOffset);
    }

}
