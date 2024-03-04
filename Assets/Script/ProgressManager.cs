using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;


public class ProgressManager : MonoBehaviour
{
    [SerializeField] Renderer[] progressStatues;
    public int ProgressIndex { get; private set; }

    private void Awake()
    {
        ProgressIndex = 0;
    }

    private void OnEnable()
    {
        GameManager.Instance.onGoalReached += NextGoal;
    }
    private void OnDisable()
    {
        GameManager.Instance.onGoalReached -= NextGoal;
    }

    private void NextGoal()
    {
        StartCoroutine(UpdageProgress());
    }

    IEnumerator UpdageProgress()
    {
        float progress = 0;
        while (progress < 360)
        {
            progress++;
            progressStatues[ProgressIndex].material.SetFloat("_Arc1", 360 - progress);
            yield return null;
        }
        progressStatues[ProgressIndex].material.SetFloat("_Arc1", 0);
        ProgressIndex++;
        if (ProgressIndex >= progressStatues.Length)
        {
            Debug.Log("YOU WIN");
        }
    }
}
