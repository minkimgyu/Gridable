using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFieldPathfinding
{
    public class GroundPathfinder : MonoBehaviour
    {
        GridComponent _gridComponent;
        const int maxSize = 1000;

        Heap<Node> _openList = new Heap<Node>(maxSize);
        HashSet<Node> _closedList = new HashSet<Node>();

        Vector3 _startNodePos;
        Vector3 _endNodePos;

        Heap<Node> _heap;
        HashSet<Node> _visited;

        public void Initialize(GridComponent gridComponent)
        {
            _gridComponent = gridComponent;
            _heap = new Heap<Node>(maxSize);
            _visited = new HashSet<Node>();
        }

        public void FindPath(Vector3 pos)
        {
            _gridComponent.ResetNodeWeight();

            Vector3Int index = _gridComponent.ReturnNodeIndex(pos);
            Node startNode = _gridComponent.ReturnNode(index);
            startNode.PathWeight = 0;

            _heap.Insert(startNode); // ���� ��� ����
            _visited.Add(startNode); // �湮 ó��

            while (_heap.Count > 0)
            {
                Node minNode = _heap.ReturnMin();
                _heap.DeleteMin();

                List<Node> nearNodes = minNode.NearNodesInGround;
                for (int i = 0; i < nearNodes.Count; i++)
                {
                    float currentWeight = nearNodes[i].Weight;

                    Vector3 directionVec = nearNodes[i].Pos - minNode.Pos;
                    currentWeight *= directionVec.magnitude; // �� ���� ������ ���̸� weight�� �߰��� �����ش�.

                    // minNode�� ���ݱ����� ��� ����ġ + �ֺ� ����� ��� ����ġ
                    float pathWeight = minNode.PathWeight + currentWeight;

                    bool nowContainNearNode = _visited.Contains(nearNodes[i]);
                    if (nowContainNearNode == true && nearNodes[i].PathWeight < pathWeight) continue;
                    // �̹� �湮�߰� ����ġ�� ���� �ͺ��� �� ū ��� �ǳʶٱ�

                    // ����ġ ������Ʈ
                    nearNodes[i].PathWeight = pathWeight;

                    // �̹� �湮�ߴٸ� �ǳʶٱ�
                    if (nowContainNearNode == true) continue;
                    _visited.Add(nearNodes[i]);

                    _heap.Insert(nearNodes[i]);
                }
            }

            _gridComponent.CalculateNodePath(startNode);
            _heap.Clear();
            _visited.Clear();
        }
    }
}