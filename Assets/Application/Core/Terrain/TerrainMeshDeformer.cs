using UnityEngine;

public static class TerrainMeshDeformer
{
    /// <summary>
    /// Verformt das Mesh eines Tiles basierend auf den Höhenwerten der Cell
    /// </summary>
    public static void DeformTileMesh(GameObject tile, TerrainCell cell, float tileSize = 1f)
    {
        MeshFilter meshFilter = tile.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogWarning($"Tile {tile.name} hat keinen MeshFilter!");
            return;
        }
        
        // Erstelle eine Kopie des Meshes (wichtig!)
        Mesh mesh = Object.Instantiate(meshFilter.sharedMesh);
        meshFilter.mesh = mesh;
        
        Vector3[] vertices = mesh.vertices;
        
        // Die Oberseite des Tiles liegt bei Y = 0.16
        float topY = 0.16f;
        float tolerance = 0.01f;
        
        // Durchlaufe alle Vertices und passe die oberen an
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = vertices[i];
            
            // Ist das ein Vertex der Oberseite?
            if (Mathf.Abs(v.y - topY) < tolerance)
            {
                float newHeight = 0f;
                
                // Bestimme die Ecke basierend auf X und Z Position
                bool isWest = v.x < 0;
                bool isEast = v.x > 0;
                bool isNorth = v.z > 0;
                bool isSouth = v.z < 0;
                
                // Weise die passende Höhe zu
                if (isWest && isNorth)
                {
                    newHeight = cell.heightNorthWest;
                }
                else if (isEast && isNorth)
                {
                    newHeight = cell.heightNorthEast;
                }
                else if (isWest && isSouth)
                {
                    newHeight = cell.heightSouthWest;
                }
                else if (isEast && isSouth)
                {
                    newHeight = cell.heightSouthEast;
                }
                
                // Setze die neue Y-Position
                vertices[i] = new Vector3(v.x, newHeight, v.z);
            }
        }
        
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        // NEU: Aktualisiere/Erstelle Mesh Collider
        UpdateMeshCollider(tile, mesh);
    }

    // <summary>
    /// Aktualisiert den Collider auf das verformte Mesh
    /// </summary>
    private static void UpdateMeshCollider(GameObject tile, Mesh mesh)
    {
        // Entferne alten Box Collider falls vorhanden
        BoxCollider boxCollider = tile.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
                Object.DestroyImmediate(boxCollider);
            else
                Object.Destroy(boxCollider);
            #else
            Object.Destroy(boxCollider);
            #endif
        }
        
        // Hole oder erstelle Mesh Collider
        MeshCollider meshCollider = tile.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = tile.AddComponent<MeshCollider>();
        }
        
        // Setze das verformte Mesh als Collider
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = false; // Für Terrain immer false
        
        // Debug-Log entfernen (zu viele Logs)
        // Debug.Log($"Mesh Collider updated for {tile.name}");
    }
}