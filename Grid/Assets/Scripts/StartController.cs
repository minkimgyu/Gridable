using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StartController : MonoBehaviour
{
    [SerializeField] Button _exitBtn;

    [SerializeField] Button _navmeshThread;
    [SerializeField] Button _singleThread;
    [SerializeField] Button _multiThread;
    [SerializeField] TMP_InputField _spawnCountInput;

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        int spawnCount = PlayerPrefs.GetInt("SpawnCount", 100);
        _spawnCountInput.text = spawnCount.ToString();

        _singleThread.onClick.AddListener(() => { LoadScene("SingleThread"); });
        _multiThread.onClick.AddListener(() => { LoadScene("MultiThread"); });
        _navmeshThread.onClick.AddListener(() => { LoadScene("NavMesh"); });

        _exitBtn.onClick.AddListener(() => { Quit(); });
    }

    void LoadScene(string name)
    {
        PlayerPrefs.SetInt("SpawnCount", int.Parse(_spawnCountInput.text));
        SceneManager.LoadScene(name);
    }
}
