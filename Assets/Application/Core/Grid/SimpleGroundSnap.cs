using UnityEngine;

public class SimpleGroundSnap : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("How far down should the ray look?")]
    public float rayDistance = 10f;
    
    [Tooltip("Small distance above the ground (so players don't get stuck IN the ground)")]
    public float groundOffset = 0.1f;

    [Header("Mesh Rotation")]
    [Tooltip("The mesh that is to be rotated (e.g., PlayerMesh as a child)")]
    public Transform meshTransform;
    
    [Tooltip("How fast should the mesh rotate to the ground slope?")]
    public float rotationSpeed = 10f;

    private Vector3 _currentGroundNormal = Vector3.up;

    [Header("Layer Settings")]
    [Tooltip("Layers to be ignored (e.g., items)")]
    public LayerMask ignoreLayer;

    private void Update()
    {
        Ray ray = new(transform.position, Vector3.down);

        int layerMask = ~ignoreLayer;
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, layerMask))
        {
            Vector3 newPosition = transform.position;
            newPosition.y = hit.point.y + groundOffset;
            transform.position = newPosition;

            _currentGroundNormal = hit.normal;
        }
    }

    public float GetGroundHeight(Vector3 position)
    {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance + 10f))
            return hit.point.y + groundOffset;
        

        return position.y;
    }

    public Vector3 GetGroundNormal(Vector3 position)
    {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance + 10f))
            return hit.normal;
        

        return Vector3.up;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * rayDistance);
        
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, _currentGroundNormal * 2f);
        }
    }
}