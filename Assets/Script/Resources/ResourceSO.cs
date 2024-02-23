using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "Resource")]
public class ResourceSO : ScriptableObject
{
    [SerializeField] string resourceName;
    [SerializeField] Sprite resourceSprite;
    [SerializeField] EResource resourceType;
    [SerializeField] int quantity;

    public string ResourceName => resourceName;
    public Sprite ResourceSprite => resourceSprite;
    public int Quantity => quantity;
    public EResource ResourceType => resourceType;

}
