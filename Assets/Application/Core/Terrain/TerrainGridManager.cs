using System.Collections.Generic;
using UnityEngine;

public class TerrainGridManager : MonoBehaviour
{
    public static TerrainGridManager Instance;
    
    [Header("Grid Settings")]
    [SerializeField] 
    private Vector2Int gridSize = new(50, 50);
    [SerializeField]
    private float tileSize = 2f;
    
    [Header("Database")]
    [SerializeField] 
    private TerrainTileDatabase tileDatabase;
    
    [Header("Runtime Data")]
    private readonly Dictionary<Vector2Int, TerrainCell> terrainGrid = new();
    
    [SerializeField] private Transform tilesParent;

    [Header("Height System")]
    private HeightGrid heightGrid;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        InitializeGrid();
    }
    
    private void InitializeGrid()
    {
        if (tilesParent == null)
        {
            GameObject parent = new GameObject("Terrain_Tiles");
            parent.transform.SetParent(transform);
            tilesParent = parent.transform;
        }
        
        heightGrid = new HeightGrid();

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                Vector2Int gridPos = new(x, z);
                TerrainCell cell = new(gridPos);
                terrainGrid.Add(gridPos, cell);
            }
        }
    }

    public HeightGrid GetHeightGrid()
        => heightGrid;
    
    
    public TerrainCell GetCell(Vector2Int gridPosition)
    {
        if (terrainGrid.ContainsKey(gridPosition))
        {
            return terrainGrid[gridPosition];
        }

        return null;
    }
    
    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / tileSize);
        int z = Mathf.FloorToInt(worldPosition.z / tileSize);
        return new Vector2Int(x, z);
    }
    
    public Vector3 GridToWorld(Vector2Int gridPosition, float height = 0f)
        => new(gridPosition.x * tileSize, height, gridPosition.y * tileSize);
    
    public void PlaceTile(Vector2Int gridPosition)
    {
        TerrainCell cell = GetCell(gridPosition);
        if (cell == null) return;

        if (cell.placedTile != null)
            DestroyImmediate(cell.placedTile);
        
        
        GameObject tilePrefab = tileDatabase.GetRandomGrassTile();
        if (tilePrefab == null)
            return;
        
        Vector3 worldPos = GridToWorld(gridPosition, 0f);
        
        GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.identity, tilesParent);
        tile.name = $"Tile_{gridPosition.x}_{gridPosition.y}";
        
        cell.placedTile = tile;

        TerrainMeshDeformer.DeformTileMesh(tile, cell, tileSize);
    }
    
    public void GenerateAllTiles()
    {
        int tilesPlaced = 0;
        
        foreach (var kvp in terrainGrid)
        {
            PlaceTile(kvp.Key);
            tilesPlaced++;
        }
    }
    
    public void ClearAllTiles()
    {
        foreach (var kvp in terrainGrid)
        {
            if (kvp.Value.placedTile != null)
            {
                Destroy(kvp.Value.placedTile);
                kvp.Value.placedTile = null;
            }
        }
    }
}