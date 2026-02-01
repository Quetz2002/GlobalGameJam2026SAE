using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void GameOver()
    {
        EndGame();
    }

    public void WinGame()
    {
        StartCoroutine(LoadSceneRoutine(2, 3));
    }

    public void RestartGame()
    {
        StartCoroutine(LoadSceneRoutine(1, 1));
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadSceneRoutine(1, 0));
    }

    private void EndGame()
    {
        StartCoroutine(LoadSceneRoutine(2, 2));
    }

    public void ExitGame()
    {
        UnityEngine.Application.Quit();
    }

    IEnumerator LoadSceneRoutine(float waitTime, int sceneToLaod)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneToLaod);
        // Load your scene here
    }


}
