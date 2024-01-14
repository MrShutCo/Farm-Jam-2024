using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Crop Data", menuName = "New Crop Data")]
public class CropData : ScriptableObject
{
    public Vector2Int[] BuildingFoundationLayout;
    public bool IsWalkable;
    public int TimeToHarvest;

    public Sprite[] growProgressSprites;
    public Sprite readyToHarvestSprite;

    public int purchasePrice;
    public int sellPrice;
}
