using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    [SerializeField] GroundUnit _unitPrefab;

    [SerializeField] Transform[] _startPoints;
    [SerializeField] Transform[] _endPoints;
    [SerializeField] int _spawnCount = 100;

    [SerializeField] GridComponent _gridComponent;
    [SerializeField] GroundPathfinder _pathfinder;
    
    private void Start()
    {
        _gridComponent.Initialize();

        for (int i = 0; i < _spawnCount; i++)
        {
            GroundUnit unit = Instantiate(_unitPrefab);
            unit.Initialize(_pathfinder.FindPath, ReturnRandomStartPos, ReturnRandomEndPos);
        }
    }

    Vector3 ReturnRandomStartPos()
    {
        return _startPoints[Random.Range(0, _startPoints.Length)].position;
    }

    Vector3 ReturnRandomEndPos()
    {
        return _endPoints[Random.Range(0, _endPoints.Length)].position;
    }
}
