using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlowFieldPathfinding
{
    public class GridComponent : MonoBehaviour
    {
        // �ֺ� ��带 ��ȯ�ϴ� �ڵ带 �ۼ��Ѵ�.

        // ��� ���� �� ������ �̵��� �� �ִ� ���
        // ���� ������ ǥ���� ���� �̵��� �� �ִ� ����, �����ڷ� ������.
        // ��� �����¿� 1ĭ�� �̵������ϰԲ� �غ���
        GridGenerator _gridGenerator;
        GroundPathfinder _groundPathfinder;

        [SerializeField] int _agentHeight = 1;
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
        [SerializeField] ShowType _showType;

        public enum Direction
        {
            UpLeft,
            Up,
            UpRight,
            Left,
            Current,
            Right,
            DownLeft,
            Down,
            DownRight
        }

        public Dictionary<Vector3, Direction> directions = new Dictionary<Vector3, Direction>()
        {
            { new Vector3(-1, 0, 1), Direction.UpLeft },
            { new Vector3(-1, 1, 1), Direction.UpLeft },
            { new Vector3(-1, -1, 1), Direction.UpLeft },

            { new Vector3(0, 0, 1), Direction.Up },
            { new Vector3(0, 1, 1), Direction.Up },
            { new Vector3(0, -1, 1), Direction.Up },

            { new Vector3(1, 0, 1), Direction.UpRight },
            { new Vector3(1, 1, 1), Direction.UpRight },
            { new Vector3(1, -1, 1), Direction.UpRight },

            { new Vector3(-1, 0, 0), Direction.Left },
            { new Vector3(-1, 1, 0), Direction.Left },
            { new Vector3(-1, -1, 0), Direction.Left },


            { new Vector3(1, 0, 0), Direction.Right },
            { new Vector3(1, -1, 0), Direction.Right },
            { new Vector3(1, 1, 0), Direction.Right },

            { new Vector3(0, 0, 0), Direction.Current },

            { new Vector3(-1, 0, -1), Direction.DownLeft },
            { new Vector3(-1, -1, -1), Direction.DownLeft },
            { new Vector3(-1, 1, -1), Direction.DownLeft },

            { new Vector3(0, 0, -1), Direction.Down },
            { new Vector3(0, 1, -1), Direction.Down },
            { new Vector3(0, -1, -1), Direction.Down },

            { new Vector3(1, 0, -1), Direction.DownRight },
            { new Vector3(1, -1, -1), Direction.DownRight },
            { new Vector3(1, 1, -1), Direction.DownRight },
        };

        public enum ShowType
        {
            Block,
            Weight,
            Direction
        }

        Node[,,] _grid;

        public void Initialize()
        {
            _showType = ShowType.Direction;

            blockStyle.fontSize = 20;
            blockStyle.alignment = TextAnchor.MiddleCenter;
            blockStyle.normal.textColor = Color.red;

            emptyStyle.fontSize = 20;
            emptyStyle.alignment = TextAnchor.MiddleCenter;
            emptyStyle.normal.textColor = Color.blue;

            weightStyle.fontSize = 5;
            weightStyle.alignment = TextAnchor.MiddleCenter;
            weightStyle.normal.textColor = Color.white;

            startPointStyle.fontSize = 20;
            startPointStyle.alignment = TextAnchor.MiddleCenter;
            startPointStyle.normal.textColor = Color.white;

            _gridGenerator = GetComponent<GridGenerator>();
            _groundPathfinder = GetComponent<GroundPathfinder>();

            _grid = _gridGenerator.CreateGrid(_agentHeight, _nodeSize, _sizeOfGrid, _blockMask, _nonPassMask);
            for (int x = 0; x < _sizeOfGrid.x; x++)
            {
                for (int y = 0; y < _sizeOfGrid.y; y++)
                {
                    for (int z = 0; z < _sizeOfGrid.z; z++)
                    {
                        _grid[x, y, z].NearNodesInGround = ReturnNearNodesInGround(new Vector3Int(x, y, z));
                        //_grid[x, y, z].NearNodes = ReturnNearNodes(new Vector3Int(x, y, z));
                    }
                }
            }

            _groundPathfinder.Initialize(this);
        }

        public Vector3 ReturnNodeDirection(Vector3 worldPos)
        {
            Vector3Int index = ReturnNodeIndex(worldPos);
            Node node = ReturnNode(index);
            return node.DirectionToMove;
        }

        public void CalculateNodePath(Node startNode)
        {
            for (int i = 0; i < _sizeOfGrid.x; i++)
            {
                for (int k = 0; k < _sizeOfGrid.z; k++)
                {
                    for (int j = 0; j < _sizeOfGrid.y; j++)
                    {
                        if (startNode == _grid[i, j, k])
                        {
                            startNode.DirectionToMove = Vector3.zero;
                            continue;
                        }

                        List<Node> nearNodes = _grid[i, j, k].NearNodesInGround;
                        if (nearNodes.Count == 0) continue;

                        float minWeight = float.MaxValue;
                        int minIndex = 0;
                        for (int z = 0; z < nearNodes.Count; z++)
                        {
                            if (nearNodes[z].PathWeight < minWeight)
                            {
                                minWeight = nearNodes[z].PathWeight;
                                minIndex = z;
                            }
                        }

                        Vector3 direction = nearNodes[minIndex].Pos - _grid[i, j, k].Pos;
                        _grid[i, j, k].DirectionToMove = direction;
                    }
                }
            }
        }

        public void ResetNodeWeight()
        {
            for (int i = 0; i < _sizeOfGrid.x; i++)
            {
                for (int j = 0; j < _sizeOfGrid.y; j++)
                {
                    for (int k = 0; k < _sizeOfGrid.z; k++)
                    {
                        _grid[i, j, k].PathWeight = float.MaxValue;
                    }
                }
            }
        }

        public List<Node> ReturnNearNodesInGround(Vector3Int index)
        {
            List<Node> nearNodes = new List<Node>();

            // y�� ������ ���̰� �ִ� ���
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


            // y�� ���̰� ���� �ֺ� �׸��� �� �� �� �� �� ���
            //       (0)
            //        �� 
            // (1) ��  �� �� (2)
            //        �� 
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


            // y�� ���̰� ���� �ֺ� �׸��� �� �� �� �� �� ���
            // (0)      (1)
            //   ��    ��
            //      ��
            //   ��    �� 
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

                // �� �� �ִ� �ڳ����� üũ
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

        //public List<Node> ReturnNearNodes(Vector3Int index)
        //{
        //    List<Node> nearNodes = new List<Node>();

        //    // �ֺ� �׸���
        //    List<Vector3Int> closeIndex = new List<Vector3Int> {
        //    new Vector3Int(index.x - 1, index.y - 1, index.z + 1), new Vector3Int(index.x, index.y - 1, index.z + 1), new Vector3Int(index.x + 1, index.y - 1, index.z + 1),
        //    new Vector3Int(index.x - 1, index.y - 1, index.z), new Vector3Int(index.x, index.y - 1, index.z), new Vector3Int(index.x + 1, index.y - 1, index.z),
        //    new Vector3Int(index.x - 1, index.y - 1, index.z - 1), new Vector3Int(index.x, index.y - 1, index.z - 1), new Vector3Int(index.x + 1, index.y - 1, index.z - 1),

        //    new Vector3Int(index.x - 1, index.y, index.z + 1), new Vector3Int(index.x, index.y, index.z + 1), new Vector3Int(index.x + 1, index.y, index.z + 1),
        //    new Vector3Int(index.x - 1, index.y, index.z), new Vector3Int(index.x + 1, index.y, index.z),
        //    new Vector3Int(index.x - 1, index.y, index.z - 1), new Vector3Int(index.x, index.y, index.z - 1), new Vector3Int(index.x + 1, index.y, index.z - 1),

        //    new Vector3Int(index.x - 1, index.y + 1, index.z + 1), new Vector3Int(index.x, index.y + 1, index.z + 1), new Vector3Int(index.x + 1, index.y + 1, index.z + 1),
        //    new Vector3Int(index.x - 1, index.y + 1, index.z), new Vector3Int(index.x, index.y + 1, index.z), new Vector3Int(index.x + 1, index.y + 1, index.z),
        //    new Vector3Int(index.x - 1, index.y + 1, index.z - 1), new Vector3Int(index.x, index.y + 1, index.z - 1), new Vector3Int(index.x + 1, index.y + 1, index.z - 1)
        //};

        //    for (int i = 0; i < closeIndex.Count; i++)
        //    {
        //        bool isOutOfRange = IsOutOfRange(closeIndex[i]);
        //        if (isOutOfRange == true) continue;

        //        Node node = ReturnNode(closeIndex[i]);
        //        nearNodes.Add(node);
        //    }

        //    return nearNodes;
        //}

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
            Vector3 topRightPos = ReturnNode(_sizeOfGrid.x - 1, _sizeOfGrid.y - 1, _sizeOfGrid.z - 1).Pos; // --> ������ ��ġ�� ����� ũ�⸦ ������� �Ѵ�.

            // �ݿø��ϰ� ���� �ȿ� ������
            // �� �κ��� GridSize �ٲ�� �����ؾ���
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

        // ��Ÿ�� ����
        GUIStyle blockStyle = new GUIStyle();
        GUIStyle emptyStyle = new GUIStyle();
        GUIStyle weightStyle = new GUIStyle();

        GUIStyle startPointStyle = new GUIStyle();

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            DrawGrid();

            if (_grid == null) return;

            for (int i = 0; i < _sizeOfGrid.x; i++)
            {
                for (int j = 0; j < _sizeOfGrid.y; j++)
                {
                    for (int k = 0; k < _sizeOfGrid.z; k++)
                    {
                        if (_grid[i, j, k].CanStep == false) continue;

                        if (_showType == ShowType.Direction)
                        {
                            if (_grid[i, j, k].CanStep == false)
                            {
                                Handles.Label(_grid[i, j, k].Pos, "X", blockStyle);
                            }
                            else
                            {
                                bool containDirection = directions.ContainsKey(_grid[i, j, k].DirectionToMove);
                                if (containDirection == false) continue;

                                Direction direction = directions[_grid[i, j, k].DirectionToMove];
                                switch (direction)
                                {
                                    case Direction.UpLeft:
                                        Handles.Label(_grid[i, j, k].Pos, "��", emptyStyle);
                                        break;
                                    case Direction.Up:
                                        Handles.Label(_grid[i, j, k].Pos, "��", emptyStyle);
                                        break;
                                    case Direction.UpRight:
                                        Handles.Label(_grid[i, j, k].Pos, "��", emptyStyle);
                                        break;
                                    case Direction.Left:
                                        Handles.Label(_grid[i, j, k].Pos, "��", emptyStyle);
                                        break;
                                    case Direction.Current:
                                        Handles.Label(_grid[i, j, k].Pos, "��", startPointStyle);
                                        break;
                                    case Direction.Right:
                                        Handles.Label(_grid[i, j, k].Pos, "��", emptyStyle);
                                        break;
                                    case Direction.DownLeft:
                                        Handles.Label(_grid[i, j, k].Pos, "��", emptyStyle);
                                        break;
                                    case Direction.Down:
                                        Handles.Label(_grid[i, j, k].Pos, "��", emptyStyle);
                                        break;
                                    case Direction.DownRight:
                                        Handles.Label(_grid[i, j, k].Pos, "��", emptyStyle);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        else if (_showType == ShowType.Block)
                        {
                            switch (_grid[i, j, k].CurrentState)
                            {
                                case Node.State.Empty:
                                    Handles.Label(_grid[i, j, k].Pos, "��", emptyStyle);
                                    break;
                                case Node.State.Block:
                                    Handles.Label(_grid[i, j, k].Pos, "X", blockStyle);
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (_showType == ShowType.Weight)
                        {
                            switch (_grid[i, j, k].CurrentState)
                            {
                                case Node.State.Empty:
                                    Handles.Label(_grid[i, j, k].Pos, _grid[i, j, k].PathWeight.ToString(), emptyStyle);
                                    break;
                                case Node.State.Block:
                                    Handles.Label(_grid[i, j, k].Pos, _grid[i, j, k].PathWeight.ToString(), blockStyle);
                                    break;
                                default:
                                    break;
                            }
                        }
                    } 
                }
            }
#endif
        }
    }
}