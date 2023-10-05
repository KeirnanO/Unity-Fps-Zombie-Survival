using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardTileQueue : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Color regularColor;
    public Color highlightColor;

    public int index;
    public AzulBoard board;
    public AzulTile[] queuedTiles;

    private void Start()
    {
        board = GetComponentInParent<AzulBoard>();
    }

    public void AddTilesToQueue(List<AzulTile> tileList)
    {
        while(tileList.Count > 0)
        {
            //If we don't add a tile quit early
            if (!TryAddTileToQueue(tileList[0]))
                break;

            //If we did add a tile remove from the listToAdd
            tileList.RemoveAt(0);
        }

        //Any tiles leftover go to the negative zone
        if(tileList.Count > 0)
        {
            //send tiles to negative zone
        }
    }

    bool TryAddTileToQueue(AzulTile tile)
    {
        //If the first tile is not set we can set this queue to this tile
        if (queuedTiles[0] == null)
        {
            queuedTiles[0] = tile;
            tile.transform.position = board.queuedSpriteObjects[index][0].transform.position + (Vector3.up * 0.3f);
            return true;
        }

        //If the first tile is not the same tile as the list return out;
        if (queuedTiles[0].TileType != tile.TileType)
            return false;
        
        //Look for an empty spot in the queue
        for(int i = 0; i < queuedTiles.Length; i++)
        {
            if (queuedTiles[i] != null)
                continue;

            queuedTiles[i] = tile;
            tile.transform.position = board.queuedSpriteObjects[index][i].transform.position + (Vector3.up * 0.3f);
            return true;
        }

        //queue is full
        return false;
    }

    private void OnMouseDown()
    {
        foreach (var player in FindObjectsOfType<BoardGamePlayer>())
        {
            player.SelectQueue(index);
        }
    }

    private void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().color = highlightColor;
    }

    private void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().color = regularColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        board.ChooseQueue(index);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<SpriteRenderer>().color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<SpriteRenderer>().color = regularColor;
    }
}
