using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MultiThreadPathfinding
{
    public class GridComponent : MonoBehaviour
    {
        GridGenerator _gridGenerator;

        [SerializeField] float _nodeSize = 0.5f;
        [SerializeField] Vector3Int _sizeOfGrid;
        [SerializeField] LayerMask _blockMask;
        [SerializeField] LayerMask _nonPassMask;

        [SerializeField] float _surfaceHeight = 0.2f;

        [SerializeField] Color _wireColor = new Color();
        [SerializeField] Color _passNodeColor = new Color();
        [SerializeField] Color _nonPassNodeColor = new Color();
        [SerializeField] Color _blockNodeColor = new Color();

        [SerializeField] Color _surfaceNodeColor;

        [SerializeField] bool _showRect;
        [SerializeField] bool _showNonPass;
        [SerializeField] bool _showBlockNode;
        [SerializeField] bool _showSurface;
        [SerializeField] bool _showNavigationRect;

        Node[,,] _grid; // our grid
        List<Node>[,,] _closeNodes; // our grid
        public List<Node>[,,] CloseNodes { get { return _closeNodes; } }

        public void Initialize()
        {
            _gridGenerator = GetComponent<GridGenerator>();
            _grid = _gridGenerator.CreateGrid(_nodeSize, _sizeOfGrid, _blockMask, _nonPassMask);

            _closeNodes = new List<Node>[_sizeOfGrid.x, _sizeOfGrid.y, _sizeOfGrid.z];
            for (int x = 0; x < _sizeOfGrid.x; x++)
            {
                for (int y = 0; y < _sizeOfGrid.y; y++)
                {
                    for (int z = 0; z < _sizeOfGrid.z; z++)
                    {
                        _closeNodes[x, y, z] = ReturnNearNodesInGround(new Vector3Int(x, y, z));
                    }
                }
            }
        }

        public List<Node> ReturnNearNodesInGround(Vector3Int index)
        {
            List<Node> nearNodes = new List<Node>();

            // y축 높낮이 차이가 있는 경우
            List<Vector3Int> closeIndex = new List<Vector3Int> {
            new Vector3Int(index.x - 1, index.y - 1, index.z + 1), new Vector3Int(index.x, index.y - 1, index.z + 1), new Vector3Int(index.x + 1, index.y - 1, index.z + 1),
            new Vector3Int(index.x - 1, index.y - 1, index.z), new Vector3Int(index.x + 1, index.y - 1, index.z),
            new Vector3Int(index.x - 1, index.y - 1, index.z - 1), new Vector3Int(index.x, index.y - 1, index.z - 1), new Vector3Int(index.x + 1, index.y - 1, index.z - 1),

            new Vector3Int(index.x - 1, index.y + 1, index.z + 1), new Vector3Int(index.x, index.y + 1, index.z + 1), new Vector3Int(index.x + 1, index.y + 1, index.z + 1),
            new Vector3Int(index.x - 1, index.y + 1, index.z), new Vector3Int(index.x + 1, index.y + 1, index.z),
            new Vector3Int(index.x - 1, index.y + 1, index.z - 1), new Vector3Int(index.x, index.y + 1, index.z - 1), new Vector3Int(index.x + 1, index.y + 1, index.z - 1)
        };

            for (int i = 0; i < closeIndex.Count; i++)
            {
                bool isOutOfRange = IsOutOfRange(closeIndex[i]);
                if (isOutOfRange == true) continue;

                Node node = ReturnNode(closeIndex[i]);
                if (node.CurrentState != Node.State.Block) continue;

                nearNodes.Add(node);
            }


            // y축 높이가 같고 주변 그리드 ↑ ↓ ← → 의 경우
            //       (0)
            //        ↑ 
            // (1) ←  ※ → (2)
            //        ↓ 
            //       (3)

            //Tuple<Vector3Int, bool>

            List<Vector3Int> nearIndex = new List<Vector3Int> {
            new Vector3Int(index.x - 1, index.y, index.z),
            new Vector3Int(index.x, index.y, index.z - 1), new Vector3Int(index.x, index.y, index.z + 1),
            new Vector3Int(index.x + 1, index.y, index.z),
        };

            for (int i = 0; i < nearIndex.Count; i++)
            {
                bool isOutOfRange = IsOutOfRange(nearIndex[i]);
                if (isOutOfRange == true) continue;

                Node node = ReturnNode(nearIndex[i]);
                if (node.CurrentState != Node.State.Block) continue;

                nearNodes.Add(node);
            }


            // y축 높이가 같고 주변 그리드 ↗ ↘ ↙ ↖ 의 경우
            // (0)      (1)
            //   ↖    ↗
            //      ※
            //   ↙    ↘ 
            // (2)      (3)

            List<Vector3Int> crossIndex = new List<Vector3Int> {
            new Vector3Int(index.x - 1, index.y, index.z - 1), new Vector3Int(index.x - 1, index.y, index.z + 1),
            new Vector3Int(index.x + 1, index.y, index.z - 1), new Vector3Int(index.x + 1, index.y, index.z + 1),
        };

            for (int i = 0; i < crossIndex.Count; i++)
            {
                bool isOutOfRange = IsOutOfRange(crossIndex[i]);
                if (isOutOfRange == true) continue;

                Node node = ReturnNode(crossIndex[i]);
                if (node.CurrentState != Node.State.Block) continue;

                // 갈 수 있는 코너인지 체크
                Node node1, node2;
                switch (i)
                {
                    case 0:
                        if (IsOutOfRange(nearIndex[0]) == true || IsOutOfRange(nearIndex[1]) == true) continue;

                        node1 = ReturnNode(nearIndex[0]);
                        node2 = ReturnNode(nearIndex[1]);
                        if (node1.CanStep == false || node2.CanStep == false) continue;
                        break;
                    case 1:
                        if (IsOutOfRange(nearIndex[0]) == true || IsOutOfRange(nearIndex[2]) == true) continue;

                        node1 = ReturnNode(nearIndex[0]);
                        node2 = ReturnNode(nearIndex[2]);
                        if (node1.CanStep == false || node2.CanStep == false) continue;
                        break;
                    case 2:
                        if (IsOutOfRange(nearIndex[1]) == true || IsOutOfRange(nearIndex[3]) == true) continue;

                        node1 = ReturnNode(nearIndex[1]);
                        node2 = ReturnNode(nearIndex[3]);
                        if (node1.CanStep == false || node2.CanStep == false) continue;
                        break;
                    case 3:
                        if (IsOutOfRange(nearIndex[2]) == true || IsOutOfRange(nearIndex[3]) == true) continue;

                        node1 = ReturnNode(nearIndex[2]);
                        node2 = ReturnNode(nearIndex[3]);
                        if (node1.CanStep == false || node2.CanStep == false) continue;
                        break;
                }

                nearNodes.Add(ReturnNode(crossIndex[i]));
            }

            return nearNodes;
        }

        public List<Node> ReturnNearNodes(Vector3Int index)
        {
            List<Node> nearNodes = new List<Node>();

            // 주변 그리드
            List<Vector3Int> closeIndex = new List<Vector3Int> {
            new Vector3Int(index.x - 1, index.y - 1, index.z + 1), new Vector3Int(index.x, index.y - 1, index.z + 1), new Vector3Int(index.x + 1, index.y - 1, index.z + 1),
            new Vector3Int(index.x - 1, index.y - 1, index.z), new Vector3Int(index.x, index.y - 1, index.z), new Vector3Int(index.x + 1, index.y - 1, index.z),
            new Vector3Int(index.x - 1, index.y - 1, index.z - 1), new Vector3Int(index.x, index.y - 1, index.z - 1), new Vector3Int(index.x + 1, index.y - 1, index.z - 1),

            new Vector3Int(index.x - 1, index.y, index.z + 1), new Vector3Int(index.x, index.y, index.z + 1), new Vector3Int(index.x + 1, index.y, index.z + 1),
            new Vector3Int(index.x - 1, index.y, index.z), new Vector3Int(index.x + 1, index.y, index.z),
            new Vector3Int(index.x - 1, index.y, index.z - 1), new Vector3Int(index.x, index.y, index.z - 1), new Vector3Int(index.x + 1, index.y, index.z - 1),

            new Vector3Int(index.x - 1, index.y + 1, index.z + 1), new Vector3Int(index.x, index.y + 1, index.z + 1), new Vector3Int(index.x + 1, index.y + 1, index.z + 1),
            new Vector3Int(index.x - 1, index.y + 1, index.z), new Vector3Int(index.x, index.y + 1, index.z), new Vector3Int(index.x + 1, index.y + 1, index.z),
            new Vector3Int(index.x - 1, index.y + 1, index.z - 1), new Vector3Int(index.x, index.y + 1, index.z - 1), new Vector3Int(index.x + 1, index.y + 1, index.z - 1)
        };

            for (int i = 0; i < closeIndex.Count; i++)
            {
                bool isOutOfRange = IsOutOfRange(closeIndex[i]);
                if (isOutOfRange == true) continue;

                Node node = ReturnNode(closeIndex[i]);
                nearNodes.Add(node);
            }

            return nearNodes;
        }

        bool IsOutOfRange(Vector3Int index)
        {
            bool isOutOfRange = index.x < 0 || index.y < 0 || index.z < 0 || index.x >= _sizeOfGrid.x || index.y >= _sizeOfGrid.y || index.z >= _sizeOfGrid.z;
            if (isOutOfRange == true) return true;

            return false;
        }


        public Node ReturnNode(Vector3Int index) { return _grid[index.x, index.y, index.z]; }
        public Node ReturnNode(int x, int y, int z) { return _grid[x, y, z]; }

        public Vector3 ReturnClampedRange(Vector3 pos)
        {
            Vector3 bottomLeftPos = ReturnNode(Vector3Int.zero).Pos;
            Vector3 topRightPos = ReturnNode(_sizeOfGrid.x - 1, _sizeOfGrid.y - 1, _sizeOfGrid.z - 1).Pos; // --> 실질적 위치는 노드의 크기를 곱해줘야 한다.

            // 반올림하고 범위 안에 맞춰줌
            // 이 부분은 GridSize 바뀌면 수정해야함
            float xPos = Mathf.Clamp(pos.x, bottomLeftPos.x, topRightPos.x);
            float yPos = Mathf.Clamp(pos.y, bottomLeftPos.y, topRightPos.y);
            float zPos = Mathf.Clamp(pos.z, bottomLeftPos.z, topRightPos.z);

            return new Vector3(xPos, yPos, zPos);
        }

        public Vector3 ReturnNodePos(Vector3 worldPos)
        {
            Vector3Int index = ReturnNodeIndex(worldPos);
            return ReturnNode(index).Pos;
        }

        public Vector3Int ReturnNodeIndex(Vector3 worldPos)
        {
            Vector3 clampedPos = ReturnClampedRange(worldPos);
            Vector3 bottomLeftPos = ReturnNode(Vector3Int.zero).Pos;

            float xRelativePos = (clampedPos.x - bottomLeftPos.x) / _nodeSize;
            float yRelativePos = (clampedPos.y - bottomLeftPos.y) / _nodeSize;
            float zRelativePos = (clampedPos.z - bottomLeftPos.z) / _nodeSize;

            int xIndex = (int)Mathf.Clamp(xRelativePos, 0, _sizeOfGrid.x - 1);
            int yIndex = (int)Mathf.Clamp(yRelativePos, 0, _sizeOfGrid.y - 1);
            int zIndex = (int)Mathf.Clamp(zRelativePos, 0, _sizeOfGrid.z - 1);

            return new Vector3Int(xIndex, yIndex, zIndex);
        }

        void DrawGizmoCube(Vector3 pos, Color color, Vector3 size)
        {
            Gizmos.color = color;
            Gizmos.DrawCube(pos, size);
        }

        void DrawGizmoCube(Vector3 pos, Color color, float size, bool isWire = false)
        {
            Gizmos.color = color;

            if (isWire) Gizmos.DrawWireCube(pos, new Vector3(size, size, size));
            else Gizmos.DrawCube(pos, new Vector3(size, size, size));
        }

        void DrawGrid()
        {
            if (_showRect == false) return;

            for (int x = 0; x < _sizeOfGrid.x; x++)
            {
                for (int z = 0; z < _sizeOfGrid.z; z++)
                {
                    for (int y = 0; y < _sizeOfGrid.y; y++)
                    {
                        if (_showNavigationRect)
                        {
                            Vector3 originPos = transform.position + new Vector3(_nodeSize / 2, _nodeSize / 2, _nodeSize / 2);
                            DrawGizmoCube(originPos + new Vector3(x, y, z) * _nodeSize, _wireColor, _nodeSize, true);
                        }

                        if (_grid == null) continue;

                        Node node = _grid[x, y, z];

                        if (node.CurrentState == Node.State.Empty)
                        {
                            DrawGizmoCube(node.Pos, _passNodeColor, _nodeSize);
                            continue;
                        }

                        if (_showNonPass && node.CurrentState == Node.State.NonPass)
                        {
                            DrawGizmoCube(node.Pos, _nonPassNodeColor, _nodeSize);
                            continue;
                        }

                        if (_showBlockNode && node.CurrentState == Node.State.Block)
                        {
                            DrawGizmoCube(node.Pos, _blockNodeColor, _nodeSize);
                        }

                        if (_showSurface == true && node.CanStep == true)
                        {
                            DrawGizmoCube(node.SurfacePos, _surfaceNodeColor, new Vector3(_nodeSize, _nodeSize * _surfaceHeight, _nodeSize));
                        }
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            DrawGrid();
        }
    }
}
