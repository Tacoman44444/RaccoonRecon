using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.Pathfinding;

public class GridWorld : MonoBehaviour
{
    public Node TESTplayer;
    public Node TESTtarget;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;
    public List<Node> path;

    float nodeDiameter;
    int gridSizeX;
    int gridSizeY;

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
        path = new List<Node>();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY]; 
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int X = 0; X < gridSizeX; X++)
        {
            for (int Y = 0; Y < gridSizeY; Y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (X * nodeDiameter + nodeRadius) + Vector3.up * (Y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius - 0.01f, unwalkableMask));
                grid[X, Y] = new Node(walkable, worldPoint, X, Y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && grid[checkX, checkY].walkable)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;

    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }


    /*
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));

        if (grid != null)
        {
            foreach (Node n in grid)
            { 
                Gizmos.color = (n.walkable) ? Color.red : Color.green;
                if (path.Contains(n))
                {
                    Gizmos.color = Color.cyan;
                }
                if (TESTplayer == n)
                {
                    Gizmos.color = Color.yellow;
                }
                if (TESTtarget == n)
                {
                    Gizmos.color = Color.blue;
                }
                Gizmos.DrawCube(n.worldPosition, new Vector3(nodeDiameter - 0.1f, nodeDiameter - 0.1f, 0));
            }
        }
    } */
    
}
