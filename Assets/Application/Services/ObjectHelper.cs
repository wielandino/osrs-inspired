using System.Collections.Generic;
using UnityEngine;

public static class ObjectHelper
{

    public static List<Vector3> CollectInteractionTilesOfPosition(Vector3 position)
    {
        var tileSize = GridManager.Instance.UnityGridSize;
        var interactionTiles = new List<Vector3>();

        position.y = Player.Instance.transform.position.y;

        Vector3[] potentialPositions = new Vector3[]
        {
            position + Vector3.right * tileSize,   // Rechts (X+)
            position + Vector3.left * tileSize,    // Links (X-)
            position + Vector3.forward * tileSize, // Vorne (Z+)
            position + Vector3.back * tileSize     // Hinten (Z-)
        };

        for (int i = 0; i < potentialPositions.Length; i++)
        {
            Vector3 pos = potentialPositions[i];

            if (IsTileFree(pos))
                interactionTiles.Add(pos);
        }

        return interactionTiles;
    }

    public static bool IsTileFree(Vector3 position)
    {
        var tileSize = GridManager.Instance.UnityGridSize;
        var obstacleLayerMask = LayerMask.GetMask("Obstacle");

        Vector3 halfExtents = new(
            tileSize / 2,
            0.6f,
            tileSize / 2
        );

        Vector3 checkPosition = new(position.x, 0.0f, position.z);
        Collider[] overlapping = Physics.OverlapBox(checkPosition, halfExtents, Quaternion.identity, obstacleLayerMask);

        return overlapping.Length == 0;
    }

    public static void UpdateAdjacentInteractionTiles(Vector3 placedPosition)
    {
        var tileSize = GridManager.Instance.UnityGridSize;
        var obstacleLayerMask = LayerMask.GetMask("Obstacle");
        
        // Die 4 angrenzenden Positionen
        Vector3[] adjacentPositions = new Vector3[]
        {
            placedPosition + Vector3.right * tileSize,
            placedPosition + Vector3.left * tileSize,
            placedPosition + Vector3.forward * tileSize,
            placedPosition + Vector3.back * tileSize
        };
        
        Debug.Log($"[UpdateAdjacentInteractionTiles] Checking adjacent positions for object at {placedPosition}");
        
        foreach (var adjacentPos in adjacentPositions)
        {
            Vector3 halfExtents = new(tileSize / 2, 0.6f, tileSize / 2);
            Vector3 checkPosition = new(adjacentPos.x, 0.0f, adjacentPos.z);
            
            Collider[] overlapping = Physics.OverlapBox(
                checkPosition, 
                halfExtents, 
                Quaternion.identity, 
                obstacleLayerMask
            );
            
            foreach (Collider col in overlapping)
            {
                // Prüfe ob es ein Objekt mit Interaktionstiles ist
                if (col.TryGetComponent<IHasInteractionTiles>(out var interactableObject))
                {
                    Debug.Log($"[UpdateAdjacentInteractionTiles] Found {col.gameObject.name} at {adjacentPos}, triggering recalculation");
                    interactableObject.RecalculateInteractionTiles();
                }
            }
        }
    }

    public static float CalculateCorrectYPosition(Vector3 spawnPosition, GameObject prefab)
    {
        Ray ray = new(spawnPosition + Vector3.up * 5f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 20f))
        {
            float tileTopY = hit.point.y;

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
}