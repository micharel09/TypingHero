using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreBootstrap_ResetHooks : MonoBehaviour
{
    [SerializeField] KeyCode restartKey = KeyCode.R;
    [SerializeField] bool listenSceneLoaded = true;

    void OnEnable()
    {
        if (listenSceneLoaded)
            SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        if (listenSceneLoaded)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void Update()
    {
        if (Input.GetKeyDown(restartKey))
        {
            ScoreSystem.I?.ResetScore();
            // nếu bạn đã có code reload scene ở nơi khác thì bỏ dòng dưới
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    void OnSceneLoaded(Scene _, LoadSceneMode __)
    {
        ScoreSystem.I?.ResetScore();
    }
}
