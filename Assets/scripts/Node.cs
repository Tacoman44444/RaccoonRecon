using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Pathfinding
{
    public class Node
    {
        public bool walkable;
        public Vector2 worldPosition;

        public int gridX;
        public int gridY;
        public Node parent;

        public int gCost;
        public int hCost;

        public Node(bool walkable, Vector2 worldPosition, int gridX, int gridY)
        {
            this.walkable = walkable;
            this.worldPosition = worldPosition;
            this.gridX = gridX;
            this.gridY = gridY; 
        }

        public int FCost { get { return gCost + hCost; } }

    }
}