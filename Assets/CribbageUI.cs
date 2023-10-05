using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CribbageUI : MonoBehaviour
{
    public GameObject discardUI;
    public GameObject countingUI;
    public GameObject tutPlayUI;
    public TextMeshProUGUI tutPlayText;

    public string[] tutStrings = { "Waiting for your turn", "Choose a card to add to the count [A-10 are face value, J,Q,K are 10]" };

    private static CribbageUI instance;
    public static CribbageUI Instance 
    { 
        get 
        { 
            return instance;
        } 
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CribbageCarGameManager.OnCountChange += SetCountText;
    }

    public TextMeshProUGUI countText;
    public CribbagePlayerHandUI playerHand;
    public PlayerPlayingField[] playingFields;
    public List<CribbageOpponentHandUI> opponentHands;

    void SetCountText(int count)
    {
        countText.SetText(count.ToString());
    }

    public void HighlightPlayableCards()
    {
        playerHand.HighlightPlayableCards(CribbageCarGameManager.instance.count, CribbageCarGameManager.maxCount);
    }

    public void DimHand()
    {
        playerHand.DimCards();
    }

    public void SetOpponentCardDisplay(CribbageGamePlayer player)
    {
        foreach (var opponent in opponentHands)
        {
            if(opponent.LocalPlayer.Equals(player))
            {
                opponent.SetCardDisplay(player.playerHandCardIndex);
            }
        }
    }

    public void SetCountingUI()
    {
        //Show hand to player - this can be an animation
        //Show hand ui the score the player got
    }

    public void PlayCard(CribbageCard card, CribbageGamePlayer player)
    {
        foreach(var field in playingFields)
        {
            if (field.LocalPlayer.Equals(player))
            {
                field.PlayCard(card);
                break;
            }
        }
    }

    public void SetDiscardMode(bool enabled)
    {
        discardUI.SetActive(enabled);
        playerHand.SetDiscardMode(enabled);
    }

    public void ShowTut(bool enabled)
    {
        tutPlayUI.SetActive(enabled);
    }

    public void TutChange(bool turn)
    {
        string text = turn ? tutStrings[1] : tutStrings[0];

        tutPlayText.SetText(text);
    }

    public void SetUpUI()
    {
        var players = CribbageCarGameManager.instance.clientPlayerRefs;

        int localPlayerIndex = 0;

        //Find localPlayer and setup our UI
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].isLocalPlayer)
            {
                localPlayerIndex = i;
                playingFields[0].SetPlayer(players[i]);
            }
        }

        //Fill rest of the fields with opponents starting with the player after the localplayer
        for (int i = 1; i < players.Count; i++)
        {
            playingFields[i].SetPlayer(players[(localPlayerIndex + i)%players.Count]);
        }
    }

    public void ResetFieldUI()
    {
        foreach (var field in playingFields)
        {
            field.ResetCards();
        }
    }

    public void ShowCards(List<int> handCardIndex)
    {

    }
}
