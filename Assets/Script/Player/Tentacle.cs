using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System;
using UnityEngine;
using UnityEngine.Animations;

public class Tentacle : MonoBehaviour
{
    public event Action<GrabbableObjectBase> onGrabComplete;
    [SerializeField] int linePositionCount = 10;
    Transform _transform;
    Vector2 baseOffset;
    LineRenderer line;
    Coroutine grabbingCR;

    [SerializeField] GrabbableObjectBase grabbedObject;

    bool grabbing = false;
    public bool Grabbing { get { return grabbing; } }

    private void Awake()
    {
        _transform = transform;
        line = GetComponent<LineRenderer>();
    }
    private void Start()
    {
        SetupLine();
    }
    private void Update()
    {
        RefreshLinePositions();
    }
    public void SetBaseOffset(Vector2 position)
    {
        baseOffset = position;
    }
    public void ReturnToBasePosition()
    {
        StopAllCoroutines();
        _transform.position = (Vector2)_transform.parent.position + baseOffset;

        for (int i = 0; i < _transform.childCount; i++)
        {
            _transform.GetChild(i).SetParent(null);
        }
        grabbingCR = null;
        grabbing = false;
    }
    void SetupLine()
    {
        line.positionCount = linePositionCount;
        line.startWidth = .2f;
        line.endWidth = .2f;
        RefreshLinePositions();
    }
    void RefreshLinePositions()
    {
        Vector2 startPos = _transform.position;
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
        controlPoint1 = startPos + new Vector2(0, -1); // Adjust the -1 to control the curve's steepness
        controlPoint2 = endPos + new Vector2(0, -1); // Adjust the -1 to control the curve's steepness

        //set line positions for all points
        for (int i = 0; i < linePositionCount; i++)
        {
            float t = i / (float)linePositionCount;
            line.SetPosition(i, BezierCurve(startPos, controlPoint1, controlPoint2, endPos, t));
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
}
