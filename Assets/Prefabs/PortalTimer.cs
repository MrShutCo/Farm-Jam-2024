using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortalTimer : MonoBehaviour
{
    [SerializeField] float timeToTeleport;
    [SerializeField] UnityEngine.UI.Image radialImage;
    private void OnEnable()
    {
        StartCoroutine(Teleport());
    }
    private void OnDisable()
    {
        radialImage.fillAmount = 0;
        StopAllCoroutines();
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
        GameManager.Instance.SetGameState(EGameState.Normal);
    }
}
