using Assets.Buildings;
using Assets.Script.Humans;
using UnityEngine;

namespace Script.Buildings
{
    [CreateAssetMenu(fileName = "ResourceBuildingData", menuName = "ResourceBuildingData")]
    public class ResourceBuildingDataSO : Placeable
    {
        [SerializeField] private Sprite[] sprites;
        [SerializeField] public EResource resource;

        [SerializeField] private int[] numFlayeesPerLevel;
        [SerializeField] private int[] numFlayersPerLevel;
        [SerializeField] public float internalBufferCapacity;
        [SerializeField] private int[] maximumPackagesAllowed;

        public int Level;

        public Sprite GetSprite(int idx) => sprites[idx];
        public Sprite GetSprite() => sprites[Level];
        public int GetFlayerCount() => numFlayersPerLevel[Level];
        public int GetFlayeeCount() => numFlayeesPerLevel[Level];
        public int GetMaxPackages() => maximumPackagesAllowed[Level];

    }
}