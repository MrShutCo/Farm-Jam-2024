using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBase : MonoBehaviour
{
    public bool home;
    protected bool paused;

    private void OnEnable()
    {
        GameManager.Instance.onPause += Pause;
    }
    private void OnDisable()
    {
        GameManager.Instance.onPause -= Pause;
    }

    public virtual void Pause(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0;
            paused = true;
        }
        else
        {
            Time.timeScale = 1;
            paused = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grid"))
        {
            if (other.GetComponent<Grid2D>() != GameManager.Instance.PathfindingGrid)
            {
                home = false;
            }
            else
            {
                home = true;
            }
        }
    }

}
