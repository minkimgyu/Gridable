using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NavmeshPathfinding
{
    public class Agent : MonoBehaviour
    {
        NavMeshAgent agent;

        const float _reachDistance = 0.5f;
        const float _endDistance = 3f;
        float _moveSpeed = 5f;

        Vector3 _endPos;

        Func<Vector3> ReturnRandomStartPos;
        Func<Vector3> ReturnRandomEndPos;

        void TeleportTo(Vector3 pos)
        {
            transform.position = pos;
        }

        public void Initialize(Func<Vector3> ReturnRandomStartPos, Func<Vector3> ReturnRandomEndPos)
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = _moveSpeed;

            this.ReturnRandomStartPos = ReturnRandomStartPos;
            this.ReturnRandomEndPos = ReturnRandomEndPos;

            Vector3 randomPos = ReturnRandomStartPos();
            TeleportTo(randomPos);

            _endPos = ReturnRandomEndPos();

            FinishPath();
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
        }

        private void Update()
        {
            if (Vector3.Distance(transform.position, _endPos) <= _endDistance)
            {
                FinishPath();
            }

            agent.SetDestination(_endPos);
        }
    }

}