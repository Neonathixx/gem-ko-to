using UnityEngine;

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

public class GameStateManager : MonoBehaviour
{
    [Header("Game State")]
    public GameState currentState;

    [Header("References")]
    public GameManager gameManager;
    public TurnManager turnManager;

    private void Start()
    {
        ChangeState(GameState.GameStart);
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        Debug.Log("GAME STATE: " + currentState);

        ExecuteState();
    }

    private void ExecuteState()
    {
        switch (currentState)
        {
            case GameState.GameStart:
                ChangeState(GameState.PlayerTurnStart);
                break;

            case GameState.PlayerTurnStart:
                Debug.Log(
                    "Player " +
                    (turnManager.CurrentPlayerIndex + 1) +
                    "'s turn starts."
                );
                ChangeState(GameState.ActionSelection);
                break;

            case GameState.ActionSelection:
                Debug.Log("Waiting for player to select a card.");
                break;

            case GameState.ResolveAction:
                Debug.Log("Resolving selected card or card combo.");
                ChangeState(GameState.ResolveLanding);
                break;

            case GameState.ResolveLanding:
                Debug.Log("Checking the player's landing space.");
                ChangeState(GameState.CheckGem);
                break;

            case GameState.CheckGem:
                Debug.Log("Checking gem collection or return.");
                ChangeState(GameState.CheckWin);
                break;

            case GameState.CheckWin:
                Debug.Log("Checking the player's score.");
                ChangeState(GameState.DrawCards);
                break;

            case GameState.DrawCards:
                Debug.Log("Restoring the player's hand to 3 cards.");
                ChangeState(GameState.NextTurn);
                break;

            case GameState.NextTurn:
                turnManager.NextTurn();
                ChangeState(GameState.PlayerTurnStart);
                break;

            case GameState.GameOver:
                Debug.Log("GAME OVER!");
                break;
        }
    }


    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ConfirmAction();
        }
    }

    public void ConfirmAction()
    {
        if (currentState == GameState.ActionSelection)
        {
            ChangeState(GameState.ResolveAction);
        }
    }



    public void EndGame()
    {
        ChangeState(GameState.GameOver);
    }
}