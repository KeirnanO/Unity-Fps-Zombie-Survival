using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardDisplay : MonoBehaviour, IPointerClickHandler , IPointerEnterHandler, IPointerExitHandler
{
    public CardScriptableObject card;

    public Graphic cardOutline;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    public TextMeshProUGUI manaText;
    public Image[] bonusManaIcons;

    public Image cardIcon;
    public PlayerHand playerHand;

    public int handIndex;

    public Vector3 position;

    private void Start()
    {
        nameText.text = card.name;
        descriptionText.text = card.description;

        //manaText.text = card.manaCost.ToString();
    }

    public void SetCard(CardScriptableObject _card)
    {
        card = _card;

        if (!_card)
        {
            gameObject.SetActive(false);
            return;
        }

        print("Setting " + handIndex + " card to " + _card.name);

        nameText.text = _card.name;
        descriptionText.text = _card.description;
        cardIcon.sprite = _card.cardSprite;

        manaText.text = _card.GetNormalManaCost().ToString();

        foreach(var icon in bonusManaIcons)
        {
            icon.enabled = false;
        }

        int iconCounter = 0;
        foreach(var manaType in _card.GetBonusManaCost())
        {
            Color manaColor = Color.white;

            switch (manaType)
            {
                case ManaType.Power:
                    manaColor = Color.yellow;
                    break;
                case ManaType.Ice:
                    manaColor= Color.blue;
                    break;
                case ManaType.Fire:
                    manaColor= Color.red;
                    break;
            }

            bonusManaIcons[iconCounter].enabled = true;
            bonusManaIcons[iconCounter].color = manaColor;
            iconCounter++;
        }
    }

    public void CopyCardDisplay(CardDisplay display)
    {
        SetCard(display.card);
    }

    public void UpgradeCard()
    {
        SetCard(card.upgradedCard);
    }

    public void SetPosition(Vector3 newPosition)
    {
        position = newPosition;
        transform.localPosition = newPosition;
    }

    public void Select()
    {
        cardOutline.color = new Color(1, 0, 0, 1);
    }
    public void Shine()
    {
        cardOutline.color = new Color(1, 1, 1, 1);
    }
    public void Deselect()
    {
        cardOutline.color = new Color(1, 1, 1, 0);
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
