using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GridMaster;

//for more on A* visit
//https://en.wikipedia.org/wiki/A*_search_algorithm
namespace GridMaster
{
    public class Pathfinder
    {
        Grid gridBase;
        public Node startPosition;
        public Node endPosition;

        public volatile bool jobDone = false;
        PathfindMaster.PathfindingJobComplete completeCallback;
        List<Vector3> foundPath;

        //Constructor
        public Pathfinder(Node start, Node target, PathfindMaster.PathfindingJobComplete callback)
        {
            startPosition = start;
            endPosition = target;
            completeCallback = callback;
            gridBase = Grid.GetInstance();
        }

        public void FindPath()
        {
            foundPath = FindPathActual(startPosition, endPosition);
            jobDone = true;
        }

        public void NotifyComplete()
        {
            if(completeCallback != null)
            {
                completeCallback(foundPath);
            }
        }

        private List<Vector3> FindPathActual(Node start, Node target)
        {
            //Typical A* algorythm from here and on
            List<Vector3> foundPath = new List<Vector3>();

            //We need two lists, one for the nodes we need to check and one for the nodes we've already checked
            Heap<Node> openSet = new Heap<Node>(1000);
            HashSet<Node> closedSet = new HashSet<Node>();

            //We start adding to the open set
            openSet.Insert(start);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.ReturnMin();

                // 가중치가 가장 작은 노드를 찾는 코드임
                //for (int i = 0; i < openSet.Count; i++)
                //{
                //    //We check the costs for the current node
                //    //You can have more opt. here but that's not important now
                //    if (openSet[i].fCost < currentNode.fCost ||
                //        (openSet[i].fCost == currentNode.fCost &&
                //        openSet[i].hCost < currentNode.hCost))
                //    {
                //        //and then we assign a new current node
                //        if (!currentNode.Equals(openSet[i]))
                //        {
                //            currentNode = openSet[i];
                //        }
                //    }
                //}

                //we remove the current node from the open set and add to the closed set
                openSet.DeleteMin();
                closedSet.Add(currentNode);

                //if the current node is the target node
                if (currentNode.Equals(target))
                {
                    //that means we reached our destination, so we are ready to retrace our path
                    foundPath = RetracePath(start, currentNode);
                    break;
                }

                //if we haven't reached our target, then we need to start looking the neighbours
                foreach (Node neighbour in GetNeighbours(currentNode,true))
                {
                    if (!closedSet.Contains(neighbour))
                    {
                        //we create a new movement cost for our neighbours
                        float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                        //and if it's lower than the neighbour's cost
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contain(neighbour))
                        {
                            ////we calculate the new costs
                            //neighbour.gCost = newMovementCostToNeighbour;
                            //neighbour.hCost = GetDistance(neighbour, target);
                            ////Assign the parent node
                            //neighbour.parentNode = currentNode;
                            //And add the neighbour node to the open set

                            neighbour.ResetValues(GetDistance(neighbour, target), newMovementCostToNeighbour, currentNode);

                            if (!openSet.Contain(neighbour))
                            {
                                openSet.Insert(neighbour);
                            }
                        }
                    }
                }
            }
            
            //we return the path at the end
            return foundPath;
        }

        private List<Vector3> RetracePath(Node startNode, Node endNode)
        {
            //Retrace the path, is basically going from the endNode to the startNode
            List<Vector3> path = new List<Vector3>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(new Vector3(currentNode.x, currentNode.y, currentNode.z));
                //by taking the parentNodes we assigned
                currentNode = currentNode.parentNode;
            }

            //then we simply reverse the list
            path.Reverse();

            return path;
        }

        private List<Node> GetNeighbours(Node node, bool getVerticalneighbours = false)
        {
            //This is were we start taking our neighbours
            List<Node> retList = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int yIndex = -1; yIndex <= 1; yIndex++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        int y = yIndex;

                        //If we don't want a 3d A*, then we don't search the y
                        if (!getVerticalneighbours)
                        {
                            y = 0;
                        }

                        if (x == 0 && y == 0 && z == 0)
                        {
                            //000 is the current node
                        }
                        else
                        {
                            Node searchPos = new Node();

                            //the nodes we want are what's forward/backwars,left/righ,up/down from us
                            searchPos.x = node.x + x;
                            searchPos.y = node.y + y;
                            searchPos.z = node.z + z;

                            Node node1 = GetNode(searchPos.x, searchPos.y, searchPos.z);
                            retList.Add(node1);
                        }
                    }
                }
            }

            return retList;

        }

        private Node GetNode(int x, int y, int z)
        {
            Node n = null;
            n = gridBase.GetNode(x, y, z);

            return n;
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
