using UnityEngine;

public enum PlayerColor
{
    Red,
    Yellow,
    Blue,
    Purple
}

public class PlayerController : MonoBehaviour
{
    [Header("Player Data")]
    public int playerID;
    public PlayerColor playerColor;

    [Header("Game State")]
    public int currentPosition = 0;
    public bool carryingGem = false;

    [Header("Token Movement")]
    [Tooltip("One Transform per board space, in order (space 0 first).")]
    public Transform[] boardSpaces;

    [Tooltip("How fast the token slides to its space (units per second).")]
    public float moveSpeed = 8f;

    private Vector3 targetPosition;
    private bool hasTarget = false;

    private void Start()
    {
        // Put the token on its starting space
        SnapToCurrentPosition();
    }

    private void Update()
    {
        if (!hasTarget)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (transform.position == targetPosition)
            hasTarget = false;
    }

    // Slides the token to the space matching currentPosition
    public void UpdateTokenPosition()
    {
        if (!TryGetSpacePosition(out Vector3 pos))
            return;

        targetPosition = pos;
        hasTarget = true;
    }

    // Teleports the token to the space matching currentPosition
    public void SnapToCurrentPosition()
    {
        if (!TryGetSpacePosition(out Vector3 pos))
            return;

        transform.position = pos;
        targetPosition = pos;
        hasTarget = false;
    }

    // Sets the position number and moves the token there
    public void SetPosition(int position)
    {
        currentPosition = position;
        UpdateTokenPosition();
    }

    private bool TryGetSpacePosition(out Vector3 pos)
    {
        pos = transform.position;

        if (boardSpaces == null || boardSpaces.Length == 0)
        {
            Debug.LogWarning(
                "Player " + playerID +
                ": boardSpaces is empty, so the token cannot move. " +
                "Assign the space Transforms in the Inspector.",
                this
            );
            return false;
        }

        if (currentPosition < 0 || currentPosition >= boardSpaces.Length)
        {
            Debug.LogWarning(
                "Player " + playerID + ": position " + currentPosition +
                " has no matching board space (0 to " +
                (boardSpaces.Length - 1) + ").",
                this
            );
            return false;
        }

        if (boardSpaces[currentPosition] == null)
        {
            Debug.LogWarning(
                "Player " + playerID + ": boardSpaces[" +
                currentPosition + "] is empty.",
                this
            );
            return false;
        }

        // Keep the token's own Z so it stays in front of the board in 2D
        Vector3 space = boardSpaces[currentPosition].position;
        pos = new Vector3(space.x, space.y, transform.position.z);
        return true;
    }

    public void MoveForward(int spaces)
    {
        currentPosition += spaces;

        // Don't walk off the end of the board
        if (boardSpaces != null && boardSpaces.Length > 0 &&
            currentPosition > boardSpaces.Length - 1)
        {
            currentPosition = boardSpaces.Length - 1;
        }

        UpdateTokenPosition();
        Debug.Log("Player " + playerID + " moved " + spaces + " spaces.");
    }

    public void MoveBackward(int spaces)
    {
        currentPosition -= spaces;

        if (currentPosition < 0)
        {
            currentPosition = 0;
        }

        UpdateTokenPosition();
        Debug.Log("Player " + playerID + " moved backward " + spaces + " spaces.");
    }

    public void CollectGem()
    {
        carryingGem = true;
        Debug.Log("Player " + playerID + " collected a gem.");
    }

    public void ReturnGem()
    {
        carryingGem = false;
        Debug.Log("Player " + playerID + " returned the gem to their base.");
    }

    public void LoseGem()
    {
        carryingGem = false;
        Debug.Log("Player " + playerID + " lost the gem.");
    }
}