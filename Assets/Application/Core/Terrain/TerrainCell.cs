using UnityEngine;

[System.Serializable]
public class TerrainCell
{
    [Header("Grid Position")]
    public Vector2Int gridPosition;
    
    [Header("Height Values")]
    public float heightNorthWest; 
    public float heightNorthEast;
    public float heightSouthWest;
    public float heightSouthEast;
    
    [Header("Tile Reference")]
    public GameObject placedTile;

    [Header("Tile Type")]
    public TileType tileType = TileType.Grass;
    
    public TerrainCell(Vector2Int position)
    {
        gridPosition = position;

        heightNorthWest = 0f;
        heightNorthEast = 0f;
        heightSouthWest = 0f;
        heightSouthEast = 0f;
    }
    
    public bool IsFlat()
    {
        return Mathf.Approximately(heightNorthWest, heightNorthEast) &&
               Mathf.Approximately(heightNorthWest, heightSouthWest) &&
               Mathf.Approximately(heightNorthWest, heightSouthEast);
    }
    
    public float GetAverageHeight()
        => (heightNorthWest + heightNorthEast + heightSouthWest + heightSouthEast) / 4f;
    
}