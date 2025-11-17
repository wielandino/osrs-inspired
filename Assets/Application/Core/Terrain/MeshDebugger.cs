using UnityEngine;

public class MeshDebugger : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] 
    private GameObject tileToDebug;

    [SerializeField] 
    private bool analyzeNow;
    
    private void OnValidate()
    {
        if (analyzeNow)
        {
            analyzeNow = false;
            if (tileToDebug != null)
                AnalyzeMesh();
        }
    }
    
    private void AnalyzeMesh()
    {
        MeshFilter meshFilter = tileToDebug.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("No MeshFilter!");
            return;
        }
        
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        
        for (int i = 0; i < vertices.Length; i++)
            Debug.Log($"Vertex {i}: {vertices[i]}");
        
        Vector3 min = vertices[0];
        Vector3 max = vertices[0];
        
        foreach (Vector3 v in vertices)
        {
            min = Vector3.Min(min, v);
            max = Vector3.Max(max, v);
        }
        
        Debug.Log($"Min Bounds: {min}");
        Debug.Log($"Max Bounds: {max}");
    }
}