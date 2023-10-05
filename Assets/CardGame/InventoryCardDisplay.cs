using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryCardDisplay : MonoBehaviour, IPointerClickHandler
{
    public CardScriptableObject card;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    public TextMeshProUGUI manaText;
    public Image[] bonusManaIcons;

    public Image cardIcon;

    public void SetCard(CardScriptableObject _card)
    {
        card = _card;

        if (!_card)
        {
            gameObject.SetActive(false);
            return;
        }

        nameText.text = card.name;
        descriptionText.text = card.description;
        cardIcon.sprite = _card.cardSprite;

        manaText.text = _card.GetNormalManaCost().ToString();

        foreach (var icon in bonusManaIcons)
        {
            icon.enabled = false;
        }

        int iconCounter = 0;
        foreach (var manaType in _card.GetBonusManaCost())
        {
            Color manaColor = Color.white;

            switch (manaType)
            {
                case ManaType.Power:
                    manaColor = Color.yellow;
                    break;
                case ManaType.Ice:
                    manaColor = Color.blue;
                    break;
                case ManaType.Fire:
                    manaColor = Color.red;
                    break;
            }

            bonusManaIcons[iconCounter].enabled = true;
            bonusManaIcons[iconCounter].color = manaColor;
            iconCounter++;
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        GetComponentInParent<InventoryMenuController>().AddCard(card);
    }
}
