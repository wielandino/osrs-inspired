using System.Collections.Generic;
using UnityEngine;

public static class ObjectHelper
{
    public static float GetGroundYPosition(Vector3 spawnPosition)
    {
        float groundHeight = 0f;
        
        if (TerrainGridManager.Instance != null && 
            TerrainGridManager.Instance.IsPositionInGrid(spawnPosition))
        {
            groundHeight = TerrainGridManager.Instance.GetTerrainHeightAtWorldPosition(spawnPosition);

            groundHeight += 0.16f;
        }
        else
        {
            Ray ray = new(spawnPosition + Vector3.up * 5f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 20f))
            {
                groundHeight = hit.point.y;
            }
            else
            {
                groundHeight = spawnPosition.y;
            }
        }
        
        if (Player.Instance != null)
        {
            SimpleGroundSnap groundSnap = Player.Instance.GetComponent<SimpleGroundSnap>();
            if (groundSnap != null)
            {
                groundHeight += groundSnap.groundOffset;
            }
        }
        
        return groundHeight;
    }

    public static List<Vector3> CollectInteractionTilesOfPosition(Vector3 position)
    {
        var tileSize = GridManager.Instance.UnityGridSize;
        var interactionTiles = new List<Vector3>();

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
            
            pos.y = GetGroundYPosition(pos);

            if (IsTileFree(pos))
            {
                interactionTiles.Add(pos);
            }
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

        Vector3 checkPosition = new(position.x, position.y, position.z);
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
            // Hole korrekte Y-Position
            Vector3 correctedPos = adjacentPos;
            correctedPos.y = GetGroundYPosition(adjacentPos);
            
            Vector3 halfExtents = new(tileSize / 2, 0.6f, tileSize / 2);
            
            Collider[] overlapping = Physics.OverlapBox(
                correctedPos, 
                halfExtents, 
                Quaternion.identity, 
                obstacleLayerMask
            );
            
            foreach (Collider col in overlapping)
            {
                if (col.TryGetComponent<IHasInteractionTiles>(out var interactableObject))
                    interactableObject.RecalculateInteractionTiles();
            }
        }
    }

    public static float CalculateCorrectYPosition(Vector3 spawnPosition, GameObject prefab)
    {
        float groundY = GetGroundYPosition(spawnPosition);
        
        Renderer renderer = prefab.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            MeshFilter meshFilter = prefab.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                Mesh mesh = meshFilter.sharedMesh;
                Vector3 meshSize = mesh.bounds.size;
                Vector3 scale = prefab.transform.localScale;

                float objectHeight = meshSize.y * scale.y;
                float objectHalfHeight = objectHeight / 2f;

                return groundY + objectHalfHeight;
            }
        }

        return groundY;
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

    public static bool IsPlayerOnInteractionTile(List<Vector3> interactionTiles, Vector3 playerPosition)
    {
        foreach (Vector3 tile in interactionTiles)
        {
            if (Mathf.Approximately(tile.x, playerPosition.x) && 
                Mathf.Approximately(tile.z, playerPosition.z))
            {
                return true;
            }
        }
        return false;
    }
}