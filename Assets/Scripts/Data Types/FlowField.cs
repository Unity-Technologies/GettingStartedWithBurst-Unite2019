using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    Map map;
    public List<Vector2Int> targets;
    public float[] stepField;
    public Vector2[] flowField;

    public PathType pathType;

    Vector2Int[] moveDirs = new Vector2Int[] {new Vector2Int(0,1),
                                          new Vector2Int(1,1),
                                          new Vector2Int(1,0),
                                          new Vector2Int(1,-1),
                                          new Vector2Int(0,-1),
                                          new Vector2Int(-1,-1),
                                          new Vector2Int(-1,0),
                                         new Vector2Int(-1,1)};

    List<Vector2Int> openSet;
    List<Vector2Int> nextSet;

    public FlowField(Map myMap, Vector2Int targetTile, PathType type = PathType.Default)
    {
        map = myMap;
        targets = new List<Vector2Int>();
        targets.Add(targetTile);
        pathType = type;
        Generate();
    }
    public FlowField(Map myMap, List<Vector2Int> targetTiles, PathType type = PathType.Default)
    {
        map = myMap;
        targets = targetTiles;
        pathType = type;
        Generate();
    }

    public Vector2 Get(Vector2Int tile)
    {
        return flowField[tile.x + tile.y * map.width];
    }

    public void Generate()
    {
        int i, j, k;

        if (stepField == null)
        {
            stepField = new float[map.width * map.height];
            flowField = new Vector2[map.width * map.height];
        }

        if (openSet == null)
        {
            openSet = new List<Vector2Int>();
            nextSet = new List<Vector2Int>();
        }
        else
        {
            openSet.Clear();
            nextSet.Clear();
        }
        for (i = 0; i < targets.Count; i++)
        {
            openSet.Add(targets[i]);
        }
        for (i = 0; i < map.width; i++)
        {
            for (j = 0; j < map.height; j++)
            {
                stepField[i + j * map.width] = int.MaxValue;
                flowField[i + j * map.width] = Vector2.zero;
            }
        }

        for (i = 0; i < targets.Count; i++)
        {
            stepField[targets[i].x + targets[i].y * map.width] = 0;
        }

        UnityEngine.Profiling.Profiler.BeginSample("FlowGenerate");
        while (openSet.Count > 0)
        {
            for (j = 0; j < openSet.Count; j++)
            {
                Vector2Int tile = openSet[j];
                float existingCost = stepField[tile.x + tile.y * map.width];
                for (i = 0; i < moveDirs.Length; i++)
                {
                    Vector2Int newTile = new Vector2Int(tile.x + moveDirs[i].x, tile.y + moveDirs[i].y);
                    if (map.IsInsideMap(newTile))
                    {
                        if (map.IsWall(newTile) == false)
                        {
                            Vector2Int checkTileA = new Vector2Int(tile.x + moveDirs[i].x, tile.y);
                            Vector2Int checkTileB = new Vector2Int(tile.x, tile.y + moveDirs[i].y);
                            if (map.IsWall(checkTileA) == false)
                            {
                                if (map.IsWall(checkTileB) == false)
                                {
                                    float moveCost = map.GetMapTile(newTile).moveCost;
                                    if (map.IsCurrentOccupantTicker(newTile))
                                    {
                                        moveCost += map.GetOccupants(newTile) * .5f;
                                    }
                                    if (pathType != PathType.Default)
                                    {
                                        if (pathType == map.GetMapTile(newTile).pathType)
                                        {
                                            moveCost *= .3f;
                                        }
                                    }
                                    float cost = moveCost * moveDirs[i].magnitude + existingCost;
                                    if (cost < stepField[newTile.x + newTile.y * map.width])
                                    {
                                        stepField[newTile.x + newTile.y * map.width] = cost;
                                        nextSet.Add(newTile);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            List<Vector2Int> temp = openSet;
            openSet = nextSet;
            nextSet = temp;
            nextSet.Clear();
        }

        for (i = 0; i < map.width; i++)
        {
            for (j = 0; j < map.height; j++)
            {
                Vector2Int tile = new Vector2Int(i, j);
                Vector2 flow = Vector2.zero;
                if (map.IsWall(tile) == false)
                {
                    float myCost = stepField[i + j * map.width];
                    float minNeighborCost = myCost;
                    Vector2Int bestNeighbor = new Vector2Int(i, j);
                    for (k = 0; k < moveDirs.Length; k++)
                    {
                        Vector2Int newTile = new Vector2Int(tile.x + moveDirs[k].x, tile.y + moveDirs[k].y);
                        if (map.IsInsideMap(newTile))
                        {
                            Vector2Int checkTileA = new Vector2Int(tile.x + moveDirs[k].x, tile.y);
                            Vector2Int checkTileB = new Vector2Int(tile.x, tile.y + moveDirs[k].y);
                            if (map.IsWall(checkTileA) == false)
                            {
                                if (map.IsWall(checkTileB) == false)
                                {
                                    if (stepField[newTile.x + newTile.y * map.width] < minNeighborCost)
                                    {
                                        minNeighborCost = stepField[newTile.x + newTile.y * map.width];
                                        bestNeighbor = newTile;
                                    }
                                }
                            }
                        }
                    }

                    Vector2Int intFlow = new Vector2Int(bestNeighbor.x - tile.x, bestNeighbor.y - tile.y);
                    flow = new Vector2(intFlow.x, intFlow.y);
                    if (flow.sqrMagnitude > 0f)
                    {
                        flow = flow.normalized;
                    }
                }
                flowField[i + j * map.width] = flow;
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }
}
