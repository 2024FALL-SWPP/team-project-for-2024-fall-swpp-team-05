using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SerializableVector2Int
{
    public int x;
    public int y;

    public SerializableVector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public SerializableVector2Int(Vector2Int vector)
    {
        x = vector.x;
        y = vector.y;
    }

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int(x, y);
    }
}

[System.Serializable]
public class TerrainData
{
    public string stage;
    public string terrainIndex;
    public SerializableVector2Int gridSize;
    public Dictionary<string, List<Vector3>> objectPositions = new Dictionary<string, List<Vector3>>();
    public List<Tuple<int, int>> pipeConnections = new List<Tuple<int, int>>();
}
