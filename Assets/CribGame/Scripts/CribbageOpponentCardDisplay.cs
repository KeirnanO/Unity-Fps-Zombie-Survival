using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CribbageOpponentCardDisplay : MonoBehaviour
{
    public CribbageCard card;

    public Image cardImage;

    public int handIndex;

    public Vector3 position;

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

    public void SetPosition(Vector3 newPosition)
    {
        position = newPosition;
        transform.localPosition = newPosition;
    }
}