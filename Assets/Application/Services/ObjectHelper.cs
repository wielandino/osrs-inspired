using System.Collections.Generic;
using UnityEngine;

public static class ObjectHelper
{

    public static float GetGroundYPosition(Vector3 spawnPosition)
    {
        Ray ray = new(spawnPosition + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 20f))
        {
            return hit.point.y;
        }
        return spawnPosition.y;
    }

    public static List<Vector3> CollectInteractionTilesOfPosition(Vector3 position)
    {
        var tileSize = GridManager.Instance.UnityGridSize;
        var interactionTiles = new List<Vector3>();

        position.y = Player.Instance.transform.position.y;

        Vector3[] potentialPositions = new Vector3[]
        {
            position + Vector3.right * tileSize, 
            position + Vector3.left * tileSize,
            position + Vector3.forward * tileSize,
            position + Vector3.back * tileSize
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
        
        Vector3[] adjacentPositions = new Vector3[]
        {
            placedPosition + Vector3.right * tileSize,
            placedPosition + Vector3.left * tileSize,
            placedPosition + Vector3.forward * tileSize,
            placedPosition + Vector3.back * tileSize
        };
        
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

    public static void SetChildModelLocalYPosition(GameObject childModel)
    {
        if (childModel == null) return;

        if (childModel.TryGetComponent<Renderer>(out var renderer))
        {
            float objectHeight = renderer.bounds.size.y;
            float objectHalfHeight = objectHeight / 2f;

            Vector3 localPos = childModel.transform.localPosition;
            localPos.y = objectHalfHeight;
            childModel.transform.localPosition = localPos;

        }
    }
    
    public static bool HasObstacleAtPosition(Vector3 position, out Collider collider)
    {
        collider = null;

        Vector3 rayStart = new(position.x, 50f, position.z);
        Ray ray = new(rayStart, Vector3.down);
        
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f, LayerMask.GetMask("Obstacle"));
        
        if (hits.Length == 0)
            return false;
        
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider)
            {
                collider = hit.collider;
                return true;
            }
        }
        
        return true;
    }
}