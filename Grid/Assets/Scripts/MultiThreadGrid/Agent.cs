using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Agent : MonoBehaviour
{
    Timer _pathFindTimer;

    List<Vector3> _path;
    int _pathIndex = 0;
    const float _reachDistance = 0.5f;
    float _moveSpeed = 5f;

    float _findDelay = 0.5f;

    Vector3 _endPos;

    Func<Vector3> ReturnRandomStartPos;
    Func<Vector3> ReturnRandomEndPos;

    public void Initialize(Func<Vector3> ReturnRandomStartPos, Func<Vector3> ReturnRandomEndPos)
    {
        _pathFindTimer = new Timer();
        this.ReturnRandomStartPos = ReturnRandomStartPos;
        this.ReturnRandomEndPos = ReturnRandomEndPos;

        Vector3 randomPos = ReturnRandomStartPos();
        TeleportTo(randomPos);

        _endPos = ReturnRandomEndPos();
    }

    void TeleportTo(Vector3 pos)
    {
        transform.position = pos;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, _endPos) <= 3f)
        {
            _path = null;
            Vector3 randomPos = ReturnRandomStartPos();
            _endPos = ReturnRandomEndPos();

            TeleportTo(randomPos);
            return;
        }

        if (_pathFindTimer.CurrentState != Timer.State.Running)
        {
            GridMaster.GridBase.GetInstance().RequestPathfind(transform.position, _endPos, (path) => { _path = path; });

            _pathIndex = 1;
            _pathFindTimer.Reset();
            _pathFindTimer.Start(_findDelay);
        }

        if (_path == null || _pathIndex >= _path.Count - 1)
        {
            return;
        }
        // 경로가 없거나 경로 끝에 도달한 경우 진행하지 않음

        float distance = Vector3.Distance(transform.position, _path[_pathIndex]);
        if (distance < _reachDistance) _pathIndex++;

        transform.position = Vector3.MoveTowards(transform.position, _path[_pathIndex], Time.deltaTime * _moveSpeed);
    }

    private void OnDrawGizmos()
    {
        if (_path == null) return;

        for (int i = 1; i < _path.Count; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_path[i - 1], _path[i]);
        }
    }
}
