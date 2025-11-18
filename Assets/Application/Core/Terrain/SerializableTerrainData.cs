using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableTerrainData
{
    public List<HeightPoint> heightPoints = new();
    public Vector2Int gridSize;
    
    public SerializableTerrainData(Vector2Int size)
    {
        gridSize = size;
    }
}