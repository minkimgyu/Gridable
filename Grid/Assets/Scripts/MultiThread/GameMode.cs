using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace MultiThreadPathfinding
{
    public class GameMode : MonoBehaviour
    {
        [SerializeField] Agent _agentPrefab;

        [SerializeField] Transform[] _startPoints;
        [SerializeField] Transform[] _endPoints;

        [SerializeField] GridComponent _gridComponent;
        [SerializeField] PathRequestManager _pathRequestManager;

        [SerializeField] Button _exitBtn;
        [SerializeField] TMP_Text _spawnCountTxt;

        private void Start()
        {
            _exitBtn.onClick.AddListener(() => { SceneManager.LoadScene("StartScene"); });

            int spawnCount = PlayerPrefs.GetInt("SpawnCount");
            _spawnCountTxt.text = $"{spawnCount} Count";

            _gridComponent.Initialize();
            _pathRequestManager.Initialize(_gridComponent);

            for (int i = 0; i < spawnCount; i++)
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