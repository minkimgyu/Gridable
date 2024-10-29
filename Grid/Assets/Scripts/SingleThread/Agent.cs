using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MultiThreadPathfinding;


namespace SingleThreadPathfinding
{
    public class Agent : MonoBehaviour
    {
        List<Vector3> _path;
        int _pathIndex = 0;
        const float _reachDistance = 0.5f;

        const float _endDistance = 3f;
        float _moveSpeed = 5f;

        Vector3 _endPos;

        Func<Vector3> ReturnRandomStartPos;
        Func<Vector3> ReturnRandomEndPos;

        Func<Vector3, Vector3, List<Vector3>> FindPath;

        public void Initialize(Func<Vector3, Vector3, List<Vector3>> FindPath, Func<Vector3> ReturnRandomStartPos, Func<Vector3> ReturnRandomEndPos)
        {
            this.FindPath = FindPath;
            this.ReturnRandomStartPos = ReturnRandomStartPos;
            this.ReturnRandomEndPos = ReturnRandomEndPos;

            Vector3 randomPos = ReturnRandomStartPos();
            TeleportTo(randomPos);

            _endPos = ReturnRandomEndPos();

            FinishPath();
        }

        void TeleportTo(Vector3 pos)
        {
            transform.position = pos;
        }

        void FinishPath()
        {
            _path = null;
            Vector3 randomPos = ReturnRandomStartPos();
            Vector3 endPos;

            do
            {
                endPos = ReturnRandomEndPos();
            }
            while (endPos == _endPos);

            _endPos = endPos;
            TeleportTo(randomPos);
            _path = FindPath(randomPos, _endPos);
            _pathIndex = 1;
        }


        private void Update()
        {
            if (Vector3.Distance(transform.position, _endPos) <= _endDistance)
            {
                FinishPath();
            }

            if (_path == null || _pathIndex >= _path.Count - 1) return;
            // 경로가 없거나 경로 끝에 도달한 경우 진행하지 않음

            float distance = Vector3.Distance(transform.position, _path[_pathIndex]);
            if (distance < _reachDistance) _pathIndex++;

            transform.position = Vector3.MoveTowards(transform.position, _path[_pathIndex], Time.deltaTime * _moveSpeed);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(_endPos, Vector3.one);

            if (_path == null) return;

            for (int i = 1; i < _path.Count; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_path[i - 1], _path[i]);
            }
        }
    }
}