using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public enum Suit
{
   Spades = 0,
   Clubs = 1,
   Hearts = 2,
   Diamonds = 3
}

public enum Value
{
    Ace = 0,
    Two = 1,
    Three = 2,
    Four = 3,
    Five = 4,
    Six = 5,
    Seven = 6,
    Eight = 7,
    Nine = 8,
    Ten = 9,
    Jack = 10,
    Queen = 11,
    King = 12
}

[CreateAssetMenu(fileName = "CribbageCard", menuName = "CribbageCard")]
public class CribbageCard : ScriptableObject
{
    public Suit suit;
    public Value value;
    public Sprite cardSprite;

    public int GetID()
    {
        return ((int)suit * 13) + (int)value;
    }

    public bool IsPlayable(int count, int maxCount)
    {
        return (count + Mathf.Clamp((int)value + 1, 1, 10)) <= maxCount;
    }
}


