using UnityEngine;

public class TerrainTester : MonoBehaviour
{
    [ContextMenu("Generate All Tiles")]
    private void GenerateAllTiles()
    {
        if (TerrainGridManager.Instance != null)
            TerrainGridManager.Instance.GenerateAllTiles();
        else
            Debug.LogError("TerrainGridManager Instance nicht gefunden!");
        
    }
    
    [ContextMenu("Clear All Tiles")]
    private void ClearAllTiles()
    {
        if (TerrainGridManager.Instance != null)      
            TerrainGridManager.Instance.ClearAllTiles();
        else  
            Debug.LogError("TerrainGridManager Instance nicht gefunden!");
        
    }
    
    [ContextMenu("Create Test Hill")]
    private void CreateTestHill()
    {
        TerrainGridManager manager = TerrainGridManager.Instance;
        if (manager == null)
        {
            Debug.LogError("TerrainGridManager not found!");
            return;
        }
        
        HeightGrid heightGrid = manager.GetHeightGrid();
        
        Vector2Int center = new(8, 8);
        heightGrid.RaiseArea(center, 3f, 3f);
        
        manager.ClearAllTiles();
        
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                Vector2Int gridPos = new(x, z);
                TerrainCell cell = manager.GetCell(gridPos);

                if (cell != null)
                    heightGrid.UpdateCellHeights(cell);
            }
        }
        
        manager.GenerateAllTiles();
    }

    [ContextMenu("Debug: Show Height Values")]
    private void DebugShowHeightValues()
    {
        TerrainGridManager manager = TerrainGridManager.Instance;
        if (manager == null) return;
        
        Vector2Int center = new(8, 8);
        
        for (int x = -2; x <= 2; x++)
        {
            for (int z = -2; z <= 2; z++)
            {
                Vector2Int pos = center + new Vector2Int(x, z);
                TerrainCell cell = manager.GetCell(pos);
                
                if (cell != null)
                    Debug.Log($"Cell ({pos.x},{pos.y}): NW={cell.heightNorthWest}, NE={cell.heightNorthEast}, SW={cell.heightSouthWest}, SE={cell.heightSouthEast}");
            }
        }
    }
}