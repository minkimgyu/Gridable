using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace Gridable
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] bool canDraw;
        [SerializeField] Color nodeColor;
        //Setting up the grid
        public int maxX = 10;
        public int maxY = 1;
        public int maxZ = 10;

        public Node[,,] grid; // our grid

        void Awake()
        {
            //The typical way to create a grid
            grid = new Node[maxX, maxY, maxZ];

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    for (int z = 0; z < maxZ; z++)
                    {
                        Node node = new Node(x, y, z);
                        grid[x, y, z] = node;
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (canDraw == false) return;

            Gizmos.color = nodeColor;

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    for (int z = 0; z < maxZ; z++)
                    {
                        Vector3 pos = new Vector3(x, y, z);
                        Vector3 size = Vector3.one;

                        Gizmos.DrawCube(pos, size);
                    }
                }
            }
        }

        public Node GetNode(int x, int y, int z)
        {
            //Used to get a node from a grid,
            //If it's greater than all the maximum values we have
            //then it's going to return null

            if(x >= maxX) x = maxX - 1;
            if (y >= maxY) y = maxY - 1;
            if (z >= maxZ) z = maxZ - 1;

            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (z < 0) z = 0;

            Node node = grid[x, y, z];
            return node;
        }

        public Node GetNode(Vector3 pos)
        {
            int x = Mathf.RoundToInt(pos.x);
            int y = Mathf.RoundToInt(pos.y);
            int z = Mathf.RoundToInt(pos.z);

            Node retVal = GetNode(x, y, z);
            return retVal;
        }
    }
}
