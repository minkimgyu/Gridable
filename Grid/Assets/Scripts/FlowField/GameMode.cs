using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FlowFieldPathfinding
{
    public class GameMode : MonoBehaviour
    {
        [SerializeField] Agent _agentPrefab;

        [SerializeField] Transform[] _startPoints;
        [SerializeField] Transform[] _endPoints;

        [SerializeField] GridComponent _gridComponent;
        [SerializeField] GroundPathfinder _pathfinder;

        [SerializeField] Button _exitBtn;
        [SerializeField] TMP_Text _spawnCountTxt;

        private void Start()
        {
            _exitBtn.onClick.AddListener(() => { SceneManager.LoadScene("StartScene"); });

            int spawnCount = PlayerPrefs.GetInt("SpawnCount");
            _spawnCountTxt.text = $"{spawnCount} Count";

            _gridComponent.Initialize();
            _pathfinder.FindPath(_endPoints[0].position);

            for (int i = 0; i < spawnCount; i++)
            {
                Agent agent = Instantiate(_agentPrefab);
                agent.Initialize(_gridComponent, _endPoints[0].position, ReturnRandomStartPos);
            }
        }

        Vector3 ReturnRandomStartPos()
        {
            return _startPoints[Random.Range(0, _startPoints.Length)].position;
        }
    }
}