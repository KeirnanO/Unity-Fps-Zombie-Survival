using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AzulBoard : NetworkBehaviour
{
    public GameObject boardSlotPrefab;
    public Transform playableSpritesTransform;
    public Transform QueuedSpritesTransform;
    public GameObject[,] playableSpriteObjects = new GameObject[5, 5];
    public GameObject[][] queuedSpriteObjects = new GameObject[5][];

    public BoardTileQueue[] tileQueues;

    BoardGamePlayer player;


    private void Start()
    {
        SetUpBoard();
    }

    void SetUpBoard()
    {
        for(int x = 0; x < 5; x++)
        {
            for(int y = 0; y < 5; y++)
            {
                GameObject boardSlotObject = Instantiate(boardSlotPrefab);
                boardSlotObject.transform.parent = playableSpritesTransform;
                boardSlotObject.transform.localScale = Vector3.one / 1.2f;
                boardSlotObject.transform.SetLocalPositionAndRotation(new Vector3(-x, -y, 0), Quaternion.identity);
                playableSpriteObjects[x, y] = boardSlotObject;
            }
        }

        for(int y = 0; y < 5; y++)
        {
            queuedSpriteObjects[y] = new GameObject[y + 1];

            for(int x = 0; x < queuedSpriteObjects[y].Length; x++)
            {
                GameObject boardSlotObject = Instantiate(boardSlotPrefab);
                boardSlotObject.transform.parent = QueuedSpritesTransform;
                boardSlotObject.transform.localScale = Vector3.one / 1.2f;
                boardSlotObject.transform.SetLocalPositionAndRotation(new Vector3(-y, x, 0), Quaternion.identity);

                queuedSpriteObjects[y][x] = boardSlotObject;
            }
        }    
    }

    [ClientRpc]
    public void SetPlayer(NetworkIdentity identity)
    {
        player = identity.GetComponent<BoardGamePlayer>();
    }

    [Server]
    public void AddTilesToQueue(List<AzulTile> tiles, int queueIndex)
    {
        tileQueues[queueIndex].AddTilesToQueue(tiles);
    }

    public void ChooseQueue(int queueIndex)
    {
        player.SelectQueue(queueIndex);
    }
}
