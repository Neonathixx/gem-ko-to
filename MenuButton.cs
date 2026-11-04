using UnityEngine;
using UnityEngine.InputSystem;

public enum MenuAction
{
    StartGame,
    OpenSettings,
    QuitGame
}

/// <summary>
/// Makes a plain sprite/mesh token behave like a button.
/// Put one of these on each of your three SampleButton objects.
/// The object needs a Collider2D (for sprites) or a Collider (for 3D meshes).
/// Uses the new Input System, matching GameStateManager.
/// </summary>
public class MenuButton : MonoBehaviour
{
    [Header("What This Token Does")]
    public MenuAction action = MenuAction.StartGame;

    [Header("References")]
    [Tooltip("Drag the object holding MenuManager here.")]
    public MenuManager menuManager;

    [Tooltip("Leave empty to use the Main Camera.")]
    public Camera menuCamera;

    [Header("Hover Feel")]
    public bool growOnHover = true;
    public float hoverScale = 1.12f;
    public float scaleSpeed = 12f;

    [Header("Click Guard")]
    [Tooltip("Smallest gap between two clicks on this token, in seconds.")]
    public float clickCooldown = 0.25f;

    private Vector3 baseScale;
    private float lastClickTime = -999f;
    private bool pointerIsOver = false;

    private void Awake()
    {
        baseScale = transform.localScale;

        if (menuCamera == null)
        {
            menuCamera = Camera.main;
        }

        if (menuCamera == null)
        {
            Debug.LogError(
                name + ": no camera found. Tag your menu camera as MainCamera " +
                "or drag it into Menu Camera.",
                this
            );
        }

        if (menuManager == null)
        {
            menuManager = FindFirstObjectByType<MenuManager>();
        }

        if (GetComponent<Collider2D>() == null && GetComponent<Collider>() == null)
        {
            Debug.LogError(
                name + ": this token has no collider, so it can never be clicked. " +
                "Add a Box Collider 2D (sprite) or a Box Collider (3D mesh).",
                this
            );
        }
    }

    private void Update()
    {
        pointerIsOver = IsPointerOverThisToken();

        if (growOnHover)
        {
            Vector3 target = pointerIsOver ? baseScale * hoverScale : baseScale;

            transform.localScale = Vector3.Lerp(
                transform.localScale,
                target,
                scaleSpeed * Time.unscaledDeltaTime
            );
        }

        if (!pointerIsOver)
        {
            return;
        }

        bool clicked =
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame;

        if (clicked && Time.unscaledTime - lastClickTime >= clickCooldown)
        {
            lastClickTime = Time.unscaledTime;
            Press();
        }
    }

    /// <summary>
    /// Public so a UI Button's On Click() can also call it if you swap to a Canvas later.
    /// </summary>
    public void Press()
    {
        if (menuManager == null)
        {
            Debug.LogError(name + ": no MenuManager assigned.", this);
            return;
        }

        // Ignore clicks while a scene fade is running.
        if (menuManager.IsBusy())
        {
            return;
        }

        switch (action)
        {
            case MenuAction.StartGame:
                menuManager.StartGame();
                break;

            case MenuAction.OpenSettings:
                menuManager.ToggleSettings();
                break;

            case MenuAction.QuitGame:
                menuManager.QuitGame();
                break;
        }
    }

    private bool IsPointerOverThisToken()
    {
        if (menuCamera == null || Mouse.current == null)
        {
            return false;
        }

        Vector2 screenPos = Mouse.current.position.ReadValue();

        // 3D path: works for mesh tokens and for sprites with a 3D collider.
        Ray ray = menuCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            if (hit.collider.gameObject == gameObject)
            {
                return true;
            }
        }

        // 2D path: works for sprites with a Collider2D.
        // Cast along the ray so it is correct under a perspective camera too.
        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray, 1000f);

        if (hit2D.collider != null && hit2D.collider.gameObject == gameObject)
        {
            return true;
        }

        return false;
    }
}