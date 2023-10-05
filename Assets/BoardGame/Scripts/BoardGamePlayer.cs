using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BoardGamePlayer : NetworkBehaviour
{
    public bool IsReady = false;

    public AzulBoard board;
    public AzulTile selectedTile;
    public int selectedQueue;

    [Client]
    public void SelectTile(AzulTile tile)
    {
        selectedTile = tile;
    }

    //UnselectTile

    [ClientRpc]
    public void RpcSetBoard()
    {
        if (!isOwned)
            return;

        foreach(var board in FindObjectsOfType<AzulBoard>())
        {
            if(board.isOwned)
            {
                this.board = board;
            }
        }
    }

    [Client]
    public void SelectQueue(int queueIndex)
    {
        if(selectedTile)
        {
            CmdSelectQueue(AzulGameManager.instance.GetIndexOfTile(selectedTile), queueIndex);
        }
    }

    [Command]
    public void CmdSelectQueue(int tileIndex, int queueIndex)
    {
        ServerSelectQueue(tileIndex, queueIndex);
    }

    [Server]
    public void ServerSelectQueue(int tileIndex, int queueIndex)
    {
        AzulTile playerSelectedTile = AzulGameManager.instance.tileDatabase[tileIndex];

        var tiles = playerSelectedTile.Island.GetTilesByType(playerSelectedTile.TileType);

        board.AddTilesToQueue(tiles, queueIndex);
    }
}