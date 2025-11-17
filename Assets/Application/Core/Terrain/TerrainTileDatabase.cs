using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainTileDatabase", menuName = "Terrain/Tile Database")]
public class TerrainTileDatabase : ScriptableObject
{
    [Header("Grass Tiles")]
    public List<TerrainTileData> grassTiles = new();
    
    public GameObject GetRandomGrassTile()
    {
        if (grassTiles.Count == 0) return null;
        
        int randomIndex = Random.Range(0, grassTiles.Count);
        return grassTiles[randomIndex].tilePrefab;
    }
}