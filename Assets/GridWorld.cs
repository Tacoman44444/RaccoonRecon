using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.Pathfinding;

public class GridWorld : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float nodeSize = 1f;
    public LayerMask obstacleMask;

    private Node[,] grid;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid = new Node[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2 worldPos = new Vector2(x, y) * nodeSize;
                bool isWalkable = !Physics2D.OverlapCircle(worldPos, nodeSize / 2, obstacleMask);

                grid[x, y] = new Node(new Vector2Int(x, y), isWalkable);
            }
        }
    }

    public Node GetNodeFromWorldPosition(Vector2 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / nodeSize);
        int y = Mathf.RoundToInt(worldPosition.y / nodeSize);
        return grid[x, y];
    }

    public Node[,] GetGrid() => grid;
}
