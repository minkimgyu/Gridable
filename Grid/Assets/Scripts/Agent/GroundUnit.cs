using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GroundUnit : MonoBehaviour
{
    MoveComponent _moveComponent;

    Vector3 _endPos;

    Func<Vector3> ReturnRandomStartPos;
    Func<Vector3> ReturnRandomEndPos;
    [SerializeField] float _moveSpeed;

    public void Initialize(Func<Vector3, Vector3, List<Vector3>> FindPath, Func<Vector3> ReturnRandomStartPos, Func<Vector3> ReturnRandomEndPos)
    {
        _moveComponent = GetComponent<MoveComponent>();
        _moveComponent.Initialize(FindPath, false, _moveSpeed);

        this.ReturnRandomStartPos = ReturnRandomStartPos;
        this.ReturnRandomEndPos = ReturnRandomEndPos;

        Vector3 randomPos = ReturnRandomStartPos();
        TeleportTo(randomPos);

        _endPos = ReturnRandomEndPos();
    }

    void TeleportTo(Vector3 pos)
    {
        _moveComponent.TeleportTo(pos);
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, _endPos) < 2f)
        {
            Vector3 randomPos = ReturnRandomStartPos();
            TeleportTo(randomPos);

            _endPos = ReturnRandomEndPos();
            return;
        }

        _moveComponent.Move(_endPos);
    }
}
