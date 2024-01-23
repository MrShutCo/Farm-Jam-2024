using Assets.Script.Buildings;
using System.Collections;
using UnityEngine;

namespace Assets.Script.Humans
{
    public class HumanInteractor : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit.collider != null)
                {
                    var clickedUser = hit.collider.GetComponent<Human>();
                    if (clickedUser != null)
                    {
                        clickedUser.SelectHuman();
                    }
                }
            }

            if (Input.GetMouseButtonDown(1) && GameManager.Instance.CurrentlySelectedHuman != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit.collider != null)
                {
                   var clickedBuilding = hit.collider.GetComponent<Building>();
                    if (clickedBuilding != null)
                    {
                        GameManager.Instance.CurrentlySelectedHuman.StopCurrentJob();
                        GameManager.Instance.CurrentlySelectedHuman.AddJob(new MoveToJob(clickedBuilding.transform.position));
                        GameManager.Instance.CurrentlySelectedHuman.AddJob(new WorkJob(clickedBuilding));
                    }
                }
            }
        }
    }
}