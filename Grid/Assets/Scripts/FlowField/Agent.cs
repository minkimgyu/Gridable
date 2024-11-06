using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FlowFieldPathfinding
{
    public class Agent : MonoBehaviour
    {
        PathSeeker _pathSeeker;
        float _moveSpeed = 5f;

        Vector3 _endPos;

        Func<Vector3> ReturnRandomStartPos;
        Func<Vector3> ReturnRandomEndPos;

        public void Initialize(GridComponent gridComponent, Func<Vector3> ReturnRandomStartPos, Func<Vector3> ReturnRandomEndPos)
        {
            //this.FindPath = FindPath;
            this.ReturnRandomStartPos = ReturnRandomStartPos;
            this.ReturnRandomEndPos = ReturnRandomEndPos;

            _pathSeeker = GetComponent<PathSeeker>();
            _pathSeeker.Initialize(gridComponent);

            Vector3 randomPos = ReturnRandomStartPos();
            TeleportTo(randomPos);

            FinishPath();
        }

        public void FinishPath()
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
        }

        void TeleportTo(Vector3 pos)
        {
            transform.position = pos;
        }

        private void Update()
        {
            Vector3 direction = _pathSeeker.ReturnDirection();
            transform.Translate(direction * _moveSpeed * Time.deltaTime);
        }
    }
}