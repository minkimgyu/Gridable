using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SingleThreadPathfinding
{
    public class GridGenerator : MonoBehaviour
    {
        public Node[,,] CreateGrid(int agentHeight, float nodeSize, Vector3Int sizeOfGrid, LayerMask obstacleLayer, LayerMask nonPassLayer)
        {
            Node[,,] grid = new Node[sizeOfGrid.x, sizeOfGrid.y, sizeOfGrid.z];
            Vector3 halfSize = new Vector3(nodeSize / 2, nodeSize / 2, nodeSize / 2); // halfExtents

            // 1. �ݶ��̴��� �����ؼ� Non-Pass, Block, Empty�� �����ش�.
            for (int x = 0; x < sizeOfGrid.x; x++)
            {
                for (int z = 0; z < sizeOfGrid.z; z++)
                {
                    for (int y = 0; y < sizeOfGrid.y; y++)
                    {
                        Vector3 originPos = transform.position + halfSize; // pivot�� ���� �Ʒ��� ��ġ��Ų��.
                        Vector3 pos = originPos + new Vector3(x, y, z) * nodeSize;

                        // �켱 nonPassCollider ����
                        Collider[] nonPassCollider = Physics.OverlapBox(pos, halfSize, Quaternion.identity, nonPassLayer);
                        if (nonPassCollider.Length > 0)
                        {
                            Node nonPassNode = new Node(pos, Node.State.NonPass);
                            grid[x, y, z] = nonPassNode;
                            continue;
                        }

                        Collider[] obstacleCollider = Physics.OverlapBox(pos, halfSize, Quaternion.identity, obstacleLayer);
                        if (obstacleCollider.Length > 0)
                        {
                            Node bloackNode = new Node(pos, Node.State.Block);
                            grid[x, y, z] = bloackNode;
                            continue;
                        }

                        Node emptyNode = new Node(pos, Node.State.Empty);
                        grid[x, y, z] = emptyNode;
                    }
                }
            }

            // 2. Pass��� �߿��� �Ʒ��� Block�� ��常 �����ؼ� Raycast�� ���ְ� ��絵�� üũ���ش�.

            for (int x = 0; x < sizeOfGrid.x; x++)
            {
                for (int z = 0; z < sizeOfGrid.z; z++)
                {
                    for (int y = 0; y < sizeOfGrid.y; y++)
                    {
                        bool checkAgentHeight = CheckAgentHeight(grid, x, z, y, agentHeight, sizeOfGrid);
                        if (checkAgentHeight == false) continue;

                        Vector3 startPos = grid[x, y, z].Pos + new Vector3(0, nodeSize, 0);

                        // �� ĭ �� �������� �����ɽ�Ʈ�� �Ʒ��� ���� Ȯ���غ���
                        RaycastHit hit;
                        Physics.BoxCast(startPos, halfSize, Vector3.down, out hit, Quaternion.identity, nodeSize, obstacleLayer);
                        if (hit.transform != null)
                        {
                            Vector3 blockNodePos = grid[x, y, z].Pos;
                            Vector3 surfacePos = new Vector3(blockNodePos.x, hit.point.y, blockNodePos.z);

                            grid[x, y, z].HaveSurface = true;
                            grid[x, y, z].SurfacePos = surfacePos;
                        }

                        //// ���� ĭ�� Empty�̰� �ٷ� �Ʒ� ĭ�� Block�� ��� Raycast�� �߻��ؼ� Surface�� üũ���ش�.
                        //if (grid[x, y, z].CurrentState == Node.State.Empty && grid[x, y - 1, z].CurrentState == Node.State.Block)
                        //{
                            
                        //}
                    }
                }
            }

            return grid;
        }

        bool CheckAgentHeight(Node[,,] grid, int x, int z, int y, int agentHeight, Vector3Int sizeOfGrid)
        {

            // agentHeight�� 2�� ��� �� ���� 2ĭ�� ��� ����ִ��� Ȯ���Ѵ�. 
            // �ִ� ���� ���� �����ؼ� �̺��� �۰Բ� �ؾ��Ѵ�.

            // ex)
            // ��
            // ��
            // ��

            // ������ 1�̸�
            // �߰� ���� 2, 3�̴�.
            
            // ������ �������� ���ϱ�
            bool isAppropriateHeight = y + agentHeight < sizeOfGrid.y;
            if(isAppropriateHeight == false) return false;

            // ���� ��尡 Block������� Ȯ���ϱ�
            bool isBlockNode = grid[x, y, z].CurrentState == Node.State.Block;
            if(isBlockNode == false) return false;

            bool isEmpty = true;
            for (int i = y + 1; i <= y + agentHeight; i++)
            {
                if(grid[x, i, z].CurrentState != Node.State.Empty)
                {
                    isEmpty = false;
                    break;
                }
            }

            return isEmpty;
        }
    }

}