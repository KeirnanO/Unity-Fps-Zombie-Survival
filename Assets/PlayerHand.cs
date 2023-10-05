using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] private FightPlayer player;
    [SerializeField] private List<CardDisplay> cardDisplays;
    [SerializeField] private GameObject handUI;

    int selectedCardDisplayIndex;
    public bool selected = false;

    public void SetCardDisplay(List<CardScriptableObject> cards)
    {
        for(int i = 0; i < cards.Count; i++)
        {
            cardDisplays[i].SetCard(cards[i]);
            cardDisplays[i].gameObject.SetActive(true);
        }

        for (int i = cards.Count; i < cardDisplays.Count; i++)
        {
            cardDisplays[i].SetCard(null);
            cardDisplays[i].gameObject.SetActive(false);
        }

        Vector3 cardOffset = Vector3.right * 200;
        Vector3 evenOffset = cards.Count % 2 == 0 ? cardOffset / 2: Vector3.zero;
        int mid = Mathf.FloorToInt(cards.Count / 2);

        for(int i = 0; i < cards.Count; i++)
        {
            int xOffset = i - mid;

            cardDisplays[i].SetPosition(Vector3.zero + (cardOffset * xOffset) + evenOffset);
        }        
    }

    public void HoverCard(CardDisplay cardDisplay)
    {
        if(cardDisplay == null)
        {
            foreach (CardDisplay card in cardDisplays)
            {
                card.transform.localScale = new Vector3(3, 3, 3);
                card.transform.localPosition = card.position;
            }
            return;
        }

        foreach (CardDisplay card in cardDisplays)
        {
            if(card == cardDisplay)
                continue;

            card.transform.localScale = new Vector3(3, 3, 3);

            Vector3 offset = card.handIndex < cardDisplay.handIndex ? Vector3.left * 25 : Vector3.right * 25;

            card.transform.localPosition = card.position + offset;
        }

        cardDisplay.transform.localScale = new Vector3(4, 4, 3);
    }

    public void UnSelectCard()
    {
        foreach (CardDisplay display in cardDisplays)
        {
            display.Deselect();
        }

        selectedCardDisplayIndex = -1;
        selected = false;
        return;
    }

    public void SelectCard(int index)
    {
        //If we already have a card selected check combining cards first
        if (selectedCardDisplayIndex >= 0)
        {
            //Do x based on the previously selectedCard
            switch (cardDisplays[selectedCardDisplayIndex].card.cardType)
            {
                case CardType.Enchant:
                    if (cardDisplays[index].card.cardType.Equals(CardType.Attack)) { cardDisplays[index].UpgradeCard(); }
                    break;
            }

            UnSelectCard();
            return;
        }

        //If we dont already have a card selected
        //Select the pressed card  
        cardDisplays[index].Select();
        selectedCardDisplayIndex = index;

        //Do x based on what card type is selected
        //Attacks should light up enemies
        //Heals and protections should light up allies
        //Enchantments should light up cards
        switch (cardDisplays[selectedCardDisplayIndex].card.cardType)
        {
            case CardType.Attack:
                selected = true;
                break;
            case CardType.Charm:
                selected = true;
                break;
            case CardType.Enchant:
                selected = false;
                ShineByCardType(CardType.Attack);
                break;
        }

        player.CmdSelectCard(cardDisplays[index].card.cardID);
    }

    void ShineByCardType(CardType type)
    {
        foreach(CardDisplay display in cardDisplays)
        {
            if (display.card.cardType.Equals(type))
                display.Shine();
        }
    }

    List<CardDisplay> GetCardByType(CardType type)
    {
        List<CardDisplay> result = new List<CardDisplay>();

        foreach(CardDisplay display in cardDisplays)
        {
            if (display == null)
                continue;

            if(display.card.cardType.Equals(type))
                result.Add(display);
        }

        return result;    
    }

    public void ResetHand()
    {
        selected = false;
        selectedCardDisplayIndex = -1;

        foreach (CardDisplay display in cardDisplays)
        {
            display.Deselect();
        }
    }

    public void SetActive(bool active)
    {
        handUI.SetActive(active);
    }
}
