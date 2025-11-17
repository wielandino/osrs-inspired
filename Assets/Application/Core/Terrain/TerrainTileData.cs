using UnityEngine;

[CreateAssetMenu(fileName = "New Terrain Tile", menuName = "Terrain/Tile Data")]
public class TerrainTileData : ScriptableObject
{
    [Header("Tile Information")]
    public string tileName;
    public GameObject tilePrefab;
    
    [Header("Tile Properties")]
    public float spawnWeight = 1f;
}