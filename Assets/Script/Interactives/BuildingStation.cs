using System;
using UnityEngine;

namespace Script.Interactives
{
    public class BuildingStation : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player at build station");
                GameManager.Instance.SetGameState(EGameState.Build);
            }
        }

        private void Update()
        {
            if (GameManager.Instance.GameState == EGameState.Build && Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.SetGameState(EGameState.Normal);
            }
        }
    }
}