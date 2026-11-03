using UnityEngine;

public class GemController : MonoBehaviour
{
    [Header("Gem Data")]
    public int gemID;
    public PlayerColor gemColor;

    [Header("Game State")]
    public bool isCollected = false;

    public void CollectGem()
    {
        isCollected = true;
        Debug.Log("Gem " + gemID + " has been collected.");
    }

    public void ReturnToCave()
    {
        isCollected = false;
        Debug.Log("Gem " + gemID + " returned to the Gem Cave.");
    }
}
