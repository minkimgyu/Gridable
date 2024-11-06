using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFieldPathfinding
{
    public class Node : INode<Node>
    {
        public enum State
        {
            Empty,
            Block,
            NonPass,
        }

        public Node(Vector3 pos, State state)
        {
            _pos = pos;
            _state = state;

            _haveSurface = false;
            _surfacePos = Vector3.zero;
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


        public byte Weight
        {
            get
            {
                if(CanStep == true)
                {
                    return 1;
                }
                else
                {
                    return 255;
                }
            }
        }

        Vector3 directionToMove;
        public Vector3 DirectionToMove { get { return directionToMove; } set { directionToMove = value; } } // 노드의 방향성

        float _pathWeight; // 다익스트라로 생성되는 가중치
        public float PathWeight { get { return _pathWeight; } set { _pathWeight = value; } }


        // 만약 해당 위치로 이동할 때 노드가 Block인 경우 이 노드 사용
        // 만약 이 노드도 사용 불가능하다면 새로운 노드를 찾아야 한다.
        public Node AlternativeNode { get; set; } // 대체할 노드
        public List<Node> NearNodesInGround { get; set; }
        public List<Node> NearNodes { get; set; }


        public int CompareTo(Node other)
        {
            int compareValue = _pathWeight.CompareTo(other._pathWeight);
            return compareValue;
        }
    }
}