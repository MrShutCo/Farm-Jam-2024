using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera mainVirtualCam;
    [SerializeField] CinemachineConfiner2D confiner;


    private void Awake()
    {
        confiner = mainVirtualCam.GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()
    {
        GameManager.Instance.onTeleport += ConfinerControl;
        GameManager.Instance.onGridChange += UpdateConfiner;
    }
    private void OnDisable()
    {
        GameManager.Instance.onTeleport -= ConfinerControl;
        GameManager.Instance.onGridChange -= UpdateConfiner;
    }

    void ConfinerControl(bool isTeleporting, Vector2 pos)
    {
        confiner.enabled = !isTeleporting;
    }
    void UpdateConfiner(Collider2D col)
    {
        confiner.m_BoundingShape2D = col;
    }

}
