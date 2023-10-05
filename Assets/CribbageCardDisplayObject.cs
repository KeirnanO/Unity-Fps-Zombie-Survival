using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CribbageCardDisplayObject : MonoBehaviour
{
    public CribbageCard card;
    public SpriteRenderer cardImage;

    public bool flipped = false;

    public void SetCard(CribbageCard _card)
    {
        card = _card;

        if (!_card)
        {
            gameObject.SetActive(false);
            return;
        }

        cardImage.sprite = _card.cardSprite;
    }

    public void FlipCard()
    {
        flipped = !flipped;

        Sprite cardSprite = flipped ? CribbageCardDatabase.instance.CardBackDatabase[0].cardSprite : card.cardSprite;

        cardImage.sprite = cardSprite;
    }
}
