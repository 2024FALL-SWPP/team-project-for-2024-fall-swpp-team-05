using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

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

[Serializable]
public class ObjectPosition
{
    public string name;
    public List<Vector3> positions = new List<Vector3>();
}

[Serializable]
public class ObjPosList
{
    public List<ObjectPosition> objPos = new List<ObjectPosition>();
}

[System.Serializable]
public class TerrainData
{
    public string stage;
    public string terrainIndex;
    public SerializableVector2Int gridSize;
    public ObjPosList objectPositions = new ObjPosList();
    public List<Tuple<int, int>> pipeConnections = new List<Tuple<int, int>>();
}
