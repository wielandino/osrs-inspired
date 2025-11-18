using System.Collections.Generic;
using UnityEngine;

public class HeightGrid
{
    private readonly Dictionary<Vector2Int, float> heightPoints = new();
    
    public void SetHeight(Vector2Int point, float height)
    {
        if (heightPoints.ContainsKey(point))
            heightPoints[point] = height;
        else
            heightPoints.Add(point, height);
        
    }
    
    public float GetHeight(Vector2Int point)
    {
        if (heightPoints.ContainsKey(point))
        {
            return heightPoints[point];
        }

        return 0f;
    }

    public void UpdateCellHeights(TerrainCell cell)
    {
        Vector2Int gridPos = cell.gridPosition;
        
        cell.heightNorthWest = GetHeight(new Vector2Int(gridPos.x, gridPos.y + 1));
        cell.heightNorthEast = GetHeight(new Vector2Int(gridPos.x + 1, gridPos.y + 1));
        cell.heightSouthWest = GetHeight(new Vector2Int(gridPos.x, gridPos.y));
        cell.heightSouthEast = GetHeight(new Vector2Int(gridPos.x + 1, gridPos.y));
    }
    
    public void RaiseArea(Vector2Int center, float radius, float amount)
    {
        int radiusInt = Mathf.CeilToInt(radius);
        
        for (int x = -radiusInt; x <= radiusInt; x++)
        {
            for (int y = -radiusInt; y <= radiusInt; y++)
            {
                Vector2Int point = center + new Vector2Int(x, y);
                float distance = Vector2.Distance(center, point);
                
                if (distance <= radius)
                {
                    float falloff = 1f - (distance / radius);
                    float currentHeight = GetHeight(point);
                    SetHeight(point, currentHeight + (amount * falloff));
                }
            }
        }
    }

    public SerializableTerrainData ExportData()
    {
        SerializableTerrainData data = new SerializableTerrainData(Vector2Int.zero);
        
        foreach (var kvp in heightPoints)
        {
            data.heightPoints.Add(new HeightPoint(kvp.Key, kvp.Value));
        }
        
        return data;
    }

    public void ImportData(SerializableTerrainData data)
    {
        heightPoints.Clear();
        
        foreach (var point in data.heightPoints)
        {
            SetHeight(point.position, point.height);
        }
        
        Debug.Log($"Imported {data.heightPoints.Count} height points");
    }

    public int GetPointCount()
    {
        return heightPoints.Count;
    }
}