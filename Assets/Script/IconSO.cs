using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "IconSO")]
public class IconSO : ScriptableObject
{
    [SerializeField] Sprite FoodIcon;
    [SerializeField] Sprite WoodIcon;
    [SerializeField] Sprite SteelIcon;
    [SerializeField] Sprite ElectronicsIcon;
    [SerializeField] Sprite BloodIcon;
    [SerializeField] Sprite OrgansIcon;
    [SerializeField] Sprite BonesIcon;


    public Sprite GetIcon(EResource resource)
    {
        switch (resource)
        {
            case EResource.Food:
                return FoodIcon;
            case EResource.Wood:
                return WoodIcon;
            case EResource.Steel:
                return SteelIcon;
            case EResource.Electronics:
                return ElectronicsIcon;
            case EResource.Blood:
                return BloodIcon;
            case EResource.Organs:
                return OrgansIcon;
            case EResource.Bones:
                return BonesIcon;
            default:
                return null;
        }
    }
}
