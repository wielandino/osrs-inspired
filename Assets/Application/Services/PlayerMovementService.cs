using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;

public class PlayerMovementService : MonoBehaviour
{
    public static PlayerMovementService Instance { get; private set;}

    public float TimePerTile = 0.3f;
    public Seeker Seeker { get; private set; }
    public List<Vector3> PathPoints { get; private set; } = new();

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Seeker = GetComponent<Seeker>();
    }

    public Vector3 GetNearestInteractionTile(List<Vector3> interactionTiles)
    {
        Vector3 playerPos = transform.position;
        Vector3 nearest = interactionTiles[0];
        float minDistance = Vector3.Distance(playerPos, nearest);

        foreach (var tile in interactionTiles)
        {
            float distance = Vector3.Distance(playerPos, tile);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = tile;
            }
        }

        return nearest;
    }

    public bool IsPlayerInInteractionTile(List<Vector3> interactionTiles)
    {
        if (interactionTiles.Contains(transform.position))
            return true;

        return false;
    }
}