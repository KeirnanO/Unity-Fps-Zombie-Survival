using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CribbageDeck : MonoBehaviour
{
    public CribbageCardDisplayObject[] topCards;

    public void FlipTopCard()
    {
        topCards[0].gameObject.GetComponent<Animator>().Play("Card_Flip");
    }

    public void SetTopCard(int cardID)
    {
        topCards[0].card = CribbageCardDatabase.instance.CardDatabase[cardID];
    }


}
