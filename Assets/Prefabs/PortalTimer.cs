using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortalTimer : MonoBehaviour
{
    [SerializeField] float timeToTeleport;
    [SerializeField] UnityEngine.UI.Image radialImage;
    [SerializeField] UnityEngine.UI.Image destructibleImage;
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
        if (destructibleImage != null)
        {
            Destroy(destructibleImage.gameObject);
        }
    }
}
