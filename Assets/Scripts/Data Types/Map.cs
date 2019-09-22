using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class Map
{
    public struct MapDataStore
    {
        public NativeArray<MapTile> tiles;
        public NativeArray<int> occupants;
        public NativeArray<int> occupantTickers;
        public int occupantTicker;
        public int maxCost;
        public int width;
        public int height;

        public int GetOccupants(Vector2Int tile)
        {
            return occupants[tile.x + tile.y * width];
        }

        public void IncrementOccupants(Vector2Int tile)
        {
            occupants[tile.x + tile.y * width] += 1;
        }

        public void ResetOccupants(Vector2Int tile)
        {
            occupants[tile.x + tile.y * width] = 1;
        }

        public bool IsCurrentOccupantTicker(Vector2Int tile)
        {
            return occupantTickers[tile.x + tile.y * width] == occupantTicker;
        }

        public void UpdateOccupantTicker(Vector2Int tile)
        {
            if (!IsCurrentOccupantTicker(tile))
            {
                occupantTickers[tile.x + tile.y * width] = occupantTicker;
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

            if (tiles[tile.x + tile.y * width].moveCost == maxCost)
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
            return tiles[tile.x + tile.y * width].isResourceSpawner;
        }

        public MapTile GetMapTile(Vector2Int tile)
        {
            return tiles[tile.x + tile.y * width];
        }

        public void SetMapTile(Vector2Int tile, MapTile toSet)
        {
            tiles[tile.x + tile.y * width] = toSet;
        }

    };

    public MapDataStore mapDataStore;

    public int width => mapDataStore.width;
    public int height => mapDataStore.height;
    public int maxCost => mapDataStore.maxCost;

    public Map(int myWidth, int myHeight, string mapString)
    {
        mapDataStore = new MapDataStore
        {
            width = myWidth,
            height = myHeight,

            occupantTicker = 0,
            maxCost = 100,
            tiles = new NativeArray<MapTile>(myWidth * myHeight, Allocator.Persistent),
            occupants = new NativeArray<int>(myWidth * myHeight, Allocator.Persistent),
            occupantTickers = new NativeArray<int>(myWidth * myHeight, Allocator.Persistent),
        };
    }

    public int GetOccupants(Vector2Int tile)
    {
        return mapDataStore.GetOccupants(tile);
    }

    public void IncrementOccupants(Vector2Int tile)
    {
        mapDataStore.IncrementOccupants(tile);
    }

    public bool IsCurrentOccupantTicker(Vector2Int tile)
    {
        return mapDataStore.IsCurrentOccupantTicker(tile);
    }

    public void UpdateOccupantTicker(Vector2Int tile)
    {
        mapDataStore.UpdateOccupantTicker(tile);
    }

    public bool IsInsideMap(Vector2Int tile)
    {
        return mapDataStore.IsInsideMap(tile);
    }

    public bool IsWall(Vector2Int tile)
    {
        return mapDataStore.IsWall(tile);
    }

    public bool IsResourceSpawner(Vector2Int tile)
    {
        return mapDataStore.IsResourceSpawner(tile);
    }

    public MapTile GetMapTile(Vector2Int tile)
    {
        return mapDataStore.GetMapTile(tile);
    }

    public void SetMapTile(Vector2Int tile, MapTile toSet)
    {
        mapDataStore.SetMapTile(tile, toSet);
    }

    public void IncrementOccupantTicker()
    {
        mapDataStore.occupantTicker++;
    }

    public void Destroy()
    {
        mapDataStore.tiles.Dispose();
        mapDataStore.occupants.Dispose();
        mapDataStore.occupantTickers.Dispose();
    }
}
