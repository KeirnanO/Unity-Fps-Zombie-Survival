using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;

public class NetworkLobbyManager : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [SerializeField] private string menuScene = string.Empty;
    [SerializeField] private string gameScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkLobbyPlayerController lobbyPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayer gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;
    [SerializeField] private GameObject chestSpawnSystem = null;
    [SerializeField] private GameObject zombieSpawnSystem = null;
    [SerializeField] private GameObject enemySpawnSystem = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnectionToClient> OnServerReadied;
     
    public List<NetworkLobbyPlayerController> LobbyPlayers { get; } = new List<NetworkLobbyPlayerController>();
    public List<NetworkGamePlayer> GamePlayers;//{ get; } = new List<NetworkGamePlayer>();

    public override void OnClientConnect()
    {   
        base.OnClientConnect();

        if (!clientLoadedScene)
        {
            OnClientConnected?.Invoke();
        }
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkLobbyPlayerController>();

            LobbyPlayers.Remove(player);
        }   
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        //Joined after the server has started
        if (SceneManager.GetActiveScene().name != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if(SceneManager.GetActiveScene().name == menuScene)
        {
            bool isLeader = LobbyPlayers.Count == 0;

            NetworkLobbyPlayerController lobbyPlayerInstance = Instantiate(lobbyPlayerPrefab);

            lobbyPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, lobbyPlayerInstance.gameObject);
        }
    }    

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in LobbyPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    public bool IsReadyToStart()
    {
        if(numPlayers < minPlayers) { return false; }

        foreach(var player in LobbyPlayers)
        {
            if(!player.IsReady) { return false; }
        }

        return true;
    }

    public void StartGame()
    {
        if(SceneManager.GetActiveScene().name == menuScene)
        {
            if(!IsReadyToStart()) { return; }

            ServerChangeScene(gameScene);
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {

        if (SceneManager.GetActiveScene().name == menuScene && newSceneName.StartsWith("Scene_"))
        {
            for (int i = LobbyPlayers.Count - 1; i >= 0; i--)
            {
                var conn = LobbyPlayers[i].connectionToClient;
                var gamePlayerInstance = Instantiate(gamePlayerPrefab);
                gamePlayerInstance.SetDisplayName(LobbyPlayers[i].DisplayName);

                NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject);

                conn.isReady = false;
            }
        }
        
        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if (sceneName.StartsWith("Scene_Map"))
        {
            //GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            //NetworkServer.Spawn(playerSpawnSystemInstance);

            //GameObject chestSpawnSystemInstance = Instantiate(chestSpawnSystem);
            //NetworkServer.Spawn(chestSpawnSystemInstance);

            //Only Server should handle zombie spawning
            //GameObject zombieSpawnSystemInstance = Instantiate(zombieSpawnSystem);
            //NetworkServer.Spawn(zombieSpawnSystemInstance);

            if(sceneName.Contains("Factory"))
            {
                GameObject enemySpawnSystemInstance = Instantiate(enemySpawnSystem);
            }
        }
    }


    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);


        if (CheckGameReady())
        {
            //Crib game
            //CribbageCarGameManager.instance.StartGameLoop();

            for (int i = GamePlayers.Count - 1; i >= 0; i--)
            {
                //RPG Games
                //GamePlayers[i].GetComponent<NetworkMovementController>().RpcSetPositionAndRotation(NetworkPlayerSpawnSystem.spawnPoints[i].position, NetworkPlayerSpawnSystem.spawnPoints[i].rotation);

                //Board Game
                //AzulGameManager.instance.SetUpGame();
                //
                GamePlayers[i].Init();
            }
        }
    }

    private bool CheckGameReady()
    {
        if(GamePlayers.Count == 0)
        {
            print("No Players in the Game");
            return false;
        }

        for(int i = 0; i < GamePlayers.Count; i++)
        {         
            var conn = GamePlayers[i].connectionToClient;
            if (!conn.isReady) return false;
            else
            {
                print("GamePlayer " + (i  + 1) + " is ready / " + GamePlayers.Count);
            }

        }


        print("All Players Ready");
        return true;
    }


}
