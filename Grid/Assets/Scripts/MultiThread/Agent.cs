using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MultiThreadPathfinding
{
    public class Agent : MonoBehaviour
    {
        PathResult _pathResult;
        //List<Vector3> _path;
        int _pathIndex = 0;
        const float _reachDistance = 0.5f;

        const float _endDistance = 3f;
        float _moveSpeed = 5f;
        [SerializeField] int _safeRange = 150;

        Vector3 _endPos;

        Pathfinder _pathfinder;

        Func<Vector3> ReturnRandomStartPos;
        Func<Vector3> ReturnRandomEndPos;

        public void Initialize(Pathfinder pathfinder, Func<Vector3> ReturnRandomStartPos, Func<Vector3> ReturnRandomEndPos)
        {
            _pathfinder = pathfinder;
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
            Vector3 randomPos = ReturnRandomStartPos();
            Vector3 endPos;

            do
            {
                endPos = ReturnRandomEndPos();
            }
            while (endPos == _endPos);

            _endPos = endPos;
            TeleportTo(randomPos);
            FindPath(randomPos);
        }

        async void FindPath(Vector3 startPos)
        {
            PathRequest request = new PathRequest(startPos, _endPos, _safeRange);
            _pathResult = await Task<PathResult>.Run(() => 
            { 
                _pathIndex = 0; 
                return _pathfinder.FindPath(request); 
            });

            //_pathResult = _pathfinder.FindPath();
            //_pathResult = await _pathfinder.FindPath(new PathRequest(startPos, _endPos, safeRange));
        }


        private void Update()
        {
            if (Vector3.Distance(transform.position, _endPos) <= _endDistance)
            {
                FinishPath();
            }

            if (_pathResult.path == null || _pathIndex >= _pathResult.path.Count - 1) return;
            // 경로가 없거나 경로 끝에 도달한 경우 진행하지 않음

            float distance = Vector3.Distance(transform.position, _pathResult.path[_pathIndex]);
            if (distance < _reachDistance) _pathIndex++;

            transform.position = Vector3.MoveTowards(transform.position, _pathResult.path[_pathIndex], Time.deltaTime * _moveSpeed);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(_endPos, Vector3.one);

            if (_pathResult.path == null) return;

            for (int i = 1; i < _pathResult.path.Count; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_pathResult.path[i - 1], _pathResult.path[i]);
            }
        }
    }
}