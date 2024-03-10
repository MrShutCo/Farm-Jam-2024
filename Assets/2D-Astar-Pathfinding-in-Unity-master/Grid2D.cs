using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid2D : MonoBehaviour
{
    public static Action<int, int> onGridUpdated;
    public float nodeRadius;
    public Node2D[,] Grid;
    
    public Tilemap obstaclemap;
    public List<Node2D> path;
    Vector3 worldBottomLeft;

    [SerializeField] private Tilemap groundMap;
    [SerializeField] private bool debug;

    float nodeDiameter;
    public int gridSizeX, gridSizeY;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        CreateGrid();
    }

    void CreateGrid()
    {
        Grid = new Node2D[gridSizeX, gridSizeY];
        worldBottomLeft = transform.position - Vector3.right * nodeDiameter * gridSizeX / 2 - Vector3.up * nodeDiameter * gridSizeY / 2;
        Vector3 worldPoint = worldBottomLeft;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                Grid[x, y] = new Node2D(false, worldPoint, x, y);
                Grid[x, y].SetObstacle(!groundMap.HasTile(groundMap.WorldToCell(Grid[x, y].worldPosition)));
                var colliderTile = obstaclemap.GetTile(obstaclemap.WorldToCell(Grid[x, y].worldPosition));
                if (colliderTile != null)
                {
                    Grid[x, y].SetObstacle(true);
                }
            }
        }
    }


    public Vector2Int GetGridPos(Vector2Int pos)
    {
        return pos - (new Vector2Int((int)(worldBottomLeft.x/nodeDiameter), (int)(worldBottomLeft.y/nodeDiameter)));
    }

    //gets the neighboring nodes in the 4 cardinal directions. If you would like to enable diagonal pathfinding, uncomment out that portion of code
    public List<Node2D> GetNeighbors(Node2D node)
    {
        List<Node2D> neighbors = new List<Node2D>();

        //checks and adds top neighbor
        if (node.GridX >= 0 && node.GridX < gridSizeX && node.GridY + 1 >= 0 && node.GridY + 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX, node.GridY + 1]);

        //checks and adds bottom neighbor
        if (node.GridX >= 0 && node.GridX < gridSizeX && node.GridY - 1 >= 0 && node.GridY - 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX, node.GridY - 1]);

        //checks and adds right neighbor
        if (node.GridX + 1 >= 0 && node.GridX + 1 < gridSizeX && node.GridY >= 0 && node.GridY < gridSizeY)
            neighbors.Add(Grid[node.GridX + 1, node.GridY]);

        //checks and adds left neighbor
        if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY >= 0 && node.GridY < gridSizeY)
            neighbors.Add(Grid[node.GridX - 1, node.GridY]);
        
        //checks and adds top right neighbor
        if (node.GridX + 1 >= 0 && node.GridX + 1 < gridSizeX && node.GridY + 1 >= 0 && node.GridY + 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX + 1, node.GridY + 1]);

        //checks and adds bottom right neighbor
        if (node.GridX + 1 >= 0 && node.GridX + 1 < gridSizeX && node.GridY - 1 >= 0 && node.GridY - 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX + 1, node.GridY - 1]);

        //checks and adds top left neighbor
        if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY + 1 >= 0 && node.GridY + 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX - 1, node.GridY + 1]);

        //checks and adds bottom left neighbor
        if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY - 1 >= 0 && node.GridY - 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX - 1, node.GridY - 1]);
        
        return neighbors;
    }


    public Node2D NodeFromWorldPoint(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - worldBottomLeft.x) / nodeDiameter);
        int y = Mathf.FloorToInt((worldPosition.y - worldBottomLeft.y) / nodeDiameter);
        return Grid[x, y];
    }

    public void SetWalkableAt(int x, int y, bool isObstacle)
    {
        var newPos = GetGridPos(new Vector2Int(x, y));
        Grid[newPos.x, newPos.y].SetObstacle(isObstacle);
        onGridUpdated?.Invoke(newPos.x, newPos.y);
    }

    //Draws visual representation of grid
    void OnDrawGizmos()
    {
        if (!debug)
            return;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSizeX, gridSizeY, 1));
        Color obstacleColor = Color.red;
        Color walkableColor = Color.white;
        Color obstaclePathColor = Color.black;

        if (Grid != null)
        {
            foreach (Node2D n in Grid)
            {
                if (n.obstacle)
                    Gizmos.color = obstacleColor;
                else
                    Gizmos.color = walkableColor;

                if (path != null && path.Contains(n))
                    Gizmos.color = obstaclePathColor;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeRadius));

            }
        }
    }
}
