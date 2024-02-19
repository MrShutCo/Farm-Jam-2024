using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBase : MonoBehaviour
{
    protected bool home;
    protected bool paused;

    Collider2D[] colHits;


    private void Awake()
    {
    }

    private void OnEnable()
    {
        GameManager.Instance.onPause += Pause;
    }
    private void OnDisable()
    {
        GameManager.Instance.onPause -= Pause;
    }
    protected virtual void Start()
    {
        Debug.Log("Checking for grid at start");
        colHits = Physics2D.OverlapBoxAll(transform.position, new Vector2(20, 20), 0);
        Debug.Log("Hits being checked: " + colHits.Length + " for " + this.name);
        foreach (var hit in colHits)
        {
            if (hit.CompareTag("Grid"))
            {
                Grid2D grid = hit.transform.parent.GetComponent<Grid2D>();

                if (grid != GameManager.Instance.PathfindingGrid)
                {
                    Debug.Log("Not Home " + this.name);
                    home = false;
                }
                else
                {
                    Debug.Log("Home " + this.name);
                    home = true;
                }
                ChangeLocation(home);
                break;
            }
            else
            {
                Debug.Log("Not Grid " + this.name);
            }
        }

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
    protected virtual void ChangeLocation(bool home) { }
    private void OnCollision(Collider other)
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
            ChangeLocation(home);
        }
    }

}
