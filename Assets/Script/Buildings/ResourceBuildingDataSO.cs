using Assets.Buildings;
using Assets.Script.Humans;
using UnityEngine;

namespace Script.Buildings
{
    [CreateAssetMenu(fileName = "ResourceBuildingData", menuName = "ResourceBuildingData")]
    public class ResourceBuildingDataSO : Placeable
    {
        [SerializeField] public Sprite workedArea;
        [SerializeField] private Sprite[] sprites;
        [SerializeField] public EResource resource;

        [SerializeField] public float damageToFlayee;
        [SerializeField] private int[] numFlayeesPerLevel;
        [SerializeField] private int[] numFlayersPerLevel;
        [SerializeField] public float internalBufferCapacity;
        [SerializeField] private int[] maximumPackagesAllowed;

        public Sprite GetSprite(int level) => sprites[level];
        public int GetFlayerCount(int level) => numFlayersPerLevel[level];
        public int GetFlayeeCount(int level) => numFlayeesPerLevel[level];
        public int GetMaxPackages(int level) => maximumPackagesAllowed[level];

    }
}