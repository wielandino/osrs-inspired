using System;
using UnityEngine;

[Serializable]
public class HeightPoint
{
    public Vector2Int position;
    public float height;
    
    public HeightPoint(Vector2Int pos, float h)
    {
        position = pos;
        height = h;
    }
}