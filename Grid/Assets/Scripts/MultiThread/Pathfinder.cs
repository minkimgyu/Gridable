using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MultiThreadPathfinding
{
    public struct PathResult
    {
        public List<Vector3> path;
        public bool success;

        public PathResult(List<Vector3> path, bool success)
        {
            this.path = path;
            this.success = success;
        }

    }

    public struct PathRequest
    {
        public Vector3 start;
        public Vector3 end;
        public int safeRange;

        public PathRequest(Vector3 start, Vector3 end, int safeRange)
        {
            this.start = start;
            this.end = end;
            this.safeRange = safeRange;
        }
    }

    public class Pathfinder
    {
        GridComponent _gridComponent;

        //Constructor
        public Pathfinder(GridComponent grid)
        {
            _gridComponent = grid;
        }

        public PathResult FindPath(PathRequest request)
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

            Vector3Int startIndex = _gridComponent.ReturnNodeIndex(request.start);
            Vector3Int endIndex = _gridComponent.ReturnNodeIndex(request.end);

            //We start adding to the open set
            Node start = _gridComponent.ReturnNode(startIndex);
            Node target = _gridComponent.ReturnNode(endIndex);
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

                List<Node> neighbourNodes = _gridComponent.CloseNodes[currentNode.NodeIndex.x, currentNode.NodeIndex.y, currentNode.NodeIndex.z];
                for (int i = 0; i < neighbourNodes.Count; i++)
                {
                    if (!closedSet.Contains(neighbourNodes[i]))
                    {
                        //we create a new movement cost for our neighbours
                        float newMovementCostToNeighbour = currentNode.G + GetDistance(currentNode, neighbourNodes[i]);

                        //and if it's lower than the neighbour's cost
                        if (newMovementCostToNeighbour < neighbourNodes[i].G || !traceSet.ContainsKey(neighbourNodes[i].NodeIndex))
                        {
                            Node neighbourNode = neighbourNodes[i];
                            neighbourNode.H = GetDistance(neighbourNodes[i], target);
                            neighbourNode.G = newMovementCostToNeighbour;

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

            
            PathResult result = new PathResult(foundPath, canFind);
            //callback?.Invoke(result);

            return result;
        }

        private List<Vector3> RetracePath(Node startNode, Node endNode, Dictionary<Vector3Int, Vector3Int> traceSet)
        {
            //Retrace the path, is basically going from the endNode to the startNode
            List<Vector3> path = new List<Vector3>();

            Vector3Int currentNodeIndex = endNode.NodeIndex;
            Vector3 currentNodePos = endNode.SurfacePos;

            while (currentNodeIndex != startNode.NodeIndex)
            {
                path.Add(currentNodePos); //by taking the parentNodes we assigned

                currentNodeIndex = traceSet[currentNodeIndex];
                currentNodePos = _gridComponent.ReturnNode(currentNodeIndex).SurfacePos;
            }

            //then we simply reverse the list
            path.Reverse();
            return path;
        }

        private int GetDistance(Node posA, Node posB)
        {
            //We find the distance between each node
            //not much to explain here

            int distX = (int)Mathf.Abs(posA.SurfacePos.x - posB.SurfacePos.x);
            int distZ = (int)Mathf.Abs(posA.SurfacePos.z - posB.SurfacePos.z);
            int distY = (int)Mathf.Abs(posA.SurfacePos.y - posB.SurfacePos.y);

            if (distX > distZ)
            {
                return 14 * distZ + 10 * (distX - distZ) + 10 * distY;
            }

            return 14 * distX + 10 * (distZ - distX) + 10 * distY;
        }
    }
}
