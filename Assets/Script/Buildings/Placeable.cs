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
        public List<Vector2Int> Layout;
        public bool IsWalkable;
        public string Name;
        public GameObject BaseObject;

        public Vector3 GetMidpoint()
        {
            float midX = (Layout.Min(v => v.x) + Layout.Max(v => v.x)+1)/2;
            float midY = (Layout.Min(v => v.y) + Layout.Max(v => v.y) + 1) / 2;
            return new Vector3(midX, midY, 0);
        }
    }
}