using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace AI.Pathfinding
{
    public class AStar
    {
        public static List<Node> FindPath(Node startNode, Node targetNode, Node[,] grid)
        {
            List<Node> openList = new List<Node> { startNode };
            HashSet<Node> closedList = new HashSet<Node>();

            while (openList.Count > 0)
            {
                Node currentNode = openList.OrderBy(GetFCost).First();
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode == targetNode)
                    return RetracePath(startNode, targetNode);

                foreach (Node neighbor in GetNeighbors(currentNode, grid))
                {
                    if (!neighbor.isWalkable || closedList.Contains(neighbor))
                        continue;

                    int newGCost = currentNode.gCost + GetDistance(currentNode, neighbor);
                    if (newGCost < neighbor.gCost || !openList.Contains(neighbor))
                    {
                        neighbor.gCost = newGCost;
                        neighbor.hCost = GetDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;

                        if (!openList.Contains(neighbor))
                            openList.Add(neighbor);
                    }
                }
            }
            return null;
        }

        private static List<Node> RetracePath(Node startNode, Node targetNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = targetNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Reverse();
            return path;
        }

        private static List<Node> GetNeighbors(Node node, Node[,] grid)
        {
            List<Node> neighbors = new List<Node>();
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (var dir in directions)
            {
                Vector2Int neighborPos = node.position + dir;
                if (neighborPos.x >= 0 && neighborPos.x < grid.GetLength(0) &&
                    neighborPos.y >= 0 && neighborPos.y < grid.GetLength(1))
                {
                    neighbors.Add(grid[neighborPos.x, neighborPos.y]);
                }
            }
            return neighbors;
        }

        private static int GetDistance(Node a, Node b)
        {
            return Mathf.Abs(a.position.x - b.position.x) + Mathf.Abs(a.position.y - b.position.y);
        }

        private static int GetFCost(Node n)
        {
            return n.FCost;
        }
    }
}
