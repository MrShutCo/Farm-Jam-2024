using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightController : MonoBehaviour
{
    [SerializeField] GameObject highlighter;
    GameObject currentTarget;

    public void Highlight(GameObject target)
    {
        if (currentTarget == target) return;

        Vector3 pos = target.transform.position;
        currentTarget = target;
        Highlight(pos);
    }

    public void Highlight(Vector3 position)
    {
        highlighter.SetActive(true);
        highlighter.transform.position = position;
    }

    public void Hide()
    {
        currentTarget = null;
        highlighter.SetActive(false);
    }
}
