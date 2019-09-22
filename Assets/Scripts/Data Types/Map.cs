using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public MapTile[,] tiles;
    public int[,] occupants;
    public int[,] occupantTickers;
    public int occupantTicker = 0;
    public int maxCost = 100;
    public int width;
    public int height;

    public Map(int myWidth, int myHeight, string mapString)
    {
        width = myWidth;
        height = myHeight;

        tiles = new MapTile[myWidth, myHeight];
        occupants = new int[myWidth, myHeight];
        occupantTickers = new int[myWidth, myHeight];
    }

    public int GetOccupants(Vector2Int tile)
    {
        return occupants[tile.x, tile.y];
    }

    public void IncrementOccupants(Vector2Int tile)
    {
        occupants[tile.x, tile.y] += 1;
    }

    public void ResetOccupants(Vector2Int tile)
    {
        occupants[tile.x, tile.y] = 1;
    }

    public bool IsCurrentOccupantTicker(Vector2Int tile)
    {
        return occupantTickers[tile.x, tile.y] == occupantTicker;
    }

    public void UpdateOccupantTicker(Vector2Int tile)
    {
        if (!IsCurrentOccupantTicker(tile))
        {
            occupantTickers[tile.x, tile.y] = occupantTicker;
            ResetOccupants(tile);
        }
        else
        {
            IncrementOccupants(tile);
        }
    }


    public bool IsInsideMap(Vector2Int tile)
    {
        if (tile.x < 0 || tile.x >= width)
        {
            return false;
        }
        if (tile.y < 0 || tile.y >= height)
        {
            return false;
        }

        return true;
    }
    public bool IsWall(Vector2Int tile)
    {
        if (tile.x < 0 || tile.x >= width || tile.y < 0 || tile.y >= height)
        {
            return true;
        }

        if (tiles[tile.x, tile.y].moveCost == maxCost)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsResourceSpawner(Vector2Int tile)
    {
        return tiles[tile.x, tile.y].isResourceSpawner;
    }

    public MapTile GetMapTile(Vector2Int tile)
    {
        return tiles[tile.x, tile.y];
    }

    public void SetMapTile(Vector2Int tile, MapTile toSet)
    {
        tiles[tile.x, tile.y] = toSet;
    }
}
