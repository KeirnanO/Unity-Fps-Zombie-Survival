using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;

public enum TileType
{
    blue = 0,
    yellow = 1,
    red = 2,
    black = 3,
    white = 4
}

public enum TileState 
{ 
    Free = 0,
    InPlay = 1,
    OnTheBoard = 2,
    Trashed = 3
}

public class AzulTile : NetworkBehaviour
{
    [SerializeField] TileType tileType;
    //Only the server will care about the tileState
    [SerializeField] TileState tileState;
    [SerializeField] Color color;
    [SerializeField] Color HighlightColor;
    [SerializeField] AzulIsland island;
    public AzulIsland Island { get { return island; } }

    public TileType TileType
    {
        get
        {
            return tileType;
        }
    }
    public TileState TileState
    {
        get
        {
            return tileState;
        }
        set
        {
            tileState = value;
        }
    }

    private void Start()
    {
        enabled = true;
        SetRegularColor();
    }

    public Color GetColor()
    {
        return color;
    }

    public void SetHighlightColor()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = HighlightColor;
    }

    public void SetRegularColor()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = color;
    }

    private void OnMouseDown()
    {
        foreach(var player in FindObjectsOfType<BoardGamePlayer>())
        {
            player.SelectTile(this);
        }
    }

    private void OnMouseEnter()
    {
        if (island != null)
        {
            foreach(var tile in island.GetTilesByType(tileType))
            {
                tile.SetHighlightColor();
            }
        }
    }

    private void OnMouseExit()
    {
        if (island != null)
        {
            foreach (var tile in island.GetTilesByType(tileType))
            {
                tile.SetRegularColor();
            }
        }
    }

    public void SetIsland(AzulIsland _island)
    {
        island = _island;
    }

    [ClientRpc]
    public void RpcSetIsland()
    {

    }

}
