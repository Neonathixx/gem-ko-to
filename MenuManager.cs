using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central brain for the Menu scene.
/// The three token buttons call into this; it does not care how they were clicked.
/// </summary>
public class MenuManager : MonoBehaviour
{
    [Header("Scene To Load")]
    [Tooltip("Must match the scene file name exactly, and the scene must be in the Scene List.")]
    public string gameSceneName = "SampleScene";

    [Header("References")]
    [Tooltip("Drag the SceneLoader object from the Menu scene here. Only used if Use Fade is off.")]
    public SceneLoader sceneLoader;

    [Tooltip("The settings panel root. Starts hidden, toggled by the red token.")]
    public GameObject settingsPanel;

    [Header("Transition")]
    [Tooltip("Fade to black between scenes instead of cutting straight across.")]
    public bool useFade = true;

    [Tooltip("Fade out before quitting, so the window does not vanish mid-frame.")]
    public bool fadeBeforeQuit = true;

    private bool isLoading = false;

    private void Start()
    {
        // Settings should never be open when the menu first appears.
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (sceneLoader == null)
        {
            sceneLoader = FindFirstObjectByType<SceneLoader>();
        }

        // Create the fade overlay up front so the first click is not delayed.
        if (useFade)
        {
            SceneTransition.Ensure();
        }
    }

    /// <summary>
    /// True while a load or fade is running. MenuButton checks this
    /// so tokens cannot be clicked during a transition.
    /// </summary>
    public bool IsBusy()
    {
        return isLoading || SceneTransition.IsTransitioning;
    }

    // ---------- YELLOW TOKEN ----------
    public void StartGame()
    {
        if (IsBusy())
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(gameSceneName))
        {
            Debug.LogError("MenuManager: gameSceneName is empty.", this);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            Debug.LogError(
                "MenuManager: the scene '" + gameSceneName +
                "' is not in the Scene List (File > Build Profiles). " +
                "Add it or the game will not start.",
                this
            );
            return;
        }

        isLoading = true;
        Debug.Log("Starting game: loading " + gameSceneName);

        // Make sure nothing is left paused from a previous session.
        Time.timeScale = 1f;

        if (useFade)
        {
            SceneTransition.Ensure().FadeToScene(gameSceneName);
        }
        else if (sceneLoader != null)
        {
            sceneLoader.LoadAsync(gameSceneName);
        }
        else
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }

    // ---------- RED TOKEN ----------
    public void ToggleSettings()
    {
        if (IsBusy())
        {
            return;
        }

        if (settingsPanel == null)
        {
            Debug.LogWarning(
                "MenuManager: no settings panel assigned, so the settings token does nothing yet.",
                this
            );
            return;
        }

        bool nowOpen = !settingsPanel.activeSelf;
        settingsPanel.SetActive(nowOpen);

        Debug.Log("Settings panel " + (nowOpen ? "opened." : "closed."));
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public bool SettingsAreOpen()
    {
        return settingsPanel != null && settingsPanel.activeSelf;
    }

    // ---------- PURPLE TOKEN ----------
    public void QuitGame()
    {
        if (IsBusy())
        {
            return;
        }

        Debug.Log("Quit requested.");

        if (fadeBeforeQuit && useFade)
        {
            StartCoroutine(QuitAfterFade());
        }
        else
        {
            DoQuit();
        }
    }

    private IEnumerator QuitAfterFade()
    {
        isLoading = true;
        yield return SceneTransition.Ensure().FadeOut();
        DoQuit();
    }

    private void DoQuit()
    {
#if UNITY_EDITOR
        // Application.Quit() is ignored in the Editor, so stop Play Mode instead.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}