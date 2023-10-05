using Cinemachine;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleManager : NetworkBehaviour
{
    enum FightState
    {
        None,
        SetupPhase,
        CardPhase,
        ActionPhase
    }

    [SerializeField] private bool fighting;

    [SerializeField] private FightingZone fightingZone;
    [SerializeField] private List<FightPlayer> RealPlayers;

    [SerializeField] private FightState state;

    private float fightStartTimeDelay = 1.5f;
    private float fightStartTimeoutDelta;

    public CinemachineVirtualCamera BattleCamera;
    public CinemachineVirtualCamera PlanningCamera;

    List<GameObject> createdBattleObjects;

    [Server]
    public void ServerAddPlayerToFight(bool teamA, NetworkIdentity clientIdentity)
    {
        //If not a player 
        if (clientIdentity.GetComponent<FightPlayer>() == null)
            return;

        FightPlayer newFightPlayer = clientIdentity.GetComponent<FightPlayer>();

        //If player is already in battle
        if (RealPlayers.Contains(newFightPlayer))
            return;

        int spot = fightingZone.TeamAEntities.Count;

        if (spot >= fightingZone.TeamAPositions.Length)
            return;

        if (state == FightState.None)
        {
            //GraphicOn(true);
            fighting = true;
            state = FightState.SetupPhase;
        }

        //Create an Entity that is added to the zone - syncs on all clients
        FightEntity newFightEntity = new FightEntity(newFightPlayer);
        fightingZone.TeamAEntities.Add(newFightEntity);

        //Create a list of all players in the battle
        RealPlayers.Add(newFightPlayer);

        //Force player into fight
        newFightPlayer.JoinFight(netIdentity, fightingZone.TeamAPositions[spot].position, fightingZone.TeamAPositions[spot].rotation);

        //Add extra time before a round begins when a player joins the fight before it starts
        if (state == FightState.SetupPhase)
        {
            fightStartTimeoutDelta = Time.time + fightStartTimeDelay;
        }

        //If joined during card selection - add player to battle on all clients
        if (state == FightState.CardPhase)
        {
            //Reset Camera to PlanningCamera
            TellClientChangeCamera(false);

            //Setup Round for the new player
            clientIdentity.GetComponent<Mana>().AddMana(0);
            clientIdentity.GetComponent<FightPlayer>().ServerSetUpRound();
            clientIdentity.GetComponent<FightPlayer>().RpcShuffleDeck();
        }
    }


    [Server]
    public void ServerAddMonsterToFight(NetworkIdentity clientIdentity)
    {
        int spot = fightingZone.TeamBEntities.Count;

        if (spot < fightingZone.TeamBPositions.Length)
        {
            CardBattleEnemyController enemyController = clientIdentity.GetComponent<CardBattleEnemyController>();
            FightEntity newEntity = new FightEntity(enemyController);

            //Add To repective Lists
            fightingZone.TeamBEntities.Add(newEntity);

            //Setup for fight
            clientIdentity.transform.SetPositionAndRotation(fightingZone.TeamBPositions[spot].position, fightingZone.TeamBPositions[spot].rotation);
        }
    }

    [ServerCallback]
    private void Update()
    {
        switch (state)
        {
            //No Fight In Progress
            case FightState.None:
                break;

            //Players Just Joining the Fight
            case FightState.SetupPhase:
                //Will set state to CardPhase the same from this passes
                if (Time.time > fightStartTimeoutDelta) { SetUpFight(); }
                break;

            //Players Chooosing what card to play
            case FightState.CardPhase:
                //Set all zones to fightState;
                //If all Players have selectedCards
                //Move to action phase
                break;

            //Playing out the action of this turn
            case FightState.ActionPhase:
                break;
        }
    }

    //Server will begin the process to starting the fight
    [Server]
    void SetUpFight()
    {
        StartCardPhase();
    }

    //The beginning phase of each round allowing players freedom of the fightUI
    //The UI will consist of cards the player can choose and a few options like drawing and passing
    //The Server's State of the fight will be set to the CardPhaseState until all players have made a choice
    [Server]
    void StartCardPhase()
    {
        //Reset Camera to PlanningCamera
        TellClientChangeCamera(false);
                
        //Grab each RealPlayer in the zone and relay the new start of a round to them
        //Include information like drawing cards and setting up the UI for each client
        foreach (var entity in fightingZone.TeamAEntities)
        {
            entity.mana.AddMana(0);

            if (entity.player != null)
            {
                entity.player.ServerSetUpRound();
            }
        }

        foreach (var entity in fightingZone.TeamBEntities)
        {
            entity.mana.AddMana(0);

            if (entity.player != null)
            {
                entity.player.ServerSetUpRound();
            }
        }

        foreach (var player in RealPlayers)
        {
            player.RpcEnableFightUI();
            player.ServerSetUpRound();
            player.RpcShuffleDeck();
            player.RpcDraw();
        }

        //Server updates based off the CardPhase logic
        state = FightState.CardPhase;
    }

    //Client tells the server the card they picked and the target if there is one
    //Server will replicate this information back to all clients -- this will cause a UI update to let everyone know this players intentions
    [Server] 
    public void PlayerSelectCard()
    {
        RpcPlayerSelectCard();

        //Return out if a player isnt ready
        foreach (FightPlayer player in RealPlayers)
        {
            if (!player.ready)
                return;
        }

        //If all players ready, move to action phase
        StartActionPhase();
    }

    [ClientRpc]
    public void RpcPlayerSelectCard()
    {
        //OnPlayerSelectCard?.Invoke();
    }

    //Moving to the ActionPhase will turn off the each client's UI and prep the  
    [Server]
    public void StartActionPhase()
    { 
        TellClientChangeCamera(true);

        state = FightState.ActionPhase;

        StartCoroutine(ActionPhaseCoroutine());
    }

    [Server]
    void TellClientChangeCamera(bool battle)
    {
        foreach (var player in RealPlayers)
        {
            player.ChangeCamera(battle);
        }
    }

    IEnumerator ActionPhaseCoroutine()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 2; i++)
        {
            var Team = i == 0 ? fightingZone.TeamAEntities : fightingZone.TeamBEntities;

            //Tracks whos turn it currently is
            int currentTurnPlayerIndex = 0;

            //Loop through each players turn
            foreach (var activeEntity in Team)
            {
                //Skip turn if dead
                if (activeEntity.IsDead())
                    continue;

                //Turn Properties
                int cardID = -1;
                int targetID = -1;

                //If we are a player set values accordingly
                if (activeEntity.player != null)
                {
                    cardID = activeEntity.player.selectedCardID;
                    targetID = activeEntity.player.targetId;
                }
                //If we are a monster set values accordingly
                else if (activeEntity.monster != null)
                {
                    activeEntity.monster.ChooseCard();

                    cardID = activeEntity.monster.cardIndex;
                    targetID = activeEntity.monster.targetIndex;
                }

                //If no card is selected skip turn
                if (cardID < 0)
                    continue;

                //If target is an enemy - flag as enemy spell and reduce id by 4
                var targetedTeam = Team;

                if (targetID > 3)
                {
                    targetID -= 4;
                    targetedTeam = GetOppositeTeam(Team);
                }

                //Attack Properties
                CardScriptableObject card = CardDatabase.instance.cardList[cardID];
                int damage = card.attack;
                float damageMultiplyer = 1.0f;

                switch (card.cardType)
                {
                    case CardType.Basic:
                        break;
                    case CardType.Attack:
                        //Check For Charms
                        List<FightEffect> playerCharms = fightingZone.TeamAEntities[currentTurnPlayerIndex].GetEffectsByType(EffectType.Charm);
                        //Use All Charms
                        while (playerCharms.Count > 0)
                        {
                            FightEffect curCharm = playerCharms[0];

                            damageMultiplyer *= (float)curCharm.strength / 100;

                            fightingZone.TeamAEntities[currentTurnPlayerIndex].RemoveEffect(playerCharms[0]);
                            playerCharms.RemoveAt(0);

                            yield return new WaitForSeconds(0.5f);

                            NetworkServer.Destroy(curCharm.gameObject);

                            yield return new WaitForSeconds(0.5f);
                        }

                        // Multiply damage by charm multiplyer and eventually traps aswell
                        int damageDealt = Mathf.CeilToInt(damage * damageMultiplyer);

                        //Holds all animations created during this attack
                        List<AttackAnimation> attackAnimations = new List<AttackAnimation>();

                        //AOE Attack 
                        if (card.AOE)
                        {
                            foreach (var enemy in targetedTeam)
                            {
                                if (enemy == null)
                                    continue;

                                enemy.health.GetComponent<Damageable>().DealDamage(damageDealt);
                                activeEntity.mana.RemoveMana(CardDatabase.instance.cardList[cardID].manaCost);

                                //Spawn Attack Animation
                                attackAnimations.Add(SpawnAttackAnimation(cardID, enemy.health.transform, damageDealt));
                            }
                        }
                        //SingleTarget Attack
                        else
                        {
                            if (targetedTeam[targetID] == null)
                                continue;

                            AttackEntity(false, targetID, damageDealt);

                            activeEntity.mana.RemoveMana(CardDatabase.instance.cardList[cardID].manaCost);

                            //Spawn Attack Animation
                            attackAnimations.Add(SpawnAttackAnimation(cardID, targetedTeam[targetID].health.transform, damageDealt));
                        }

                        //Wait for all animations to conclude
                        while (attackAnimations.Count > 0)
                        {
                            if (!attackAnimations[0].animating)
                                attackAnimations.RemoveAt(0);

                            yield return null;
                        }

                        //Buffer and destroy animation objects on all clients
                        yield return new WaitForSeconds(1f);
                        foreach (var animation in attackAnimations)
                        {
                            NetworkServer.Destroy(animation.gameObject);
                        }
                        break;

                    case CardType.Heal:
                        break;

                    case CardType.Charm:
                        //Spawn Charm Object
                        var charm = Instantiate(CardDatabase.instance.cardList[cardID].animationObject, transform.position, Quaternion.identity);
                        charm.transform.position = fightingZone.TeamAPositions[targetID].transform.position + Vector3.up + (Vector3.forward * 0.2f);
                        NetworkServer.Spawn(charm);

                        //Add Effect
                        FightEffect charmEffect = charm.GetComponent<FightEffect>();
                        targetedTeam[targetID].AddEffect(charmEffect);
                        break;

                    case CardType.Ward:
                        //Spawn Ward Object
                        var ward = Instantiate(CardDatabase.instance.cardList[cardID].animationObject, transform.position, Quaternion.identity);
                        ward.transform.position = fightingZone.TeamAPositions[targetID].transform.position + Vector3.up + (Vector3.forward * 0.2f);
                        NetworkServer.Spawn(ward);

                        //Add Effect
                        FightEffect wardEffect = ward.GetComponent<FightEffect>();
                        targetedTeam[targetID].AddEffect(wardEffect);
                        break;
                }

                DestroyDead();

                yield return new WaitForSeconds(1f);

                currentTurnPlayerIndex++;
            }

            yield return new WaitForSeconds(1f);
        }

        //TODO: Ending turns / starting turns

        EndRound();
    }

    [Server]
    void AttackEntity(bool teamA, int entityIndex, int damage)
    {
        var TeamEntities = teamA ? fightingZone.TeamAEntities : fightingZone.TeamBEntities;

        //Use Traps
        TeamEntities[entityIndex].health.GetComponent<Damageable>().DealDamage(damage);
    }

    [Server]
    AttackAnimation SpawnAttackAnimation(int cardID, Transform target, int damage)
    {
        var go = Instantiate(CardDatabase.instance.cardList[cardID].animationObject, transform.position, Quaternion.identity);
        go.transform.LookAt(target, Vector3.up);
        var animation = go.GetComponent<AttackAnimation>();
        animation.damage = damage;

        NetworkServer.Spawn(go);

        return animation;
    }

    /// <summary>
    /// This will only effect monsters gameObjects for now
    /// </summary>
    [Server]
    void DestroyDead()
    {
        for (int i = 0; i < fightingZone.TeamBEntities.Count; i++)
        {
            if (fightingZone.TeamBEntities[i] == null)
                continue;

            if (fightingZone.TeamBEntities[i].IsDead())
            {
                NetworkServer.Destroy(fightingZone.TeamBEntities[i].monster.gameObject);
                fightingZone.TeamBEntities[i] = null;
            }
        }
    }

    [Server]
    void EndRound()
    {
        bool alive = false;

        foreach (var entity in fightingZone.TeamBEntities)
        {
            if (entity != null)
            {
                alive = true;
                break;
            }
        }

        if (!alive)
        {
            fighting = false;

            foreach (var player in RealPlayers)
            {
                player.GetComponent<FightPlayer>().LeaveFight(netIdentity);
                player.GetComponent<NetworkMovementController>().RpcSetEnabled(true);
                player.GetComponent<NetworkCameraController>().RpcSetEnabled(true);
            }

            fightingZone.TeamAEntities.Clear();
            fightingZone.TeamBEntities.Clear();

            RealPlayers.Clear();
            state = FightState.None;
        }
        else
        {
            StartCardPhase();
        }
    }

    [Server]
    SyncList<FightEntity> GetOppositeTeam(SyncList<FightEntity> AllyTeam)
    {
        if (fightingZone.TeamAEntities.Equals(AllyTeam))
            return fightingZone.TeamBEntities;

        return fightingZone.TeamAEntities;
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        print("Trying to add " + other.name + " to the fight");

        //If the fight has not started, we can not add a player to the fight
        if (!fighting)
            return;

        //If the player aready exists in this fight we do not add the player
        if (other.gameObject.CompareTag("Player"))
        {
            if (RealPlayers.Contains(other.GetComponent<FightPlayer>()))
                return;

            ServerAddPlayerToFight(true, other.GetComponent<NetworkIdentity>());
        }
    }

    void OnPlayerDeath(NetworkIdentity netid)
    {

    }
}
