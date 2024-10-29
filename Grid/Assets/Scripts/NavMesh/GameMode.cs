using SingleThreadPathfinding;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NavmeshPathfinding
{
    public class GameMode : MonoBehaviour
    {
        [SerializeField] Agent _agentPrefab;

        [SerializeField] Transform[] _startPoints;
        [SerializeField] Transform[] _endPoints;

        [SerializeField] Button _exitBtn;
        [SerializeField] TMP_Text _spawnCountTxt;

        private void Start()
        {
            _exitBtn.onClick.AddListener(() => { SceneManager.LoadScene("StartScene"); });

            int spawnCount = PlayerPrefs.GetInt("SpawnCount");
            _spawnCountTxt.text = $"{spawnCount} Count";

            for (int i = 0; i < spawnCount; i++)
            {
                Agent agent = Instantiate(_agentPrefab);
                agent.Initialize(ReturnRandomStartPos, ReturnRandomEndPos);
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