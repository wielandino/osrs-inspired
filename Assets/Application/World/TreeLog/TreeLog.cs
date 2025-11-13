using System.Collections.Generic;
using UnityEngine;

public class TreeLog : MonoBehaviour, IHasInteractionTiles
{
    private TreeLogStateManager _stateManager;
    public List<Vector3> InteractionTiles = new();
    public float XPDropPerFiremaking = 5f;
    public bool CanBeStacked = true;

    [Header("Model Objects")]
    public GameObject TreeLogIdleStateObject;
    public GameObject TreeLogBurningStateObject;
    public GameObject CurrentActiveStateObject;

    
    [Header("Needs Settings")]
    public float CarryEnergyDrain;
    public float CarryHungerDrain;

    private float _tileSize;

    private void Awake()
    {
        _stateManager = gameObject.GetComponent<TreeLogStateManager>();
    }

    private void Start()
    {
        _tileSize = GridManager.Instance.UnityGridSize;
        InteractionTiles = ObjectHelper.CollectInteractionTilesOfPosition(gameObject.transform.position);
    }

    public void OnInteract(PlayerStateManager player)
    {
        if (GetStateManager() == null)
            return;

        GetStateManager().OnInteractInCurrentState(player);
    }

    public Collider GetColliderFromActiveStateObject()
        => CurrentActiveStateObject.GetComponent<Collider>();

    public TreeLogStateManager GetStateManager()
        => _stateManager;

    public void RecalculateInteractionTiles()
    {
        InteractionTiles = ObjectHelper.CollectInteractionTilesOfPosition(gameObject.transform.position);
    }

    public void SetCorrectYPositionForAllStateModelObjects(Vector3 targetPosition)
    {
        ObjectHelper.SetChildModelLocalYPosition(TreeLogIdleStateObject);
        ObjectHelper.SetChildModelLocalYPosition(TreeLogBurningStateObject);
    }

    public void ChangeLayerOfObjectAndModelObjects(string layerMask)
    {
        gameObject.layer = LayerMask.NameToLayer(layerMask);

        foreach (Transform child in transform.GetComponentsInChildren<Transform>(true))  
                child.gameObject.layer = LayerMask.NameToLayer(layerMask);
        
    }

    public bool IsTreeLogStacked()
    {
        Vector3 position = transform.position;
        Vector3 rayStart = new(position.x, 50f, position.z);
        Ray ray = new(rayStart, Vector3.down);
        
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f, LayerMask.GetMask("Obstacle"));
        
        int treeLogCount = 0;
        
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent<TreeLog>(out _) ||
                hit.collider.transform.parent.TryGetComponent<TreeLog>(out _))
            {
                treeLogCount++;
            }
        }
        
        return treeLogCount > 1;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.green;
        Vector3 tileSize = new(_tileSize, 0.1f, _tileSize);

        foreach (var interactionTile in InteractionTiles)
        {
            Gizmos.DrawWireCube(interactionTile, tileSize);
        }
    }
}