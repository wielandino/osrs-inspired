using System.Collections.Generic;
using UnityEngine;

public class TreeLogSpawnController : MonoBehaviour
{
    public static TreeLogSpawnController Instance;

    private Tree _currentTree;
    private GameObject _logPrefab;

    [SerializeField]
    public LayerMask ObstacleLayerMask;

    [SerializeField]
    private float _despawnTime = 5f;

    private int _tileSize;

    private void Start()
    {
        ObstacleLayerMask = LayerMask.GetMask("Obstacle");
        _tileSize = GridManager.Instance.UnityGridSize;
    }

    private void OnEnable()
    {
        Instance = this;
    }

    private void OnDisable()
    {
        if (Instance != null)
            Instance = null;
    }

    private void ResetController()
    {
        _currentTree = null;
        _logPrefab = null;
    }

    // public void SpawnLog(Tree tree, GameObject logPrefab)
    // {
    //     _currentTree = tree;
    //     _logPrefab = logPrefab;

    //     Vector3 spawnPosition = FindFreeSpawnPosition();

    //     if (spawnPosition != Vector3.zero)
    //     {
    //         // Calculate Y-Position before the Treelog gets spawned
    //         float correctYPosition = ObjectHelper.CalculateCorrectYPosition(spawnPosition, logPrefab);


    //         Vector3 finalPosition = new(spawnPosition.x, correctYPosition, spawnPosition.z);
    //         var log = Instantiate(_logPrefab, finalPosition, Quaternion.identity);

    //         Debug.Log($"[SpawnLog] Final Position: {log.transform.position}");

    //         RegisterForDespawn(log);
    //     }

    //     ResetController();
    // }
    
    public void SpawnLog(Tree tree, GameObject logPrefab)
    {
        _currentTree = tree;
        _logPrefab = logPrefab;
        Vector3 spawnPosition = FindFreeSpawnPosition();

        if (spawnPosition != Vector3.zero)
        {
            // 1. Parent spawnt auf dem Boden
            float groundY = ObjectHelper.GetGroundYPosition(spawnPosition);
            Vector3 finalPosition = new(spawnPosition.x, groundY, spawnPosition.z);

            var log = Instantiate(_logPrefab, finalPosition, Quaternion.identity);
            var treeLogComponent = log.GetComponent<TreeLog>();

            // 2. Setze LOKALE Y-Positionen für die Children
            ObjectHelper.SetChildModelLocalYPosition(treeLogComponent.TreeLogIdleStateObject.gameObject);
            ObjectHelper.SetChildModelLocalYPosition(treeLogComponent.TreeLogBurningStateObject.gameObject);

            RegisterForDespawn(log);
        }
        
        ResetController();
    }

    private float CalculateCorrectYPositionBeforeSpawn(Vector3 spawnPosition, GameObject prefab)
    {
        Ray ray = new Ray(spawnPosition + Vector3.up * 5f, Vector3.down);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 20f))
        {
            float tileTopY = hit.point.y;
            
            Debug.Log($"[Debug] Raycast Hit Point: {tileTopY}");
            Debug.Log($"[Debug] Hit Object: {hit.collider.gameObject.name}");
            
            // Hol die Höhe vom PREFAB
            Renderer renderer = prefab.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                Mesh mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
                Vector3 meshSize = mesh.bounds.size;
                Vector3 scale = prefab.transform.localScale;
                
                float objectHeight = meshSize.y * scale.y;
                float objectHalfHeight = objectHeight / 2f;
                
                float pivotY = tileTopY + objectHalfHeight;
                
                return pivotY;
            }
        }
        
        return spawnPosition.y;
    }


    private void RegisterForDespawn(GameObject log)
    {
        DespawnController.Instance.RegisterForDespawn(log, _despawnTime);
    }

    private Vector3 FindFreeSpawnPosition()
    {
        Vector3 treePos = new(_currentTree.transform.position.x, 0f, _currentTree.transform.position.z);


        Vector3[] potentialPositions = new Vector3[]
        {
            treePos + Vector3.right * _tileSize,
            treePos + Vector3.left * _tileSize,
            treePos + Vector3.forward * _tileSize,
            treePos + Vector3.back * _tileSize
        };

        for (int i = 0; i < potentialPositions.Length; i++)
        {
            Vector3 pos = potentialPositions[i];

            if (IsTileFree(pos))
                return pos;

        }

        return Vector3.zero;
    }




    private bool IsTileFree(Vector3 position)
    {
        Vector3 halfExtents = new(
            _tileSize * 0.4f,   // 40% der Tile-Breite
            _tileSize * 0.5f,   // Höhe für 3D-Objekte  
            _tileSize * 0.4f    // 40% der Tile-Tiefe
        );


        Collider[] overlapping = Physics.OverlapBox(position, halfExtents, Quaternion.identity, ObstacleLayerMask);

        if (overlapping.Length > 0)
        {
            for (int i = 0; i < overlapping.Length; i++)
            {
                Collider col = overlapping[i];
                //Debug.Log($"  [{i}] Name: {col.name}");
                //Debug.Log($"      GameObject: {col.gameObject.name}");
                //Debug.Log($"      Layer: {col.gameObject.layer} ({LayerMask.LayerToName(col.gameObject.layer)})");
                //Debug.Log($"      Position: {col.transform.position}");
                //Debug.Log($"      Bounds: {col.bounds}");

                // WICHTIG: Prüfe ob es der Baum selbst ist
                if (col.gameObject == _currentTree.gameObject || col.transform.IsChildOf(_currentTree.transform))
                    continue;

                return false;
            }

            return true;
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        if (_currentTree == null) return;

        Vector3 centerPos = _currentTree.transform.position;

        Gizmos.color = Color.yellow;
        Vector3 tileSize = new(_tileSize, 0.1f, _tileSize);

        Gizmos.DrawWireCube(centerPos + Vector3.right * _tileSize, tileSize);
        Gizmos.DrawWireCube(centerPos + Vector3.left * _tileSize, tileSize);
        Gizmos.DrawWireCube(centerPos + Vector3.forward * _tileSize, tileSize);
        Gizmos.DrawWireCube(centerPos + Vector3.back * _tileSize, tileSize);


        Gizmos.color = Color.red;
        Vector3 checkSize = new(
            _tileSize * 0.8f,   // 80% für Visualisierung
            _tileSize * 1f,     // Höhe
            _tileSize * 0.8f    // 80% der Tile-Tiefe
        );

        Gizmos.DrawWireCube(centerPos + Vector3.right * _tileSize, checkSize);
        Gizmos.DrawWireCube(centerPos + Vector3.left * _tileSize, checkSize);
        Gizmos.DrawWireCube(centerPos + Vector3.forward * _tileSize, checkSize);
        Gizmos.DrawWireCube(centerPos + Vector3.back * _tileSize, checkSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(centerPos, tileSize);

        Gizmos.color = Color.magenta;
        Vector3 realCheckSize = new(
            _tileSize * 0.8f,   // Entspricht den halfExtents * 2
            _tileSize * 1f,
            _tileSize * 0.8f
        );

        Gizmos.DrawWireCube(centerPos + Vector3.right * _tileSize, realCheckSize);
        Gizmos.DrawWireCube(centerPos + Vector3.left * _tileSize, realCheckSize);
        Gizmos.DrawWireCube(centerPos + Vector3.forward * _tileSize, realCheckSize);
        Gizmos.DrawWireCube(centerPos + Vector3.back * _tileSize, realCheckSize);
    }

    // Debug-Hilfsmethode
    [ContextMenu("Debug Current Tree Colliders")]
    public void DebugTreeColliders()
    {
        if (_currentTree == null)
        {
            Debug.LogError("Kein _currentTree gesetzt!");
            return;
        }

        Debug.Log("=== TREE COLLIDER DEBUG ===");
        Debug.Log($"Tree Name: {_currentTree.name}");
        Debug.Log($"Tree Position: {_currentTree.transform.position}");
        Debug.Log($"Tree Layer: {_currentTree.gameObject.layer} ({LayerMask.LayerToName(_currentTree.gameObject.layer)})");

        Collider[] treeColliders = _currentTree.GetComponentsInChildren<Collider>();
        Debug.Log($"Tree hat {treeColliders.Length} Collider:");

        for (int i = 0; i < treeColliders.Length; i++)
        {
            Collider col = treeColliders[i];
            Debug.Log($"  [{i}] {col.name} - Layer: {col.gameObject.layer} - Bounds: {col.bounds}");
        }

        Debug.Log($"Obstacle LayerMask: {ObstacleLayerMask}");
        Debug.Log($"Tree ist im ObstacleLayerMask: {((1 << _currentTree.gameObject.layer) & ObstacleLayerMask) != 0}");
    }
}