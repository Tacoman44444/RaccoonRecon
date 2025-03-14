using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Pathfinding
{
    public class Node
    {
        public Vector2Int position;
        public bool isWalkable;
        public Node parent;

        public int gCost;
        public int hCost;
        public int FCost
        {
            get { return gCost + hCost; }
        }

        public Node(Vector2Int pos, bool walkable)
        {
            position = pos;
            isWalkable = walkable;
        }
    }
}