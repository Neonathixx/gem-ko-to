using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Load by name
    public void LoadByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Load by build index
    public void LoadByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }

    // Load without freezing the game (good for loading screens)
    public void LoadAsync(string sceneName)
    {
        StartCoroutine(LoadRoutine(sceneName));
    }

    private System.Collections.IEnumerator LoadRoutine(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            Debug.Log($"Loading: {progress * 100f}%");
            yield return null;
        }
    }

    // Reload the current scene
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}