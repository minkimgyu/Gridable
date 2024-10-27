using UnityEngine;
using System.Collections;

namespace GridMaster
{
    public class Node : INode<Node>
    {
        //Node's position in the grid
        public int x;
        public int y;
        public int z;

        //Node's costs for pathfinding purposes
        public float hCost;
        public float gCost;
        public Node parentNode;

        // 레이스 컨디션 방지
        public void ResetValues(float hCost, float gCost, Node parentNode)
        {
            //lock(valueLock)
            //{
                this.hCost = hCost;
                this.gCost = gCost;
                this.parentNode = parentNode;
            //}
        }

        public float fCost
        {
            get //the fCost is the gCost+hCost so we can get it directly this way
            {
                return gCost + hCost;
            }
        }

        public int index;

        // 레이스 컨디션 방지
        public void ResetIndex(int index)
        {
            //lock(indexLock)
            //{
                this.index = index;
            //}
        }

        public int GetIndex()
        {
            return index;
        }

        public bool isWalkable = true;
        
        //Reference to the world object so we can have the world position of the node among other things
        public GameObject worldObject;

        //Types of nodes we can have, we will use this later on a case by case examples
        public NodeType nodeType;

        public enum NodeType
        {
            ground,
            air
        }

        public void Dispose()
        {
            index = -1;
            parentNode = null;
        }

        public int CompareTo(Node other)
        {
            int compareValue = fCost.CompareTo(other.fCost);
            if (compareValue == 0) compareValue = hCost.CompareTo(other.hCost);
            return compareValue;
        }
    }
}
