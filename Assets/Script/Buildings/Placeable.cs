using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Buildings
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Placeable", menuName = "Placeable", order = 0)]

    public class Placeable : ScriptableObject
    {
        public Vector2Int Layout;
        public bool IsWalkable;
        
        public Vector2Int GetMidpoint()
        {
            return Layout / 2;
        }
    }
}