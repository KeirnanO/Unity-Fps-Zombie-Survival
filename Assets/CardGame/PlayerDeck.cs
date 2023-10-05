using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    public List<CardScriptableObject> deck;

    public int deckSize = 10;

    static int SortByCardType(CardScriptableObject type1, CardScriptableObject type2)
    {
        return (type1.cardID).CompareTo(type2.cardID);
    }

    public void Sort()
    {
        deck.Sort(SortByCardType);
    }

    public void SetDeckSize(int size)
    {
        deckSize = size;

        while(deck.Count > deckSize)
        {
            deck.RemoveAt(deck.Count - 1);
        }
    }
}
