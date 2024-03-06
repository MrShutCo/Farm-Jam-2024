using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PortalTimer : MonoBehaviour
{
    [SerializeField] float timeToTeleport;
    [SerializeField] UnityEngine.UI.Image radialImage;
    [SerializeField] UnityEngine.UI.Image destructibleImage;
    float timer;
    private void OnEnable()
    {
        timer = 0;
        StartCoroutine(Teleport());
        GameManager.Instance.onAddTime += AddTime;
    }
    private void OnDisable()
    {
        radialImage.fillAmount = 0;
        StopAllCoroutines();
        GameManager.Instance.onAddTime -= AddTime;
    }
    private void AddTime(int time)
    {
        timer = Mathf.Clamp(timer - (float)time, 0, timeToTeleport);
    }
    IEnumerator Teleport()
    {
        timer = 0;
        while (timer < timeToTeleport)
        {
            timer += Time.deltaTime;
            radialImage.fillAmount = timer / timeToTeleport;
            yield return null;
        }
        GameManager.Instance.Player.GetComponent<Player>().EnablePortal();
        if (destructibleImage != null)
        {
            Destroy(destructibleImage.gameObject);
        }
    }
}
