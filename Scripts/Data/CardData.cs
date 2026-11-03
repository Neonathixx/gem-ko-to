using UnityEngine;

public enum CardType
{
    Movement, Attack, Defense
}

public enum CardColor
{
    Red, Yellow, Blue, Purple
}

[CreateAssetMenu(fileName = "NewCardData", menuName = "GEM KO 'TO!/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName;
    public CardColor cardColor;
    public CardType cardType;

    [TextArea]
    public string description;

    public int movementAmount;
    public bool targetRequired;
    public int targetMovement;
    public bool returnsGem;
    public bool cancelsAttack;
}