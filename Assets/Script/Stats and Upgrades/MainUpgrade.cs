using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Humans;
using Unity.VisualScripting;
using UnityEngine;

namespace Script.Stats_and_Upgrades
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "MainUpgrade", menuName = "Upgrades/MainUpgrade", order = 1)]
    public class MainUpgrade : ScriptableObject
    {
        public List<ResourceCost> cost;
        public string name;

        public bool CanBuy() => cost.All(r => GameManager.Instance.Resources[r.Resource] >= r.Amount);
    }

    [Serializable]
    public class ResourceCost
    {

        public EResource Resource;
        public int Amount;
        
        public ResourceCost(EResource resource, int amount)
        {
            Resource = resource;
            Amount = amount;
        }
    }
}