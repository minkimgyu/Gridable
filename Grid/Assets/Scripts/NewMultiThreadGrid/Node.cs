using UnityEngine;
using System.Collections;

namespace Gridable
{
    public struct Node : INode<Node>
    {
        //Node's position in the grid
        public int x;
        public int y;
        public int z;

        //Node's costs for pathfinding purposes
        public float hCost;
        public float gCost;

        public Node(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            hCost = 0;
            gCost = 0;
        }

        public Node(int x, int y, int z, float hCost, float gCost) : this(x, y, z)
        {
            this.hCost = hCost;
            this.gCost = gCost;
        }

        public float fCost
        {
            get //the fCost is the gCost+hCost so we can get it directly this way
            {
                return gCost + hCost;
            }
        }

        public Vector3Int NodeIndex { get { return new Vector3Int(x, y, z); } }
        public Vector3 Pos { get { return new Vector3(x, y, z); } }

        public int CompareTo(Node other)
        {
            int compareValue = fCost.CompareTo(other.fCost);
            if (compareValue == 0) compareValue = hCost.CompareTo(other.hCost);
            return compareValue;
        }
    }
}
