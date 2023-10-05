using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Basic = 0,
    Attack = 1,
    Heal = 2,
    Charm = 3,
    Ward = 4,
    Enchant = 5
}

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardScriptableObject : ScriptableObject
{
    public new string name;
    public string description;

    public List<ManaType> manaCost;
    public CardType cardType;
    public int attack;
    public bool AOE;

    public int cardID;

    public CardScriptableObject upgradedCard;
    public GameObject animationObject;
    public Sprite cardSprite;

    public void Print()
    {
        Debug.Log(name + ": " + description + " - The Card Costs: " + manaCost);
    }

    public int GetNormalManaCost()
    {
        int cost = 0;


        foreach (ManaType type in manaCost)
        {
            if (type == ManaType.Normal)
                cost++;
        }

        return cost;
    }

    public List<ManaType> GetBonusManaCost()
    {
        List<ManaType> bonusManaCost = new List<ManaType>();

        foreach (ManaType type in manaCost)
        {
            if (type != ManaType.Normal)
                bonusManaCost.Add(type);
        }

        return bonusManaCost;
    }
}
