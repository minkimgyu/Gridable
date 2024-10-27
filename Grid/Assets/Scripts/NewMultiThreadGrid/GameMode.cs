using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gridable
{
    public class GameMode : MonoBehaviour
    {
        [SerializeField] Agent _agentPrefab;

        [SerializeField] Transform[] _startPoints;
        [SerializeField] Transform[] _endPoints;
        [SerializeField] int _spawnCount = 100;

        private void Start()
        {
            for (int i = 0; i < _spawnCount; i++)
            {
                Agent unit = Instantiate(_agentPrefab);
                unit.Initialize(ReturnRandomStartPos, ReturnRandomEndPos);
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
}