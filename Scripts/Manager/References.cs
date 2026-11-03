using UnityEngine;

public class References : MonoBehaviour
{
    public enum GameState
    {   
    GameStart,
    PlayerTurnStart,
    ActionSelection,
    ResolveAction,
    ResolveLanding,
    CheckGem,
    CheckWin,
    DrawCards,
    NextTurn,
    GameOver
    }

    [Header("References")]
    public TurnManager turnManager;
    public BoardController board;
    public GameManager gameManager;
    public GemController testGem;

    [Header("Test Settings")]
    public int totalTurns = 24;
    public float delayBetweenTurns = 0.1f;

    [Tooltip("Smallest and largest move a simulated player may try.")]
    public int minMove = 1;
    public int maxMove = 3;

    [Header("Token Checks")]
    [Tooltip("Verify the token sprite really reaches its board space.")]
    public bool verifyTokenMovement = true;
    [Tooltip("Pause between steps in the walk-the-track test so you can watch.")]
    public float stepDelay = 0.3f;
    [Tooltip("Real seconds to wait for a token to arrive before failing.")]
    public float tokenArriveTimeout = 5f;
    [Tooltip("How close (world units) a token must be to its space.")]
    public float tokenTolerance = 0.05f;

    private int passed = 0;
    private int failed = 0;
    private int warnings = 0;

    // Set after the token setup check; token tests are skipped if false
    private bool tokensReady = false;

    // The highest space index that both the board and the markers cover
    private int usableLast = 0;
}
