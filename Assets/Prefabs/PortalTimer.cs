using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class PortalTimer : MonoBehaviour
{
    [SerializeField] float timeToTeleport;
    [SerializeField] UnityEngine.UI.Image radialImage;
    private void OnEnable()
    {
        GameManager.Instance.onGameStateChange += StartTimer;
    }
    private void OnDisable()
    {
        GameManager.Instance.onGameStateChange -= StartTimer;
    }
    void StartTimer(EGameState gameState)
    {
        Debug.Log("Start Timer? " + gameState);
        if (gameState == EGameState.Wild)
        {
            StartCoroutine(Teleport());
        }
        if (gameState == EGameState.Normal)
        {
            StopAllCoroutines();
            radialImage.fillAmount = 0;
        }
        if (gameState == EGameState.Death)
        {
            StopAllCoroutines();
            radialImage.fillAmount = 0;
        }
    }
    IEnumerator Teleport()
    {
        float teleportTime = 0;
        while (teleportTime < timeToTeleport)
        {
            teleportTime += Time.deltaTime;
            radialImage.fillAmount = teleportTime / timeToTeleport;
            yield return null;
        }
        GameManager.Instance.Player.GetComponent<Player>().EnablePortal();
    }
}
