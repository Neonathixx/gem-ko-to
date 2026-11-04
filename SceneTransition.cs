using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Fades the screen out, loads a scene, then fades back in.
/// Builds its own fullscreen overlay at runtime, so no Canvas is needed
/// in either scene. Survives scene loads via DontDestroyOnLoad.
///
/// Usage:  SceneTransition.Ensure().FadeToScene("SampleScene");
/// </summary>
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    /// <summary>True while a fade or load is in progress. Guard your buttons with this.</summary>
    public static bool IsTransitioning { get; private set; }

    [Header("Look")]
    public Color fadeColor = Color.black;

    [Header("Timing (seconds, unscaled)")]
    [Range(0.05f, 3f)] public float fadeOutDuration = 0.6f;
    [Range(0.05f, 3f)] public float fadeInDuration = 0.6f;
    [Tooltip("Extra beat on a fully black screen before the new scene appears.")]
    [Range(0f, 2f)] public float holdDuration = 0.15f;

    [Header("Feel")]
    [Tooltip("Ease the fade instead of running it linearly.")]
    public bool useSmoothing = true;

    private CanvasGroup group;
    private Image fadeImage;

    // ---------------------------------------------------------------
    // Setup
    // ---------------------------------------------------------------

    /// <summary>
    /// Returns the live instance, creating one if the scene has none.
    /// </summary>
    public static SceneTransition Ensure()
    {
        if (Instance != null)
        {
            return Instance;
        }

        SceneTransition found = FindFirstObjectByType<SceneTransition>();

        if (found != null)
        {
            return found;
        }

        GameObject host = new GameObject("SceneTransition");
        return host.AddComponent<SceneTransition>();
    }

    private void Awake()
    {
        // Only one may live. A second copy arriving from a newly loaded
        // scene destroys itself so the fade is never interrupted.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        BuildOverlay();
    }

    private void BuildOverlay()
    {
        GameObject canvasGO = new GameObject("FadeCanvas");
        canvasGO.transform.SetParent(transform, false);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32760; // sit above everything else

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        group = canvasGO.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform, false);

        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.color = fadeColor;
        fadeImage.raycastTarget = false;

        // Stretch to fill the whole screen, whatever the aspect ratio.
        RectTransform rect = fadeImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    // ---------------------------------------------------------------
    // Public API
    // ---------------------------------------------------------------

    /// <summary>Fade out, load the named scene, fade back in.</summary>
    public void FadeToScene(string sceneName)
    {
        if (IsTransitioning)
        {
            Debug.Log("SceneTransition: already mid-transition, ignoring request.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                "SceneTransition: '" + sceneName +
                "' is not in the Scene List (File > Build Profiles).",
                this
            );
            return;
        }

        StartCoroutine(TransitionRoutine(sceneName));
    }

    /// <summary>Fade out, load by build index, fade back in.</summary>
    public void FadeToScene(int buildIndex)
    {
        if (IsTransitioning)
        {
            return;
        }

        StartCoroutine(TransitionRoutine(null, buildIndex));
    }

    /// <summary>Fade to black and stay there. Useful before quitting.</summary>
    public Coroutine FadeOut()
    {
        return StartCoroutine(Fade(0f, 1f, fadeOutDuration));
    }

    // ---------------------------------------------------------------
    // Guts
    // ---------------------------------------------------------------

    private IEnumerator TransitionRoutine(string sceneName, int buildIndex = -1)
    {
        IsTransitioning = true;

        // Block clicks on anything underneath while the curtain is down.
        group.blocksRaycasts = true;

        // 1. Curtain down
        yield return Fade(0f, 1f, fadeOutDuration);

        // 2. Load while the screen is black. allowSceneActivation stays false
        //    so the swap cannot pop in before the fade finishes.
        AsyncOperation op = sceneName != null
            ? SceneManager.LoadSceneAsync(sceneName)
            : SceneManager.LoadSceneAsync(buildIndex);

        op.allowSceneActivation = false;

        // Unity stalls at 0.9 when activation is held back.
        while (op.progress < 0.9f)
        {
            yield return null;
        }

        if (holdDuration > 0f)
        {
            yield return new WaitForSecondsRealtime(holdDuration);
        }

        op.allowSceneActivation = true;

        while (!op.isDone)
        {
            yield return null;
        }

        // Give the new scene one frame to run Awake/Start and position things
        // before we reveal it, so the first frame is not a half-built scene.
        yield return null;

        // 3. Curtain up
        yield return Fade(1f, 0f, fadeInDuration);

        group.blocksRaycasts = false;
        IsTransitioning = false;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (group == null)
        {
            yield break;
        }

        fadeImage.color = fadeColor;

        float elapsed = 0f;
        group.alpha = from;

        while (elapsed < duration)
        {
            // Unscaled so a paused game still fades correctly.
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            if (useSmoothing)
            {
                t = t * t * (3f - 2f * t); // smoothstep
            }

            group.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        group.alpha = to;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            IsTransitioning = false;
        }
    }
}