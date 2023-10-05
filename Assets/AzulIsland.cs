using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AzulIsland : NetworkBehaviour
{
   public List<AzulTile> tiles;

    private void Start()
    {
        foreach(var tile in tiles)
        {
            tile.SetIsland(this);
        }
    }

    public List<AzulTile> GetTilesByType(TileType type)
    {
        List<AzulTile> resultTiles = new List<AzulTile>();

        foreach(var tile in tiles)
        {
            if (tile.TileType.Equals(type))
            {
                resultTiles.Add(tile);
            }

        }

        return resultTiles;

    }
}
