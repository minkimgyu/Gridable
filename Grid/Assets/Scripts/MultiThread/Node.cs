using UnityEngine;
using System.Collections;

namespace MultiThreadPathfinding
{
    public struct Node : INode<Node>
    {
        public enum State
        {
            Empty,
            Block,
            NonPass,
        }

        State _state;
        public State CurrentState { get { return _state; } }

        Vector3 _pos; // 그리드 실제 위치
        public Vector3 Pos { get { return _pos; } }


        bool _haveSurface;
        public bool HaveSurface { set { _haveSurface = value; } }

        Vector3 _surfacePos; // 발을 딛을 수 있는 표면 위치
        public Vector3 SurfacePos { set { _surfacePos = value; } get { return _surfacePos; } }

        public bool CanStep { get { return _state == State.Block && _haveSurface == true; } }

        float g, h;
        public float G { get { return g; } set { g = value; } }
        public float H { get { return h; } set { h = value; } }
        public float F { get { return g + h; } }

        public Node(Vector3 pos, Vector3Int nodeIndex, State state)
        {
            _pos = pos;
            _nodeIndex = nodeIndex;
            _state = state;

            _haveSurface = false;
            _surfacePos = Vector3.zero;
            g = 0;
            h = 0;
        }

        public Node(Vector3 pos, Vector3Int nodeIndex, State state, float hCost, float gCost) : this(pos, nodeIndex, state)
        {
            g = gCost;
            h = hCost;
        }

        Vector3Int _nodeIndex;
        public Vector3Int NodeIndex { get { return _nodeIndex; } }

        public int CompareTo(Node other)
        {
            int compareValue = F.CompareTo(other.F);
            if (compareValue == 0) compareValue = H.CompareTo(other.H);
            return compareValue;
        }
    }
}
