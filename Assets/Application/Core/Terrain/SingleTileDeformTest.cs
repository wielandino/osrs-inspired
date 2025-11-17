using UnityEngine;

public class SingleTileDeformTest : MonoBehaviour
{
    [Header("Test Single Tile")]
    [SerializeField] 
    private GameObject testTilePrefab;

    private GameObject spawnedTile;
    
    [ContextMenu("Spawn Flat Tile")]
    private void SpawnFlatTile()
    {
        if (spawnedTile != null) Destroy(spawnedTile);
        
        spawnedTile = Instantiate(testTilePrefab, Vector3.zero, Quaternion.identity);
        spawnedTile.name = "TestTile_Flat";
    }
    
    [ContextMenu("Deform Tile - North High")]
    private void DeformTileNorthHigh()
    {
        if (spawnedTile == null)
        {
            Debug.LogError("Spawne zuerst ein Tile!");
            return;
        }

        TerrainCell testCell = new(Vector2Int.zero)
        {
            heightNorthWest = 2f,
            heightNorthEast = 2f,
            heightSouthWest = 0f,
            heightSouthEast = 0f
        };

        TerrainMeshDeformer.DeformTileMesh(spawnedTile, testCell, 1f);
    }
    
    [ContextMenu("Deform Tile - Pyramid")]
    private void DeformTilePyramid()
    {
        if (spawnedTile == null)
        {
            Debug.LogError("Spawne zuerst ein Tile!");
            return;
        }

        TerrainCell testCell = new(Vector2Int.zero)
        {
            heightNorthWest = 0f,
            heightNorthEast = 0f,
            heightSouthWest = 0f,
            heightSouthEast = 3f
        };

        TerrainMeshDeformer.DeformTileMesh(spawnedTile, testCell, 1f);
    }
}