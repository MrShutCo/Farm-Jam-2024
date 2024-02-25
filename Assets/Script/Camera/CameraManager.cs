using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera mainVirtualCam;
    [SerializeField] CinemachineVirtualCamera buildVirtualCam;
    [SerializeField] CinemachineConfiner2D confiner;

    

    private void Awake()
    {
        confiner = mainVirtualCam.GetComponent<CinemachineConfiner2D>();
        GameManager.Instance.onGameStateChange += state =>
        {
            switch (state)
            {
                case EGameState.Build:
                    buildVirtualCam.Priority = 100;
                    mainVirtualCam.Priority = 0;
                    break;
                case EGameState.Normal:
                    buildVirtualCam.Priority = 0;
                    mainVirtualCam.Priority = 100;
                    break;
            }
        };
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
