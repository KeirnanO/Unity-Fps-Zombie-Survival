using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;
using System;

public class FightPlayer : NetworkBehaviour
{
    //Player's Cards
    [SerializeField] private PlayerDeck deck;
    //[SerializeField] readonly private SyncList<CardScriptableObject> FightingDeck;
    //[SerializeField] readonly private SyncList<CardScriptableObject> FightingHand;
    [SerializeField] private float maxHandSize;

    //UI
    [SerializeField] private GameObject FightUI;
    [SerializeField] private PlayerHand playerHandUI;
    [SerializeField] private List<PlayerDisplay> allyDisplays;
    [SerializeField] private List<PlayerDisplay> enemyDisplays;

    //The Zone The Player Is In
    [SerializeField] private FightingZone zone;

    //Reference to the PlayerCamera
    [SerializeField] private CinemachineVirtualCamera followCam;

    [SerializeField] private Animator anim;

    [SyncVar(hook = nameof(SelectedCardChange))]
    public int selectedCardID;
    public int selectedCardHandIndex;

    [SyncVar(hook = nameof(TargetChange))]
    public int targetId;
    [SyncVar(hook = nameof(ReadyChange))]
    public bool ready;

    public Mana mana;
    public float extraManaChance = 1/3;

    private void SelectedCardChange(int _oldvalue, int _newvalue)
    {
    }
    private void TargetChange(int _oldvalue, int _newvalue)
    {
    }
    private void ReadyChange(bool _oldvalue, bool _newvalue)
    {
    }

    [Server]
    public void JoinFight(NetworkIdentity fightID, Vector3 position, Quaternion rotation)
    {
        zone = fightID.GetComponent<FightingZone>();
        RpcJoinFight(fightID, position, rotation);
    }

    [ClientRpc]
    public void RpcJoinFight(NetworkIdentity fightID, Vector3 position, Quaternion rotation)
    {        
        if (!isOwned)
            return;

        zone = fightID.GetComponent<FightingZone>();

        followCam.enabled = false;

        //zone.OnPlayerSelectCard += UpdatePlayerDisplays;

        GetComponent<NetworkMovementController>().enabled = false;
        GetComponent<NetworkCameraController>().enabled = false;
    }

    [ClientRpc]
    public void LeaveFight(NetworkIdentity fightID)
    {
        if (!isOwned)
            return;

        followCam.enabled = true;
        zone.PlanningCamera.enabled = false;
        zone = null;
        
    }

    [ClientRpc]
    public void RpcEnableFightUI()
    {
        if (!isOwned)
            return;

        EnableFightUI(true);
    }

    [Client]
    public void EnableFightUI(bool enabled)
    {
        FightUI.SetActive(enabled);
        playerHandUI.SetActive(enabled);

        if(enabled)
            //playerHandUI.SetCardDisplay(FightingHand.CopyTo<>);

        UpdatePlayerDisplays();
    }

    [ClientRpc]
    public void DisableFightUI()
    {
        if (!isOwned)
            return;

        FightUI.SetActive(false);
    }

    [Client]
    void UpdatePlayerDisplays()
    {
        if (!zone)
            return;

        var allies = zone.TeamAEntities;
        var enemies = zone.TeamBEntities;

        for (int i = 0; i < allyDisplays.Count; i++)
        {
            if (i > allies.Count - 1)
            {
                allyDisplays[i].gameObject.SetActive(false);
                continue;
            }

            if (allies[i] == null)
            {
                allyDisplays[i].gameObject.SetActive(false);
                continue;
            }

            allyDisplays[i].gameObject.SetActive(true);
            allyDisplays[i].SetPlayerDisplay(allies[i].player.GetComponent<NetworkGamePlayer>().GetDisplayName(), allies[i].health, allies[i].mana);
        }

        for (int i = 0; i < enemyDisplays.Count; i++)
        {
            if (i > enemies.Count - 1)
            {
                enemyDisplays[i].gameObject.SetActive(false);
                continue;
            }

            if (enemies[i] == null)
            {
                enemyDisplays[i].gameObject.SetActive(false);
                continue;
            }

            enemyDisplays[i].gameObject.SetActive(true);
            enemyDisplays[i].SetPlayerDisplay(enemies[i].monster.transform.name, enemies[i].health, enemies[i].mana);
        }
    }

    [Server]
    public void ServerSetUpRound()
    {
        selectedCardID = -1;
        selectedCardHandIndex = -1;
        targetId = -1;
    }

    [ClientRpc]
    public void RpcShuffleDeck()
    {
        if (!isOwned)
            return;

        ShuffleDeck();
    }

    [Command]
    public void CmdSelectCard(int cardID)
    {
        selectedCardID = cardID;

        if (CardDatabase.instance.cardList[cardID].AOE)
        {
            ready = true;
            targetId = 8;
            //zone.PlayerSelectCard();
        }
    }

    [Client]
    public void ShuffleDeck()
    {
        //FightingDeck.Clear();
        //FightingDeck = new List<CardScriptableObject>(deck.deck);
        //FightingDeck.AddRange(GetComponent<Equipment>().GetBonusCards());
    }

    [ClientRpc]
    public void RpcDraw()
    {
        if (!isOwned)
            return;

        DrawCard();
        //playerHandUI.SetCardDisplay(FightingHand);
    }

    [Client]
    void DrawCard()
    {
        //if (FightingDeck.Count == 0)
           // return;

        //if (FightingHand.Count >= 7)
            //return;

        //CardScriptableObject drawnCard = FightingDeck[UnityEngine.Random.Range(0, FightingDeck.Count)];

        //FightingHand.Add(drawnCard);
        //FightingDeck.Remove(drawnCard);

        //Recursively draw until we can no longer draw
        DrawCard();
    }

    [Client]
    public void SelectTarget(int index)
    {
        //If we already have a target selected dont select a new target
        if (ready)
            return;

        //If a card is already selected send info to server - cardID and targetIDs
        if (!playerHandUI.selected)
            return;


        CmdChooseTarget(index);

        //Turn off card slection object
        //Turn on waiting object - with button to unselect card
    }

    [Command]
    public void CmdChooseTarget(int index)
    {
        targetId = index;
        ready = true;
        //zone.PlayerSelectCard();
    }

    [ClientRpc]
    public void RpcChooseTargetCallBack()
    {
        if (!isOwned)
            return;

        playerHandUI.SetActive(false);
    }


    /*
[Client]
public bool SelectCard(int index)
{
if (!mana.CheckCost(fightingHand[index].manaCost))
return false;

selectedCardHandIndex = index;
CardScriptableObject selectedCard = fightingHand[index];
CmdSelectCard(selectedCard.cardID);

return true;
}





[Server]
public void PlayCard()
{
//mana -= fightingHand[selectedCardHandIndex].manaCost;

RpcPlayCard();
}

[ClientRpc]
public void RpcPlayCard()
{
if (!isOwned)
return;

fightingHand.Remove(fightingHand[selectedCardHandIndex]);
}

public void RemoveCard(int index)
{
fightingHand.RemoveAt(index);
}

public void AddCard(int index, CardScriptableObject card)
{
fightingHand.Insert(index, card);
}

public void SetCards()
{
playerHandUI.SetCardDisplay(fightingHand);
}
*/

    [ClientRpc]
    public void ChangeCamera(bool battle)
    {
        if (!isOwned)
            return;

        if (!zone)
            return;

        zone.ChangeCamera(battle);
    }

    [Server]
    public void GiveMana()
    {
        if(UnityEngine.Random.Range(0.0f, 1.0f) <= extraManaChance)
        {
            print("Extra Mana!!");
            //mana++;
        }

        //mana++;
    }
}
