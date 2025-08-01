using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace AI.Pathfinding
{
    public class AStar
    {
        GridWorld gridWorld;
        public AStar(GridWorld gridWorld)
        {
            this.gridWorld = gridWorld;
        }
        public List<Tile> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Tile startNode = gridWorld.NodeFromWorldPoint(startPos);
            Tile targetNode = gridWorld.NodeFromWorldPoint(targetPos);

            List<Tile> openSet = new List<Tile>();
            HashSet<Tile> closedSet = new HashSet<Tile>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Tile currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost)
                    {
                        if (openSet[i].hCost < currentNode.hCost)
                            currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                } 

                foreach (Tile neighbour in gridWorld.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }

                }
                
            }
            Debug.Log("Fuck. A star didn't work gang");
            return null;
        }

        private List<Tile> RetracePath(Tile startNode, Tile endNode)
        {
            List<Tile> path = new List<Tile>();
            Tile currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();
            gridWorld.path = path;
            return path;
        }

        private int GetDistance(Tile a, Tile b)
        {
            int dstX = Mathf.Abs(a.gridX - b.gridX);
            int dstY = Mathf.Abs(a.gridY - b.gridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);

        }
    }
}
