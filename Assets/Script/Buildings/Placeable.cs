using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Buildings
{
    public enum EDirection
    {
        Left, Right, Up, Down
    }

    public enum Rotation
    {
        None, QuarterTurn, HalfTurn, ThreeQuarterTurn
    }

    [System.Serializable]
    [CreateAssetMenu(fileName = "Placeable", menuName = "Placeable", order = 0)]

    public class Placeable : ScriptableObject
    {
        public List<Vector2Int> Layout;
        public List<Connector> Connectors;
        public bool IsWalkable;
        public string Name;
        public GameObject BaseObject;
        Rotation rotation;

        public Vector3 GetMidpoint()
        {
            float midX = (Layout.Min(v => v.x) + Layout.Max(v => v.x)+1)/2;
            float midY = (Layout.Min(v => v.y) + Layout.Max(v => v.y) + 1) / 2;
            return new Vector3(midX, midY, 0);
        }

        public void RotateCW()
        {
            Layout = Layout.Select(tile => new Vector2Int(-tile.y, tile.x)).ToList();
            rotation = (Rotation)((int)(rotation + 1) % 4);
        }

        public void RotateCCW()
        {
            Layout = Layout.Select(tile => new Vector2Int(tile.y, -tile.x)).ToList();
            rotation = (Rotation)((int)(rotation - 1) % 4);
        }

    }

    public class FleshFactory
    {
        public static Placeable CreateFlesh(int maxWidth, int maxHeight, int tileCount, int connectorCount)
        {
            HashSet<Vector2Int> fleshTiles = new HashSet<Vector2Int>() { new Vector2Int(0,0) };
            HashSet<Vector2Int> frontier = new HashSet<Vector2Int>() { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };

            // Place Tiles
            for (int i = 0; i < tileCount; i++)
            {
                if (frontier.Count == 0)
                    break;
                var newIndex = Random.Range(0, frontier.Count-1);
                var newFlesh = frontier.ElementAt(newIndex);
                fleshTiles.Add(newFlesh);
                frontier.Remove(newFlesh);


                // Remove add new neighbours to frontier
                if (!fleshTiles.Contains(newFlesh + Vector2Int.left) && Mathf.Abs(newFlesh.x - 1) < maxWidth) frontier.Add(newFlesh + Vector2Int.left);
                if (!fleshTiles.Contains(newFlesh + Vector2Int.right) && Mathf.Abs(newFlesh.x + 1) < maxWidth) frontier.Add(newFlesh + Vector2Int.right);
                if (!fleshTiles.Contains(newFlesh + Vector2Int.down) && Mathf.Abs(newFlesh.y - 1) < maxHeight) frontier.Add(newFlesh + Vector2Int.down);
                if (!fleshTiles.Contains(newFlesh + Vector2Int.up) && Mathf.Abs(newFlesh.y + 1) < maxWidth) frontier.Add(newFlesh + Vector2Int.up);
            }

            List<Connector> connectors = new List<Connector>();

            return new Placeable()
            {
                Layout = fleshTiles.ToList(),
                Name = "Flesh",
                IsWalkable = true,
                Connectors = connectors
            };
        }
    }

    public class Connector
    {
        public Vector2Int Position;
        public EDirection EDirection;
        public bool IsConnected;

    }
}