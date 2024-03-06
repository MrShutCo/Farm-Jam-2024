using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class ProgressManager : MonoBehaviour
{
    [SerializeField] Image EndingPanel;
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
    private void Update()
    {
        if (Input.GetKey(KeyCode.Y))
            if (Input.GetKey(KeyCode.U))
                if (Input.GetKeyDown(KeyCode.M))
                {
                    StartCoroutine(EndGame());
                }
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
            StartCoroutine(EndGame());
        }
    }
    IEnumerator EndGame()
    {
        yield return StartCoroutine(LerpToOpacity(EndingPanel, 1, 3));
        SceneManager.LoadScene(2);
    }
    IEnumerator LerpToOpacity(Image image, float targetOpacity, float duration)
    {
        float time = 0;
        float startOpacity = image.color.a;

        while (time < duration)
        {
            Color color = image.color;
            color.a = Mathf.Lerp(startOpacity, targetOpacity, time / duration);
            image.color = color;
            time += Time.deltaTime;
            yield return null;
        }
        Color finalColor = image.color;
        finalColor.a = targetOpacity;
        image.color = finalColor;
    }

}
