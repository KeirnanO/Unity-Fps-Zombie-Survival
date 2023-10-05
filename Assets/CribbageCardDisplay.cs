using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CribbageCardDisplay : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public CribbageCard card;

    public Image cardImage;
    public Image dimImage;
    public CribbagePlayerHandUI playerHand; 

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

    public void Dim(bool enabled)
    {
        dimImage.enabled = enabled;
    }

    public void Select()
    {
        //cardOutline.color = new Color(1, 0, 0, 1);
    }

    public void Deselect()
    {
        //cardOutline.color = new Color(1, 1, 1, 0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        playerHand.SelectCard(handIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        playerHand.HoverCard(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        playerHand.HoverCard(null);
    }
}