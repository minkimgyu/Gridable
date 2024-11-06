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

        Vector3 _pos; // �׸��� ���� ��ġ
        public Vector3 Pos { get { return _pos; } }


        bool _haveSurface;
        public bool HaveSurface { set { _haveSurface = value; } }

        Vector3 _surfacePos; // ���� ���� �� �ִ� ǥ�� ��ġ
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
        public Vector3 DirectionToMove { get { return directionToMove; } set { directionToMove = value; } } // ����� ���⼺

        float _pathWeight; // ���ͽ�Ʈ��� �����Ǵ� ����ġ
        public float PathWeight { get { return _pathWeight; } set { _pathWeight = value; } }


        // ���� �ش� ��ġ�� �̵��� �� ��尡 Block�� ��� �� ��� ���
        // ���� �� ��嵵 ��� �Ұ����ϴٸ� ���ο� ��带 ã�ƾ� �Ѵ�.
        public Node AlternativeNode { get; set; } // ��ü�� ���
        public List<Node> NearNodesInGround { get; set; }
        public List<Node> NearNodes { get; set; }


        public int CompareTo(Node other)
        {
            int compareValue = _pathWeight.CompareTo(other._pathWeight);
            return compareValue;
        }
    }
}