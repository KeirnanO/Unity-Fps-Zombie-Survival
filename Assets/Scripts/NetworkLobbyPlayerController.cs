using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class NetworkLobbyPlayerController : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] private Graphic[] playerReadyIcon = new Graphic[4];
    [SerializeField] private Button startGameButton = null;

    public Color readyColour;
    public Color notReadyColor;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyChanged))]
    public bool IsReady = false;
    private bool isLeader;

    public bool IsLeader 
    { 
        set 
        { 
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        } 
    }

    

    private NetworkLobbyManager lobby;
    private NetworkLobbyManager Lobby
    {
        get
        {
            if(lobby != null) { return lobby; }
            return lobby = NetworkManager.singleton as NetworkLobbyManager;
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerPrefs.GetString("PlayerName"));
        CmdSetReadyState(false);

        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        Lobby.LobbyPlayers.Add(this);

        UpdateDisplay();
    }

    public override void OnStopServer()
    {
        Lobby.LobbyPlayers.Remove(this);

        UpdateDisplay();
    }

    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandleReadyChanged(bool oldValue, bool newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if(!isOwned)
        {
            foreach(var player in Lobby.LobbyPlayers)
            {
                if (player.isOwned)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for(int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting For Player...";
            playerReadyIcon[i].gameObject.SetActive(false);
        }

        for(int i = 0; i < Lobby.LobbyPlayers.Count; i++)
        {
            playerNameTexts[i].text = Lobby.LobbyPlayers[i].DisplayName;
            playerReadyIcon[i].gameObject.SetActive(true);
            playerReadyIcon[i].color = Lobby.LobbyPlayers[i].IsReady ? readyColour : notReadyColor;
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if(!isLeader) { return; }

        startGameButton.interactable = readyToStart;
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    [Command]
    private void CmdSetReadyState(bool state)
    {
        IsReady = state;
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        Lobby.NotifyPlayersOfReadyState();
    }


    [Command]
    public void CmdStartGame()
    {
        if (Lobby.LobbyPlayers[0].connectionToClient != connectionToClient) { return; }

        Lobby.StartGame();
    }
}
