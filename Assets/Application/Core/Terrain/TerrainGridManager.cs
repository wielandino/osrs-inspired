using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TerrainGridManager : MonoBehaviour
{
    public static TerrainGridManager Instance;
    
    [Header("Persistent Data")]
    [SerializeField] private SerializableTerrainData terrainData;

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
    
    private void Start()
    {
        if (Application.isPlaying && terrainData != null && terrainData.heightPoints.Count > 0)
        {
            LoadTerrainData();
            Debug.Log("Auto-loaded terrain data in Play mode");
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Mehrere TerrainGridManager in der Szene!");
                return;
            }

            Destroy(gameObject);
            return;
        }
        
        if (terrainGrid == null || terrainGrid.Count == 0)
            InitializeGrid();
        
    }
    
    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;
        
        
        if (terrainGrid == null || terrainGrid.Count == 0)
            InitializeGrid();
    }

    private void InitializeGrid()
    {
        if (tilesParent == null)
        {
            GameObject parent = new("Terrain_Tiles");
            parent.transform.SetParent(transform);
            tilesParent = parent.transform;
        }
        
        heightGrid ??= new HeightGrid();
        
        if (terrainData != null && terrainData.heightPoints != null && terrainData.heightPoints.Count > 0)
            LoadTerrainData();
        
        
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                Vector2Int gridPos = new(x, z);
                TerrainCell cell = new(gridPos);
                
                heightGrid.UpdateCellHeights(cell);               
                terrainGrid.Add(gridPos, cell);
            }
        }
        
        Debug.Log($"Terrain Grid initialized: {gridSize.x}x{gridSize.y} = {terrainGrid.Count} cells");
    }

    public HeightGrid GetHeightGrid()
        => heightGrid;
    
    
    public TerrainCell GetCell(Vector2Int gridPosition)
    {
        if (terrainGrid.ContainsKey(gridPosition))
            return terrainGrid[gridPosition];
        
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
        
        
        GameObject tilePrefab = tileDatabase.GetRandomTile(cell.tileType);
        if (tilePrefab == null)
            return;
        
        Vector3 worldPos = GridToWorld(gridPosition, 0f);
        
        GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.identity, tilesParent);
        tile.name = $"Tile_{gridPosition.x}_{gridPosition.y}";
        
        cell.placedTile = tile;

        TerrainMeshDeformer.DeformTileMesh(tile, cell, tileSize);

        #if UNITY_EDITOR
        MarkDirty();
        #endif
    }
    
    public void GenerateAllTiles()
    {
        int tilesPlaced = 0;
        
        foreach (var kvp in terrainGrid)
        {
            PlaceTile(kvp.Key);
            tilesPlaced++;
        }

        UpdatePathfindingGraph();
    }
    
    public void ClearAllTiles()
    {
        foreach (var kvp in terrainGrid)
        {
            if (kvp.Value.placedTile != null)
            {
                DestroyImmediate(kvp.Value.placedTile);
                kvp.Value.placedTile = null;
            }
        }

        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(Instance);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );
        }
    }

    public float GetTerrainHeightAtWorldPosition(Vector3 worldPosition)
    {
        if (heightGrid == null)
        {
            Debug.LogWarning("HeightGrid not initialized!");
            return 0f;
        }
        
        Vector2Int gridPos = WorldToGrid(worldPosition);
        
        TerrainCell cell = GetCell(gridPos);
        if (cell == null)
            return 0f;
        
        
        return GetInterpolatedHeightInCell(cell, worldPosition);
    }

    private float GetInterpolatedHeightInCell(TerrainCell cell, Vector3 worldPosition)
    {
        Vector3 cellWorldPos = GridToWorld(cell.gridPosition, 0f);
        
        float localX = (worldPosition.x - cellWorldPos.x) / tileSize; 
        float localZ = (worldPosition.z - cellWorldPos.z) / tileSize;
        
        localX = Mathf.Clamp01(localX);
        localZ = Mathf.Clamp01(localZ);
        
        float heightSouth = Mathf.Lerp(cell.heightSouthWest, cell.heightSouthEast, localX);
        float heightNorth = Mathf.Lerp(cell.heightNorthWest, cell.heightNorthEast, localX);
        
        float finalHeight = Mathf.Lerp(heightSouth, heightNorth, localZ);
        
        return finalHeight;
    }

    public bool IsPositionInGrid(Vector3 worldPosition)
    {
        Vector2Int gridPos = WorldToGrid(worldPosition);
        return GetCell(gridPos) != null;
    }

    public void SaveTerrainData()
    {
        if (heightGrid == null)
        {
            Debug.LogWarning("HeightGrid is null, cannot save!");
            return;
        }
        
        terrainData = heightGrid.ExportData();
        terrainData.gridSize = gridSize;
        
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
        );
        #endif
        
        Debug.Log($"Terrain data saved! {terrainData.heightPoints.Count} height points");
    }

    public void LoadTerrainData()
    {
        if (terrainData == null)
        {
            Debug.Log("terrainData is null");
            return;
        }
        
        if (terrainData.heightPoints == null)
        {
            Debug.Log("terrainData.heightPoints is null");
            return;
        }
        
        if (terrainData.heightPoints.Count == 0)
        {
            Debug.Log("No Terrain-Daten to load");
            return;
        }
        
        if (heightGrid == null)
        {
            heightGrid = new HeightGrid();
            Debug.Log("HeightGrid recreated");
        }
        
        heightGrid.ImportData(terrainData);
        
        Debug.Log($"Terrain data loaded! {terrainData.heightPoints.Count} height points");
    }

    public void UpdatePathfindingGraph()
    {
        if (AstarPath.active != null)
        {
            AstarPath.active.Scan();
            Debug.Log("A* Graph updated!");
        }
    }

    public void FlattenTerrain()
    {
        heightGrid ??= new HeightGrid();
        terrainData = new SerializableTerrainData(gridSize);
        
        for (int x = 0; x <= gridSize.x; x++)
            for (int z = 0; z <= gridSize.y; z++)
                heightGrid.SetHeight(new Vector2Int(x, z), 0f);
            
    
        foreach (var kvp in terrainGrid)
            heightGrid.UpdateCellHeights(kvp.Value);
        
        
        #if UNITY_EDITOR
        MarkDirty();
        #endif
        
        Debug.Log("Terrain flattened!");
    }

    public void ResetTerrain()
    {
        ClearAllTiles();
        FlattenTerrain();
        
        Debug.Log("Terrain reset - all tiles cleared and heights set to 0!");
    }

    #if UNITY_EDITOR
    [ContextMenu("Initialize Grid")]
    public void EditorInitializeGrid()
    {
        InitializeGrid();
        UnityEditor.EditorUtility.SetDirty(this);
    }

    private void MarkDirty()
    {
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );
        }
    }
    #endif
}