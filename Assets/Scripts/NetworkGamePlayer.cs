using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class NetworkGamePlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(DisplayNameHook))]
    private string displayName = "Loading...";

    [Header("Player's Local Space")]
    [SerializeField] private GameObject localSpace;
    [Header("Player model that other clients see")]
    [SerializeField] private GameObject clientPlayerObject;

    [Header("Temp space for NameText")]
    [SerializeField] private TextMeshProUGUI playerNameText;

    private NetworkLobbyManager lobby;
    private NetworkLobbyManager Lobby
    {
        get
        {
            if (lobby != null) { return lobby; }
            return lobby = NetworkManager.singleton as NetworkLobbyManager;
        }
    }

    private void Start()
    {
        DisplayNameHook("", displayName);
    }

    public override void OnStartLocalPlayer()
    {
        if(localSpace != null)
            SetGameLayerRecursive(localSpace, LayerMask.NameToLayer("Gun"));
        //SetMeshRendererRecursive(clientPlayerObject, false);
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);

        Lobby.GamePlayers.Add(this);
    }

    public override void OnStopServer()
    {
        Lobby.GamePlayers.Remove(this);
    }

    public void DisplayNameHook(string _oldString, string _newString)
    {
        displayName = _newString;

        if(playerNameText != null)
            playerNameText.text = displayName;
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

    private void SetGameLayerRecursive(GameObject _go, int _layer)
    {
        _go.layer = _layer;
        foreach (Transform child in _go.transform)
        {
            child.gameObject.layer = _layer;

            Transform _HasChildren = child.GetComponentInChildren<Transform>();
            if (_HasChildren != null)
                SetGameLayerRecursive(child.gameObject, _layer);

        }
    }

    void SetMeshRendererRecursive(GameObject _go, bool enabled)
    {
        var renderer = _go.GetComponent<SkinnedMeshRenderer>();

        if (renderer)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }

        foreach (Transform child in _go.transform)
        {
            Transform _HasChildren = child.GetComponentInChildren<Transform>();
            if (_HasChildren != null)
                SetMeshRendererRecursive(child.gameObject, enabled);

        }
    }

    [TargetRpc]
    public virtual void Init()
    {
    } 

    public string GetDisplayName()
    {
        return displayName;
    }
}
