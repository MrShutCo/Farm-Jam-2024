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
        colHits = Physics2D.OverlapBoxAll(transform.position, new Vector2(1, 1), 0);
        foreach (var hit in colHits)
        {
            if (hit.CompareTag("Grid"))
            {
                Debug.Log("Checking Grid Collision");
                if (hit.transform.parent.GetComponent<Grid2D>() != GameManager.Instance.PathfindingGrid)
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
            ChangeLocation(home);
        }
    }

}
