using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class CribbagePlayerHandUI : MonoBehaviour
{
    public static CribbagePlayerHandUI instance;

    public bool discardMode = false;
    public int[] cardsToDiscard = {-1, -1};

    private CribbageGamePlayer localPlayer;
    public CribbageGamePlayer LocalPlayer 
    {
        get
        {
            if(localPlayer == null)
            {
                foreach (var player in FindObjectsOfType<CribbageGamePlayer>())
                {
                    if (player.isOwned)
                    {
                        localPlayer = player;
                        break;
                    }
                }
            }
            return localPlayer;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] private List<CribbageCardDisplay> cardDisplays;

    public bool selected = false;

    public void SetCardDisplay(List<int> cardIDs)
    {
        for (int i = 0; i < Mathf.Clamp(cardIDs.Count, 0, cardDisplays.Count); i++)
        {
            cardDisplays[i].SetCard(CribbageCardDatabase.instance.CardDatabase[cardIDs[i]]);
            cardDisplays[i].gameObject.SetActive(true);
        }

        for (int i = cardIDs.Count; i < cardDisplays.Count; i++)
        {
            cardDisplays[i].SetCard(null);
            cardDisplays[i].gameObject.SetActive(false);
        }

        Vector3 cardOffset = Vector3.right * 200;
        Vector3 evenOffset = Mathf.Clamp(cardIDs.Count, 0, cardDisplays.Count) % 2 == 0 ? cardOffset / 2 : Vector3.zero;
        int mid = Mathf.FloorToInt(Mathf.Clamp(cardIDs.Count, 0, cardDisplays.Count) / 2);

        for (int i = 0; i < Mathf.Clamp(cardIDs.Count, 0, cardDisplays.Count); i++)
        {
            int xOffset = i - mid;

            cardDisplays[i].SetPosition(Vector3.zero + (cardOffset * xOffset) + evenOffset);
        }
    }

    public void HoverCard(CribbageCardDisplay cardDisplay)
    {
        if (cardDisplay == null)
        {
            foreach (CribbageCardDisplay card in cardDisplays)
            {
                card.transform.localScale = new Vector3(3, 3, 3);
                card.transform.localPosition = card.position;
            }
            return;
        }

        foreach (CribbageCardDisplay card in cardDisplays)
        {
            if (card == cardDisplay)
                continue;

            card.transform.localScale = new Vector3(3, 3, 3);

            Vector3 offset = card.handIndex < cardDisplay.handIndex ? Vector3.left * 25 : Vector3.right * 25;

            card.transform.localPosition = card.position + offset;
        }

        cardDisplay.transform.localScale = new Vector3(4, 4, 3);
    }

    public void HighlightPlayableCards(int count, int maxCount)
    {
        foreach (CribbageCardDisplay display in cardDisplays)
        {
            if (display.card == null)
                continue;

            if(count + Mathf.Clamp((int)display.card.value + 1, 1, 10) <= maxCount)
            {
                display.Dim(false);
            }
        }
    }

    public void DimCards()
    {
        foreach (CribbageCardDisplay display in cardDisplays)
        {
            display.Dim(true);
        }
    }

    public void UnSelectCard()
    {
        foreach (CribbageCardDisplay display in cardDisplays)
        {
            display.Deselect();
        }
        selected = false;
        return;
    }

    
    public void SelectCard(int index)
    {
        //Tell server what card was selected
        //normal select card
        if (!discardMode)
        {
            LocalPlayer.CmdSelectCard(index);
        }
        else
        {
            //If we are discarding the card we should store it in a list of cards to discard that can be confirmed for discard later
            //Cards about to be discarded should be a different colour;

            if (cardsToDiscard[0] < 0)
            {
                cardsToDiscard[0] = index;
                cardDisplays[index].Dim(true);
            }
            else if (cardsToDiscard[0] == index)
            {
                cardsToDiscard[0] = -1;
                cardDisplays[index].Dim(false);
            }
            else if (cardsToDiscard[1] < 0)
            {
                cardsToDiscard[1] = index;
                cardDisplays[index].Dim(true);
            }
            else if (cardsToDiscard[1] == index)
            {
                cardDisplays[index].Dim(false);
                cardsToDiscard[1] = -1;
            }

            if (cardsToDiscard[0] != -1 && cardsToDiscard[1] != -1)
            {
                LocalPlayer.CmdSelectCardsToDiscard(cardsToDiscard);
            }
        }
    }

    public void SetDiscardMode(bool enabled)
    {
        discardMode = enabled;

        cardsToDiscard[0] = -1;
        cardsToDiscard[1] = -1;
    }
}
