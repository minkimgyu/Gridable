using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

//for more on A* visit
//https://en.wikipedia.org/wiki/A*_search_algorithm
namespace Gridable
{
    public class Pathfinder
    {
        Grid grid;

        //Constructor
        public Pathfinder(Grid grid)
        {
            this.grid = grid;
        }

        public void FindPath(PathRequest request, Action<PathResult> callback)
        {
            bool canFind = false;
            //Typical A* algorythm from here and on
            List<Vector3> foundPath = new List<Vector3>();

            //We need two lists, one for the nodes we need to check and one for the nodes we've already checked
            Heap<Node> openSet = new Heap<Node>(1000);
            HashSet<Node> closedSet = new HashSet<Node>();

            // 여기에 자식 노드 인덱스, 부모 노드 인덱스로 Key Value를 맞춰준다.
            // openSet에 contain을 확인
            Dictionary<Vector3Int, Vector3Int> traceSet = new Dictionary<Vector3Int, Vector3Int>();

            //We start adding to the open set
            Node start = grid.GetNode(request.start);
            Node target = grid.GetNode(request.end);
            openSet.Insert(start);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.ReturnMin();

                //we remove the current node from the open set and add to the closed set
                openSet.DeleteMin();
                closedSet.Add(currentNode);

                //if the current node is the target node
                if (currentNode.NodeIndex == target.NodeIndex)
                {
                    canFind = true;
                    //that means we reached our destination, so we are ready to retrace our path
                    foundPath = RetracePath(start, currentNode, traceSet);
                    break;
                }

                List<Node> neighbourNodes = GetNeighbours(currentNode);

                for (int i = 0; i < neighbourNodes.Count; i++)
                {
                    if (!closedSet.Contains(neighbourNodes[i]))
                    {
                        //we create a new movement cost for our neighbours
                        float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbourNodes[i]);

                        //and if it's lower than the neighbour's cost
                        if (newMovementCostToNeighbour < neighbourNodes[i].gCost || !traceSet.ContainsKey(neighbourNodes[i].NodeIndex))
                        {
                            Node neighbourNode = neighbourNodes[i];
                            neighbourNode.hCost = GetDistance(neighbourNodes[i], target);
                            neighbourNode.gCost = newMovementCostToNeighbour;

                            bool haveIndex = traceSet.ContainsKey(neighbourNode.NodeIndex);
                            traceSet[neighbourNode.NodeIndex] = currentNode.NodeIndex;

                            if (haveIndex == false)
                            {
                                openSet.Insert(neighbourNode);
                            }
                        }
                    }
                }
            }
            
            PathResult result = new PathResult(foundPath, canFind, request.callback);
            callback?.Invoke(result);
        }

        private List<Vector3> RetracePath(Node startNode, Node endNode, Dictionary<Vector3Int, Vector3Int> traceSet)
        {
            //Retrace the path, is basically going from the endNode to the startNode
            List<Vector3> path = new List<Vector3>();

            Vector3Int currentNodeIndex = endNode.NodeIndex;
            Vector3 currentNodePos = endNode.Pos;

            while (currentNodeIndex != startNode.NodeIndex)
            {
                path.Add(currentNodePos); //by taking the parentNodes we assigned

                currentNodeIndex = traceSet[currentNodeIndex];
                currentNodePos = grid.GetNode(currentNodeIndex).Pos;
            }

            //then we simply reverse the list
            path.Reverse();
            return path;
        }

        private List<Node> GetNeighbours(Node node)
        {
            //This is were we start taking our neighbours
            List<Node> retList = new List<Node>();


            // 0, 1
            // 0, -1

            // 1, 1
            // 1, 0
            // 1, -1

            // -1, 1
            // -1, 0
            // -1, -1


            List<Vector3Int> offsets = new List<Vector3Int>()
            {
                new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1),

                new Vector3Int(1, 0, 1), new Vector3Int(1, 0, 0), new Vector3Int(1, 0, -1),

                new Vector3Int(-1, 0, 1), new Vector3Int(-1, 0, 0), new Vector3Int(-1, 0, -1)
            };


            for (int i = 0; i < offsets.Count; i++)
            {
                Node searchNode = new Node(node.x + offsets[i].x, node.y + offsets[i].y, node.z + offsets[i].z);
                retList.Add(searchNode);
            }

            return retList;
        }

        private int GetDistance(Node posA, Node posB)
        {
            //We find the distance between each node
            //not much to explain here

            int distX = Mathf.Abs(posA.x - posB.x);
            int distZ = Mathf.Abs(posA.z - posB.z);
            int distY = Mathf.Abs(posA.y - posB.y);

            if (distX > distZ)
            {
                return 14 * distZ + 10 * (distX - distZ) + 10 * distY;
            }

            return 14 * distX + 10 * (distZ - distX) + 10 * distY;
        }
    }
}
