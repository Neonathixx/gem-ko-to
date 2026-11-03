using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("Players")]
    public PlayerController[] players;

    [Header("Turn Data")]
    public int CurrentPlayerIndex { get; private set; }

    private void Start()
    {
        CurrentPlayerIndex = 0;

        Debug.Log(
            "Turn Manager started. Current player: " +
            (CurrentPlayerIndex + 1)
        );
    }

    public PlayerController GetCurrentPlayer()
    {
        if (players == null || players.Length == 0)
        {
            return null;
        }

        return players[CurrentPlayerIndex];
    }

    public void NextTurn()
    {
        if (players == null || players.Length == 0)
        {
            return;
        }

        CurrentPlayerIndex++;

        if (CurrentPlayerIndex >= players.Length)
        {
            CurrentPlayerIndex = 0;
        }

        Debug.Log(
            "Turn passed to Player " +
            (CurrentPlayerIndex + 1)
        );
    }
}