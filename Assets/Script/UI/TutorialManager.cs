using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    enum ETutorialState
    {
        Default,
        Build,
        Placement,
        Complete,
    }
    [SerializeField] GameObject buildTutorialElement;
    [SerializeField] GameObject placementTutorialElement;

    int startBuildIndex = 0;
    ETutorialState tutorialState = ETutorialState.Default;
    private void OnEnable()
    {
        GameManager.Instance.onGameStateChange += OnGameStateChange;
        GameManager.Instance.onResourceChange += OnResourceChange;
    }
    private void OnDisable()
    {
        GameManager.Instance.onGameStateChange -= OnGameStateChange;
        GameManager.Instance.onResourceChange -= OnResourceChange;
    }
    private void Start()
    {
        startBuildIndex = GameManager.Instance.Buildings.Count;
    }

    void OnGameStateChange(EGameState state)
    {
        switch (tutorialState)
        {
            case ETutorialState.Default:
                if (state == EGameState.Build)
                {
                    buildTutorialElement.SetActive(true);
                    StartCoroutine(LerpIntoExistence(buildTutorialElement));
                    tutorialState = ETutorialState.Build;
                }
                break;
            case ETutorialState.Build:
                if (state == EGameState.Normal)
                {
                    if (GameManager.Instance.Buildings.Count > startBuildIndex)
                    {
                        StartCoroutine(LerpIntoAbyss(buildTutorialElement));
                        placementTutorialElement.SetActive(true);
                        StartCoroutine(LerpIntoExistence(placementTutorialElement));
                        tutorialState = ETutorialState.Placement;
                    }
                }
                break;
            case ETutorialState.Placement:
                break;
            case ETutorialState.Complete:

                break;
        }
    }
    void OnResourceChange()
    {
        if (tutorialState == ETutorialState.Placement)
        {
            int value;
            value = GameManager.Instance.Resources[EResource.Blood] + GameManager.Instance.Resources[EResource.Bones] + GameManager.Instance.Resources[EResource.Organs];
            if (value > 0)
            {
                StartCoroutine(LerpIntoAbyss(placementTutorialElement));
                tutorialState = ETutorialState.Complete;
            }
        }
    }

    IEnumerator LerpIntoExistence(GameObject go)
    {
        float t = 0;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;

        while (t < 1)
        {
            t += Time.deltaTime * .25f;
            go.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
    }
    IEnumerator LerpIntoAbyss(GameObject go)
    {
        float t = 0;
        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;
        while (t < 1)
        {
            t += Time.deltaTime * .25f;
            go.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        Destroy(go);
    }

}
