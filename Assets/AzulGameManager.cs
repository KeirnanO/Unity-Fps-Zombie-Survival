using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class AzulGameManager : NetworkBehaviour
{
    static public AzulGameManager instance;

    private void Awake()
    {
        instance = this;
    }

    public List<NetworkGamePlayer> Players
    {
        get
        {
            return FindObjectOfType<NetworkLobbyManager>().GamePlayers;
        }
    }

    public GameObject islandPrefab;
    public List<AzulIsland> islands = new List<AzulIsland>();
    public Transform[] islandPositions;

    public AzulBoard boardPrefab;
    public Transform[] boardPositions;
    private List<AzulBoard> boards = new List<AzulBoard>();

    public AzulTile[] tileDatabase;

    //Client Calls
    [Command(requiresAuthority = true)]
    public void CmdSelectTile(NetworkIdentity idetity)
    {
        ServerSelectTile(idetity);
    }


    //ServerCalls
    [Server]
    public void ServerSelectTile(NetworkIdentity identity)
    {

    }

    [Server]
    public void SetUpGame()
    {
        foreach (var islandPosition in islandPositions)
        {
            GameObject newIsland = Instantiate(islandPrefab, islandPosition.position, Quaternion.identity);

            islands.Add(newIsland.GetComponent<AzulIsland>());

            NetworkServer.Spawn(newIsland);
        }

        int index = 0;
        foreach (var player in Players)
        {
            var newBoard = Instantiate(boardPrefab, boardPositions[index].position, boardPositions[index].rotation);
            newBoard.SetPlayer(player.netIdentity);
            player.GetComponent<BoardGamePlayer>().RpcSetBoard();
            boards.Add(newBoard);
            index++;

            NetworkServer.Spawn(newBoard.gameObject, player.gameObject);
        }

        foreach(var island in islands)
        {
            for(int i =0; i < 4; i++)
            {
                var tile = GetRandomTile();
                tile.TileState = TileState.InPlay;
                tile.transform.SetPositionAndRotation(island.transform.position + (Vector3.up * 2), Quaternion.identity);
                tile.SetIsland(island);
                island.tiles.Add(tile);
                tile.GetComponent<Rigidbody>().AddForce(new Vector3(0.2f * i, 1.5f * i, 0.8f * i), ForceMode.Impulse);
            }
        }
    }


    [Server]
    public void ShuffleTiles()
    {
        foreach(var tile in tileDatabase)
        {
            if(tile.TileState != TileState.OnTheBoard)
            {
                //move tile off the screen
                tile.transform.position = new Vector3(10, 3, 0);
                tile.TileState = TileState.Free;
            }
        }
    }

    public AzulTile GetRandomTile()
    {
        List<AzulTile> freeTiles = new List<AzulTile>();

        foreach(var tile in tileDatabase)
        {
            if(tile.TileState == TileState.Free)
                freeTiles.Add(tile);
        }


        int random = Random.Range(0, freeTiles.Count);

        return freeTiles[random];
    }

    public int GetIndexOfTile(AzulTile tile)
    {
        for(int i = 0; i < tileDatabase.Length; i++)
        {
            if (tile == tileDatabase[i])
                return i;
        }

        return -1;
    }
}
