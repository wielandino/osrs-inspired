using UnityEngine;

public static class TerrainMeshDeformer
{
    public static void DeformTileMesh(GameObject tile, TerrainCell cell, float tileSize = 1f)
    {
        MeshFilter meshFilter = tile.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogWarning($"Tile {tile.name} hat keinen MeshFilter!");
            return;
        }
        
        Mesh mesh = Object.Instantiate(meshFilter.sharedMesh);
        meshFilter.mesh = mesh;
        
        Vector3[] vertices = mesh.vertices;
        
        float topY = 0.16f;
        float tolerance = 0.01f;
        
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = vertices[i];
            
            if (Mathf.Abs(v.y - topY) < tolerance)
            {
                float newHeight = 0f;
                
                bool isWest = v.x < 0;   // x < 0
                bool isEast = v.x > 0;   // x > 0
                bool isNorth = v.z > 0;  // z > 0
                bool isSouth = v.z < 0;  // z < 0
                
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
                
                vertices[i] = new Vector3(v.x, newHeight, v.z);
            }
        }
        
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
    }
}