using UnityEngine;

public class BoardController : MonoBehaviour
{
    [Header("Board Data")]
    public int boardSize;
    public int startPosition = 1;
    public int gemCavePosition = 1;

    public bool IsGemCave(int position)
    {
        return position == gemCavePosition;
    }

    public bool IsAtStart(int position)
    {
        return position == startPosition;
    }

    public bool IsValidPosition(int position)
    {
        return position >= 0 && position <= boardSize;
    }
}
