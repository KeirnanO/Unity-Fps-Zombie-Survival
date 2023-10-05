using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class CribbageCarGameManager : NetworkBehaviour
{
    public static int maxCount = 31;

    public static CribbageCarGameManager instance;

    private void Awake()
    {
        instance = this;
    }

    public CribbageCar cribbageCarObject;

    public List<CribbageCar> playerCars = new List<CribbageCar>();
    //a list of spawned cars on the cribbage board

    public List<CribbageGamePlayer> gamePlayers = new List<CribbageGamePlayer>();
    //A list of players the server has control over

    public List<CribbageGamePlayer> clientPlayerRefs = new List<CribbageGamePlayer>();
    //A list of players (in turn-order) that the clients may use to reference other clientsObjects
    //**Most values are hidden on unowned objects**// -- Mostly used for UI things, might have to move where the clients can access this info - probably using events

    public CribbageBoard board;
    public List<int> cribCardIDList = new List<int>();

    [SyncVar(hook = nameof(CountChangeHook))]
    public int count = 0;

    public int deckSize = 0;

    public static event Action<int> OnCountChange;

    public CribbageDeck deck;
    public List<CribbageCard> cardsInDeck = new List<CribbageCard>();

    private bool AllPlayersHaveNoCards 
    { 
        get
        {
            foreach (var player in gamePlayers)
            {
                if (player.playerHandCardIndex.Count > 0)
                    return false;
            }

            return true;
        } 
    }


    public void CountChangeHook(int _oldValue, int _newValue) => OnCountChange?.Invoke(_newValue);

    [Server]
    public void StartGameLoop()
    {
        StartCoroutine(GameLoopCoroutine());
    }

    IEnumerator GameLoopCoroutine()
    {
        var players = FindObjectsOfType<NetworkGamePlayer>();

        int cut = 1;

        //Setup Game
        for (int i = 0; i < players.Length; i++)
        {
            gamePlayers.Add(players[i].GetComponent<CribbageGamePlayer>());
            gamePlayers[i].RpcSetUpPlayer();

            var newCar = Instantiate(cribbageCarObject);

            switch (playerCars.Count)
            {
                case 0:
                    newCar.SetColor(Color.red);
                    newCar.SetPosition(CribbageBoard.instance.lanes[0].positions[0].position);
                    break;
                case 1:
                    newCar.SetColor(Color.blue);
                    newCar.SetPosition(CribbageBoard.instance.lanes[1].positions[0].position);
                    break;
                default:
                    newCar.SetColor(Color.white);
                    break;
            }
            playerCars.Add(newCar);

            NetworkServer.Spawn(newCar.gameObject);

            newCar.colorId = i;
            newCar.lane = board.lanes[i];
            SetScore(i, 0);
        }

        yield return null;

        ReshuffleDeck(1);

        //Were the rotation of play starts
        int starterID = 0;

        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            count = 0;
            RpcResetBoardUI();

            //Deal Cards
            foreach (var player in gamePlayers)
            {
                player.playerHandCardIndex = GetRandomCardsInDeck(6);

                foreach (var cardID in player.playerHandCardIndex)
                {
                    player.DrawCard(cardID);
                }

                //Enable Input for discarding
                player.EnableInput();
                player.ResetHand();
            }

            yield return new WaitForSeconds(0.1f);

            //Holds the cards that will be discarded to the crib
            cribCardIDList.Clear();//temp

            RpcSetDiscardMode(true);

            //Discard Cards to Crib Round
            while(cribCardIDList.Count < gamePlayers.Count * 2)
            {
                yield return new WaitForSeconds(0.3f);
            }

            RpcSetDiscardMode(false);

            //Set cut card for all clients
            cut = GetRandomCardInDeck();
            RpcSetCutCard(cut);
            yield return new WaitForSeconds(0.3f);

            //21 phase
            //Begins Each Round While everyone has cards
            #region 21Phase
            while (!AllPlayersHaveNoCards)
            {
                List<int> turnOrder = GetNewTurnOrder(starterID);
                List<int> playersInThisRound = new List<int>(turnOrder);

                count = 0;

                int lastToPlay = -1;
                int lastCardPlayed = -1;
                int setCombo = 2;
                int runCombo = 1;

                //A Single Round of 21
                //Game will loop until count hits 21 or players are unable to play on the board
                while (playersInThisRound.Count > 0)
                {                  
                    //Loop through the turn of each player still in this round
                    for (int i = 0; i < playersInThisRound.Count; i++)
                    {
                        //Remove player from round if they dont have cards that can be played
                        if (!gamePlayers[playersInThisRound[i]].HasAnyPlayableCards(count, maxCount))
                        {
                            playersInThisRound.RemoveAt(i);
                            i--;
                            continue;
                        }

                        //Enable Client Input
                        gamePlayers[playersInThisRound[i]].ResetHand();
                        gamePlayers[playersInThisRound[i]].EnableInput();

                        //Wait until player has selected a playable card
                        while (!gamePlayers[playersInThisRound[i]].HasPlayableCardSelected(count, maxCount))
                        {
                            yield return new WaitForSeconds(0.3f);
                        }

                        //Disable PlayerClientInput
                        gamePlayers[playersInThisRound[i]].RpcDisableInput();
                        //Play card and grab the ref
                        CribbageCard playedCard = gamePlayers[playersInThisRound[i]].ServerPlayCard();

                        //Handle increasing count and scoring combos
                        #region CountAndCombos
                        if (lastCardPlayed < 0)
                        {
                            setCombo = 2;
                            lastCardPlayed = playedCard.GetID();
                        }
                        else if(playedCard.value == CribbageCardDatabase.instance.CardDatabase[lastCardPlayed].value)
                        {
                            IncreaseScore(i, setCombo);
                            setCombo += 4;
                        }
                        else if((int)playedCard.value == (int)CribbageCardDatabase.instance.CardDatabase[lastCardPlayed].value + 1)
                        {
                            runCombo++;

                            if (runCombo > 2)
                                IncreaseScore(i, runCombo);
                        }
                        else
                        {
                            setCombo = 1;
                            runCombo = 1;
                            lastCardPlayed = playedCard.GetID();
                        }                        

                        lastToPlay = playersInThisRound[i];

                        yield return null;

                        if(count == 15)
                        {
                            IncreaseScore(i, 1);
                        }

                        if (count >= maxCount)
                        {
                            starterID = playersInThisRound[i];
                            lastToPlay = -1;
                            playersInThisRound.Clear();
                            turnOrder.Clear();

                            IncreaseScore(i, 2);

                            //Delay before next round starts
                            yield return new WaitForSeconds(1f);
                        }
                        #endregion

                    }
                }

                //if there was a last player and did not reach maxCount player gets a point
                if(lastToPlay >= 0)
                    IncreaseScore(lastToPlay, 1);
               
            }
            #endregion

            //Show the players hands again
            foreach (var player in gamePlayers)
            {
                player.ShowTrueHand();
            }

            //TO:DO
            /* Disable all player cards
             * Turn on counting cards display
             * Show every players hand and show score of the car
             */

            //Counting CardsPhase
            for (int i = 1; i <= players.Length; i++)
            {
                //Count cards for each player
                //starting with the player beside the dealer 
                int playerID = (starterID + i) % players.Length;
                bool isCrib = playerID == starterID ? true : false;
                int score = CribbageCalculations.score_hand(gamePlayers[playerID].trueHandCardIndex, cut, false);

                List<int> cardIDs = gamePlayers[playerID].trueHandCardIndex;

                RpcDisplayScoredHand(cardIDs[0], cardIDs[1], cardIDs[2], cardIDs[3]);



                print(CribbageCalculations.score_15s(gamePlayers[playerID].trueHandCardIndex, cut));
                print(CribbageCalculations.score_pairs(gamePlayers[playerID].trueHandCardIndex, cut));
                print(CribbageCalculations.score_runs(gamePlayers[playerID].trueHandCardIndex, cut));
                print(CribbageCalculations.score_flush(gamePlayers[playerID].trueHandCardIndex, cut, false));
                print(CribbageCalculations.score_nobs(gamePlayers[playerID].trueHandCardIndex, cut));
                print(score);

                IncreaseScore(playerID, score);

                if (isCrib && cribCardIDList.Count == 4)
                {
                    score = CribbageCalculations.score_hand(cribCardIDList, cut, false);

                    print(CribbageCalculations.score_15s(cribCardIDList, cut));
                    print(CribbageCalculations.score_pairs(cribCardIDList, cut));
                    print(CribbageCalculations.score_runs(cribCardIDList, cut));
                    print(CribbageCalculations.score_flush(cribCardIDList, cut, false));
                    print(CribbageCalculations.score_nobs(cribCardIDList, cut));
                    print(score);

                    IncreaseScore(playerID, score);
                }
            }

            yield return new WaitForSeconds(5f);



            RpcFlipTopCard();            
        }
    }

    void ReshuffleDeck(int amountOfDecks)
    {
        cardsInDeck.Clear();

        for (int i = 0; i < amountOfDecks; i++)
        {
            foreach (var card in CribbageCardDatabase.instance.CardDatabase)
            {
                cardsInDeck.Add(card);
            }
        }

        //Call client side stuff based on how big the pile is
        RpcReshuffleDeck(cardsInDeck.Count);
    }

    [ClientRpc]
    void RpcReshuffleDeck(int _deckSize)
    {
        deckSize = _deckSize;
    }

    [Server]
    public void IncreaseCount(int _count)
    {
        count += _count;
    }

    List<int> GetRandomCardsInDeck(int amount)
    {
        List<int> tempList = new List<int> ();

        for (int i = 0; i < amount; i++)
        {
            if(cardsInDeck.Count == 0)
            {
                ReshuffleDeck(1);
            }

            int random = UnityEngine.Random.Range(0, cardsInDeck.Count);

            tempList.Add(cardsInDeck[random].GetID());
            cardsInDeck.RemoveAt(random);
        }

        return tempList;
    }

    int GetRandomCardInDeck()
    {
        int random = UnityEngine.Random.Range(0, cardsInDeck.Count);

        return cardsInDeck[random].GetID();
    }

        List<int> GetNewTurnOrder(int dealerID)
    {
        List<int> newPlayerOrder = new List<int>();

        for (int i = 0; i < gamePlayers.Count; i++)
        {
            newPlayerOrder.Add((i + dealerID + 1) % gamePlayers.Count);
        }

        return newPlayerOrder;
    }

    List<int> GetNewTurnOrder()
    {
        CribbageGamePlayer dealer = null;
        int dealerID = 0;

        for (int i = 0; i < gamePlayers.Count; i++)
        {
            if (gamePlayers[i].dealer)
            {
                dealer = gamePlayers[i];
                dealerID = i;
            }
        }

        List<int> newPlayerOrder = new List<int>();

        for(int i = 0; i < gamePlayers.Count; i++)
        {
            newPlayerOrder.Add((i + dealerID + 1) % gamePlayers.Count);
        }


        return newPlayerOrder;
    }

    [Server]
    public void SetScore(int carIndex, int score)
    {
        playerCars[carIndex].SetPositionInLane(score);
    }

    [Server]
    public void IncreaseScore(int carIndex, int score)
    {
        playerCars[carIndex].SetPositionInLane(playerCars[carIndex].position + score);        
    }

    [ClientRpc]
    public void RpcSetDiscardMode(bool enabled)
    {
        CribbageUI.Instance.SetDiscardMode(enabled);
        CribbageUI.Instance.ShowTut(!enabled);
    }

    [ClientRpc]
    public void RpcResetBoardUI()
    {
        CribbageUI.Instance.ResetFieldUI();
    }

    [ClientRpc]
    public void RpcSetCutCard(int cardID)
    {
        deck.SetTopCard(cardID);
        deck.FlipTopCard();
    }

    [ClientRpc]
    public void RpcFlipTopCard()
    {
        deck.FlipTopCard();
    }

    [ClientRpc]
    public void RpcDisplayScoredHand(int cardID1, int cardID2, int cardID3, int cardID4)
    {


        //Display the hand
        
    }

    [Server]
    public void DiscardCard(int cardID)
    {
        cribCardIDList.Add(cardID);
    }
}
