using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Players")]
    public PlayerController[] players;

    [Header("Board")]
    public BoardController board;

    [Header("Game Settings")]
    public int winningScore = 2;

    public int currentPlayerIndex = 0;

    private int[] playerScores;

    private void Start()
    {
        playerScores = new int[players.Length];

        Debug.Log("GEM KO 'TO! has started!");
        Debug.Log("Player " + (currentPlayerIndex + 1) + "'s turn.");
    }

    public PlayerController GetCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }

    public void NextTurn()
    {
        currentPlayerIndex++;

        if (currentPlayerIndex >= players.Length)
        {
            currentPlayerIndex = 0;
        }

        Debug.Log("Player " + (currentPlayerIndex + 1) + "'s turn.");
    }

    public void AddScore(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= playerScores.Length)
        {
            return;
        }

        playerScores[playerIndex]++;

        Debug.Log(
            "Player " + (playerIndex + 1) +
            " scored! Total score: " +
            playerScores[playerIndex]
        );

        CheckWinner(playerIndex);
    }

    private void CheckWinner(int playerIndex)
    {
        if (playerScores[playerIndex] >= winningScore)
        {
            Debug.Log(
                "Player " + (playerIndex + 1) +
                " wins GEM KO 'TO!"
            );
        }
    }

    public void PlayMovementCard(CardData card)
    {
        PlayerController currentPlayer = GetCurrentPlayer();

        currentPlayer.MoveForward(card.movementAmount);
    }

    public void AttackPlayer(
        PlayerController targetPlayer,
        CardData card
    )
    {
        if (card.targetRequired == false)
        {
            return;
        }

        targetPlayer.MoveBackward(card.targetMovement);
    }

    public void UseOuchieCard(
        PlayerController targetPlayer,
        GemController targetGem,
        CardData card
    )
    {
        if (card.returnsGem && targetPlayer.carryingGem)
        {
            targetPlayer.LoseGem();
            targetGem.ReturnToCave();
        }
    }
}