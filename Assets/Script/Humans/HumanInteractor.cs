using System;
using Assets.Script.Buildings;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Assets.Script.Humans
{
    public class HumanInteractor : MonoBehaviour
    {
        [SerializeField] LayerMask rightClickLayer;
        [SerializeField] LayerMask leftClickLayer;
        [SerializeField] private LayerMask humanLayer;
        RaycastHit2D[] hits;

        [SerializeField] private Transform wildHumanParent;

        void OnEnable()
        {
            GameManager.Instance.onGameStateChange += OnGameStateChange;
        }
        void OnDisable()
        {
            GameManager.Instance.onGameStateChange -= OnGameStateChange;
        }
        void OnGameStateChange(EGameState newState)
        {
            if (newState == EGameState.Dialogue)
            {
                Human human = GameManager.Instance.CurrentlySelectedHuman;
                if (human != null)
                {
                    human.Deselect();
                    GameManager.Instance.CurrentlySelectedHuman = null;
                    human.StartCoroutine(human.LerpToPosition(FindObjectOfType<LiveStockBuilding>().transform.position));
                }
            }
        }


        // Update is called once per frame
        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                var humans = wildHumanParent.GetComponentsInChildren<Human>();
                foreach (var human in humans)
                    human.Select();
            }


            if (Input.GetKeyUp(KeyCode.Tab))
            {
                var humans = wildHumanParent.GetComponentsInChildren<Human>();
                foreach (var human in humans)
                    human.Deselect();
            }

            if (GameManager.Instance.GameState != EGameState.Normal) return;

            if (GameManager.Instance.CurrentlySelectedHuman != null)
            {
                GameManager.Instance.CurrentlySelectedHuman.transform.position =
                    Camera.main.ScreenToWorldPoint(Input.mousePosition);
                GameManager.Instance.CurrentlySelectedHuman.transform.position -= new Vector3(0, 0, GameManager.Instance.CurrentlySelectedHuman.transform.position.z);
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, leftClickLayer);
                if (hit.collider != null)
                {
                    var clickedUser = hit.collider.GetComponent<Human>();
                    if (clickedUser != null && clickedUser.CanBePickedUp())
                    {
                        clickedUser.SelectHuman();
                        GameManager.Instance.UnassignHumanFromBuilding(clickedUser);
                        return;
                    }
                }
            }


            if (Input.GetMouseButtonDown(1) && GameManager.Instance.CurrentlySelectedHuman != null)
            {
                GameManager.Instance.CurrentlySelectedHuman.Deselect();
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