using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UpdateMainGrid : MonoBehaviour
{
    Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.onGridChange?.Invoke(col);
        }
    }
}
