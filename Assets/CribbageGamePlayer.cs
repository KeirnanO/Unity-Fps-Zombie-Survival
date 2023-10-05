using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CribbageGamePlayer : NetworkBehaviour
{
    public List<int> playerHandCardIndex;
    public List<int> trueHandCardIndex;
    [SyncVar(hook = nameof(SelectCardHook))]
    public int selectedCardIndexInHand;

    public bool dealer = false;

    public override void OnStartServer()
    {
        SelectCardHook(0, -1);
        selectedCardIndexInHand = -1;
    }

    public void SelectCardHook(int _oldValue, int _newValue)
    {
    }

    [Command]
    public void CmdSelectCard(int handIndex)
    {
        selectedCardIndexInHand = handIndex;
    }

    [Command]
    public void CmdSelectCardsToDiscard(int[] cards)
    {
        int cardID1 = CribbageCardDatabase.instance.CardDatabase[playerHandCardIndex[cards[0]]].GetID();
        int cardID2 = CribbageCardDatabase.instance.CardDatabase[playerHandCardIndex[cards[1]]].GetID();

        CribbageCarGameManager.instance.DiscardCard(cardID1);
        CribbageCarGameManager.instance.DiscardCard(cardID2);
        playerHandCardIndex.Remove(cardID1);
        playerHandCardIndex.Remove(cardID2);

        RpcDiscardCard(cards[0], cardID1);
        RpcDiscardCard(cards[1], cardID2);

        RpcDisableInput();
        RpcSetHand();

        trueHandCardIndex.Clear();
        trueHandCardIndex = new List<int>(playerHandCardIndex);
    }

    [Server]
    public CribbageCard ServerPlayCard()
    {
        CribbageCard playedCard = CribbageCardDatabase.instance.CardDatabase[playerHandCardIndex[selectedCardIndexInHand]];

        CribbageCarGameManager.instance.IncreaseCount(Mathf.Clamp((int)playedCard.value + 1, 1, 10));

        RpcPlayCard(selectedCardIndexInHand, playerHandCardIndex[selectedCardIndexInHand]);
        playerHandCardIndex.RemoveAt(selectedCardIndexInHand);

        return playedCard;
    }

    [Server]
    public void ResetHand()
    {
        selectedCardIndexInHand = -1;
    }

    [ClientRpc]
    public void RpcPlayCard(int handIndex, int cardID)
    {
        //If we do not own this player we can not see the other players cards in hand
        //Set the card being played to the correct cardID
        if(!isOwned && !isServer)
        {
            playerHandCardIndex[handIndex] = cardID;
        }

        if (!isServer)
            playerHandCardIndex.RemoveAt(handIndex);

        //Play card infront of Player
        CribbageUI.Instance.PlayCard(CribbageCardDatabase.instance.CardDatabase[cardID], this);

        if (isOwned)
            CribbagePlayerHandUI.instance.SetCardDisplay(playerHandCardIndex);
        else
            CribbageUI.Instance.SetOpponentCardDisplay(this);
    }

    [Server]
    public CribbageCard ServerDiscardCard()
    {
        if (selectedCardIndexInHand < 0 || selectedCardIndexInHand > playerHandCardIndex.Count - 1)
            return null;

        CribbageCard playedCard = CribbageCardDatabase.instance.CardDatabase[playerHandCardIndex[selectedCardIndexInHand]];
        int handIndex = selectedCardIndexInHand;

        playerHandCardIndex.RemoveAt(handIndex);
        selectedCardIndexInHand = -1;

        RpcDiscardCard(handIndex, playedCard.GetID());        
        return playedCard;
    }

    [ClientRpc]
    public void RpcDiscardCard(int handIndex, int cardID)
    {
        //If we do not own this player we can not see the other players cards in hand
        //Set the card being played to the correct cardID
        if (!isOwned && !isServer)
        {
            playerHandCardIndex.RemoveAt(Mathf.Clamp(handIndex, 0, playerHandCardIndex.Count));
            CribbageUI.Instance.SetOpponentCardDisplay(this);
            return;
        }

        if (!isServer)
            playerHandCardIndex.Remove(cardID);

        CribbagePlayerHandUI.instance.SetCardDisplay(playerHandCardIndex);

        CribbageUI.Instance.DimHand();
        CribbageUI.Instance.TutChange(false);
        CribbageUI.Instance.SetDiscardMode(false);        
    }

    [ClientRpc]
    public void RpcSetHand()
    {
        trueHandCardIndex = new List<int>(playerHandCardIndex);
    }

    [ClientRpc]
    public void DrawCard(int cardID)
    {
        int cardToAdd = isOwned ? cardID : -1;

        if (!isServer)
            playerHandCardIndex.Add(cardToAdd);

        if (isClient)
        {
            if (!isOwned)
            {
                CribbageUI.Instance.SetOpponentCardDisplay(this);
                return;
            }
            CribbagePlayerHandUI.instance.SetCardDisplay(playerHandCardIndex);
        }
    }

    [ClientRpc]
    public void ShowTrueHand()
    {
        if (!isOwned)
            return;

        CribbagePlayerHandUI.instance.SetCardDisplay(trueHandCardIndex);
    }

        [ClientRpc]
    public void RpcSetUpPlayer()
    {
        CribbageCarGameManager.instance.clientPlayerRefs.Add(this);
        CribbageUI.Instance.SetUpUI();
        //If a client is owned the board will automatically setup its own view and UI 


        //However if we do not own the player we must create a space for them to exist on our client
        if (!isOwned)
        {
            CribbageUI.Instance.opponentHands[0].SetLocalPlayer(this);
        }
    }

    [ClientRpc]
    public void EnableInput()
    {
        if (!isOwned)
            return;

        CribbageUI.Instance.HighlightPlayableCards();
        CribbageUI.Instance.TutChange(true);

    }

    [ClientRpc]
    public void RpcDisableInput()
    {
        if (!isOwned)
            return;

        CribbageUI.Instance.DimHand();
        CribbageUI.Instance.TutChange(false);
    }

    [Server]
    public bool HasAnyPlayableCards(int count, int maxCount)
    {
        foreach(var cardID in playerHandCardIndex)
        {
            if (CribbageCardDatabase.instance.CardDatabase[cardID].IsPlayable(count, maxCount))
                return true;
        }

        return false;
    }

    [Server]
    public bool HasPlayableCardSelected(int count, int maxCount)
    {
        if (selectedCardIndexInHand < 0)
            return false;

        if(CribbageCardDatabase.instance.CardDatabase[playerHandCardIndex[selectedCardIndexInHand]].IsPlayable(CribbageCarGameManager.instance.count, CribbageCarGameManager.maxCount))
        {
            return true;
        }

        return false;
    }
}
