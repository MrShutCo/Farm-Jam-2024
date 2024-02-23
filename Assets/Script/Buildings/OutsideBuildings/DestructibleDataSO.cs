using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "DestructibleData", menuName = "DestructibleData")]
public class DestructibleDataSO : ScriptableObject
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] PickUpItem pickUpPrefab;
    [SerializeField] int health;
    public Sprite[] Sprites => sprites;
    public PickUpItem PickUpPrefab => pickUpPrefab;
    public int Health => health;
}
