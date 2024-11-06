using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FlowFieldPathfinding
{
    public class Agent : MonoBehaviour
    {
        PathSeeker _pathSeeker;

        const float _endDistance = 3f;
        float _moveSpeed = 5f;

        Vector3 _endPos;

        Func<Vector3> ReturnRandomStartPos;

        public void Initialize(GridComponent gridComponent, Vector3 endPos, Func<Vector3> ReturnRandomStartPos)
        {
            //this.FindPath = FindPath;
            this.ReturnRandomStartPos = ReturnRandomStartPos;

            _pathSeeker = GetComponent<PathSeeker>();
            _pathSeeker.Initialize(gridComponent);

            Vector3 randomPos = ReturnRandomStartPos();
            TeleportTo(randomPos);

            _endPos = endPos;

            FinishPath();
        }
        void FinishPath()
        {
            Vector3 randomPos = ReturnRandomStartPos();
            TeleportTo(randomPos);
        }

        void TeleportTo(Vector3 pos)
        {
            transform.position = pos;
        }

        private void Update()
        {
            if (Vector3.Distance(transform.position, _endPos) <= _endDistance)
            {
                FinishPath();
            }

            Vector3 direction = _pathSeeker.ReturnDirection();
            transform.Translate(direction * _moveSpeed * Time.deltaTime);
        }
    }
}