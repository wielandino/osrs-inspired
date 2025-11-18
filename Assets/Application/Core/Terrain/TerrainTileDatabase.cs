using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainTileDatabase", menuName = "Terrain/Tile Database")]
public class TerrainTileDatabase : ScriptableObject
{
    [Header("Grass Tiles")]
    public List<TerrainTileData> grassTiles = new();
    
    [Header("Stone Tiles")]
    public List<TerrainTileData> stoneTiles = new();

    public GameObject GetRandomTile(TileType type)
    {
        List<TerrainTileData> tiles = GetTileList(type);
        
        if (tiles == null || tiles.Count == 0)
        {
            Debug.LogWarning($"No Tiles of {type} in database!");
            return null;
        }
        
        // Zufälliges Tile auswählen
        int randomIndex = Random.Range(0, tiles.Count);
        return tiles[randomIndex].tilePrefab;
    }
    
    private List<TerrainTileData> GetTileList(TileType type)
    {
        return type switch
        {
            TileType.Grass => grassTiles,
            TileType.Stone => stoneTiles,
            _ => grassTiles,
        };
    }
}