using UnityEngine;

public class SimpleGroundSnap : MonoBehaviour
{
    [Header("Einstellungen")]
    [Tooltip("Wie weit soll der Ray nach unten schauen?")]
    public float rayDistance = 10f;
    
    [Tooltip("Kleiner Abstand über dem Boden (damit Spieler nicht IM Boden steckt)")]
    public float groundOffset = 0.1f;

    [Header("Mesh Rotation")]
    [Tooltip("Das Mesh das rotiert werden soll (z.B. PlayerMesh als Child)")]
    public Transform meshTransform;
    
    [Tooltip("Wie schnell soll sich das Mesh zur Bodenneigung drehen?")]
    public float rotationSpeed = 10f;

    private Vector3 _currentGroundNormal = Vector3.up;

    [Header("Layer Settings")]
    [Tooltip("Layer die ignoriert werden sollen (z.B. Items)")]
    public LayerMask ignoreLayer;

    void Update()
    {
        // Schritt 1: Schieße einen Ray vom Spieler nach unten
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        
        // Schritt 2: Prüfe ob der Ray etwas getroffen hat (ignoriere bestimmte Layer)
        int layerMask = ~ignoreLayer; // Invertiere die Maske
        if (Physics.Raycast(ray, out hit, rayDistance, layerMask))
        {
            // Schritt 3: Setze Spieler auf die Höhe wo der Ray den Boden getroffen hat
            Vector3 newPosition = transform.position;
            newPosition.y = hit.point.y + groundOffset;
            transform.position = newPosition;

            // Speichere die Boden-Normale (wird nicht mehr für Rotation benutzt, nur für Debug)
            _currentGroundNormal = hit.normal;
            
            // Mesh-Rotation wird NICHT mehr hier gesetzt!
            // Das macht jetzt das Movement-Script
        }
    }

    private void RotateMeshToGround()
    {
        // Berechne die Ziel-Rotation basierend auf der Boden-Normale
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, _currentGroundNormal);
        
        // Behalte die Y-Rotation (Blickrichtung) vom Parent bei
        Vector3 currentEuler = meshTransform.localRotation.eulerAngles;
        Vector3 targetEuler = targetRotation.eulerAngles;
        
        // Normalisiere Winkel zu -180 bis 180 Grad
        float normalizedX = targetEuler.x > 180 ? targetEuler.x - 360 : targetEuler.x;
        float normalizedZ = targetEuler.z > 180 ? targetEuler.z - 360 : targetEuler.z;
        
        // Kombiniere: Boden-Neigung (X und Z) + Blickrichtung (Y)
        // WICHTIG: Vorzeichen invertiert für korrekte Neigung
        Quaternion finalRotation = Quaternion.Euler(
            -normalizedX,  // Invertiert!
            currentEuler.y, // Behalte Y-Rotation
            -normalizedZ   // Invertiert!
        );
        
        // Smooth Rotation (nicht instant)
        meshTransform.localRotation = Quaternion.Slerp(
            meshTransform.localRotation, 
            finalRotation, 
            Time.deltaTime * rotationSpeed
        );
    }

    // NEUE METHODE: Andere Scripts können diese Methode aufrufen um die Bodenhöhe zu bekommen
    public float GetGroundHeight(Vector3 position)
    {
        Ray ray = new Ray(position + Vector3.up * 5f, Vector3.down);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, rayDistance + 10f))
        {
            return hit.point.y + groundOffset;
        }
        
        // Fallback: Wenn kein Boden gefunden wurde, gib die aktuelle Höhe zurück
        return position.y;
    }

    // NEUE METHODE: Hol dir die Boden-Normale an einer bestimmten Position
    public Vector3 GetGroundNormal(Vector3 position)
    {
        Ray ray = new Ray(position + Vector3.up * 5f, Vector3.down);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, rayDistance + 10f))
        {
            return hit.normal;
        }
        
        return Vector3.up; // Fallback: Flacher Boden
    }

    // NEUE METHODE: Setze die Mesh-Rotation manuell (für Movement Script)
    public void SetTargetGroundNormal(Vector3 normal)
    {
        _currentGroundNormal = normal;
        
        if (meshTransform != null)
        {
            RotateMeshToGround();
        }
    }

    // Damit du den Ray im Editor sehen kannst (nur zum Debuggen)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * rayDistance);
        
        // Zeige die Boden-Normale als grünen Pfeil
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, _currentGroundNormal * 2f);
        }
    }
}