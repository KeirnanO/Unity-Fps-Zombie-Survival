using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlayingField : MonoBehaviour
{
    public CribbageCardDisplayObject[] playableCards;
    public int playedCards = 0;

    private CribbageGamePlayer localPlayer;

    public CribbageGamePlayer LocalPlayer { get { return localPlayer; } }


    public void FlipCards()
    {

    }

    public void PlayCard(CribbageCard card)
    {
        playableCards[playedCards].gameObject.SetActive(true);
        playableCards[playedCards].SetCard(card);
        playableCards[playedCards].gameObject.GetComponent<Animator>().Play("PlayCardAnim");


        playedCards++;
    }

    public void ResetCards()
    {
        foreach(var display in playableCards)
        {
            display.gameObject.SetActive(false);
        }

        playedCards = 0;
    }

    public void SetPlayer(CribbageGamePlayer player)
    {
        localPlayer = player;
    }
}
