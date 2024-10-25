using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveComponent : MonoBehaviour
{
    Rigidbody _rigidbody;
    Func<Vector3, Vector3, List<Vector3>> FindPath;

    float _findDelay = 0.5f;
    Timer _pathFindTimer;
    bool _onAir;

    List<Vector3> _path;
    int _pathIndex = 0;
    const float _reachDistance = 0.5f;
    float _moveSpeed = 5f;

    public void Initialize(Func<Vector3, Vector3, List<Vector3>> FindPath, bool onAir, float moveSpeed)
    {
        this.FindPath = FindPath;
        _onAir = onAir;
        _moveSpeed = moveSpeed;

        _rigidbody = GetComponent<Rigidbody>();
        _pathFindTimer = new Timer();
    }

    private void OnDrawGizmos()
    {
        if (_path == null) return;

        for (int i = 1; i < _path.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_path[i - 1], _path[i]);
        }
    }

    public void TeleportTo(Vector3 pos)
    {
        _rigidbody.position = pos;
    }

    public void Stop()
    {
        _rigidbody.velocity = Vector3.zero;
    }

    public void Move(Vector3 targetPos)
    {
        if(_pathFindTimer.CurrentState != Timer.State.Running)
        {
            // Stopwatch 객체 생성 및 시작
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();

            _path = FindPath(transform.position, targetPos);

            //stopwatch.Stop(); // Stopwatch 중지
            //print($"코드 실행 시간: {stopwatch.ElapsedMilliseconds} ms");  // 결과 출력

            _pathIndex = 0;

            _pathFindTimer.Reset();
            _pathFindTimer.Start(_findDelay);
        }

        if (_path == null || _pathIndex >= _path.Count - 1)
        {
            Stop();
            return;
        }
        // 경로가 없거나 경로 끝에 도달한 경우 진행하지 않음

        float distance = Vector3.Distance(transform.position, _path[_pathIndex]);
        if (distance < _reachDistance) _pathIndex++;

        Vector3 direction = (_path[_pathIndex] - transform.position).normalized;
        _rigidbody.velocity = direction * _moveSpeed;
    }
}
