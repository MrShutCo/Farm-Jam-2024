﻿using Assets.Script.Buildings;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Script.Humans
{
    public class HumanInteractor : MonoBehaviour
    {
        [SerializeField] LayerMask rightClickLayer;
        [SerializeField] LayerMask leftClickLayer;
        RaycastHit2D[] hits;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        { 
            if (GameManager.Instance.GameState != EGameState.Normal) return; 
            
            if (GameManager.Instance.CurrentlySelectedHuman != null)
            {
                GameManager.Instance.CurrentlySelectedHuman.transform.position =
                    Camera.main.ScreenToWorldPoint(Input.mousePosition);
                GameManager.Instance.CurrentlySelectedHuman.transform.position -= new Vector3(0,0, GameManager.Instance.CurrentlySelectedHuman.transform.position.z);
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, leftClickLayer);
                if (hit.collider != null)
                {
                    var clickedUser = hit.collider.GetComponent<Human>();
                    if (clickedUser != null && clickedUser.CanBePickedUp())
                    {
                        clickedUser.SelectHuman();
                        return;
                    }
                }
            }

            if (Input.GetMouseButtonDown(1) && GameManager.Instance.CurrentlySelectedHuman != null)
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                

                hits = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity, rightClickLayer);

                foreach (var item in hits)
                {
                    if (item.collider != null)
                    {
                        var clickedBuilding = item.collider.GetComponent<Building>();
                        if (clickedBuilding != null)
                        {
                            var currHuman = GameManager.Instance.CurrentlySelectedHuman;
                            clickedBuilding.AssignHuman(currHuman, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                            GameManager.Instance.CurrentlySelectedHuman.Deselect();
                            GameManager.Instance.CurrentlySelectedHuman = null;
                        }

                        return;
                    }
                }

                GameManager.Instance.CurrentlySelectedHuman = null;
            }
        }
    }
}