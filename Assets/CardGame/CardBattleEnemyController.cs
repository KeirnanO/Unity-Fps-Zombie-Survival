using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CardBattleEnemyController : NetworkBehaviour
{
    // Start is called before the first frame update
    public List<CardScriptableObject> deck;
    public Mana mana;

    public int targetIndex;
    public int cardIndex;

    [Server]
    public int[] ChooseCard()
    {
        targetIndex = -1;
        cardIndex = -1;

        //Chooses a random card 20 times
        for (int i = 0; i < 20; i++)
        {
            int random = Random.Range(0, deck.Count);

            //If card is playable
            if (mana.CheckCost(deck[random].manaCost))
            {
                switch (deck[random].cardType)
                {
                    case CardType.Attack:
                        cardIndex = deck[random].cardID;
                        targetIndex = 0;
                        break;
                    case CardType.Charm:
                        cardIndex = deck[random].cardID;
                        targetIndex = 0;
                        break;
                    case CardType.Ward:
                        cardIndex = deck[random].cardID;
                        targetIndex = 0;
                        break;
                }
            }
        }

        return new int[] { cardIndex, targetIndex };
    }

    [Server]
    public void GetCardPhase(FightingZone zone)
    {
       
    }
}
