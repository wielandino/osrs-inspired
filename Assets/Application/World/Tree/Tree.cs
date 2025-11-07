using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, IInteractable, IHasInteractionTiles
{
    [Header("Tree Stats")]
    public float MaxHealth;

    public float CurrentHealth;

    [Header("Leveling")]
    [SerializeField]
    private int _requiredLevelToCut = 1;

    public float XPDropPerCut = 5f;

    [Header("Other")]
    public float RespawnTime = 15f;

    [Header("Tree Models")]
    [SerializeField]
    private GameObject _defaultTreeModel;

    [SerializeField]
    private GameObject _treeTrunkModel;

    [SerializeField]
    private GameObject _treeLogPrefab;

    public bool IsDestroyed;

    private int _tileSize;

    public LayerMask ObstacleLayerMask;

    public Vector3 TreePosition { get; private set; }

    public List<Vector3> InteractionTiles = new();

    private TreeStateManager _treeStateManager;

    private void Start()
    {
        _treeStateManager = gameObject.GetComponent<TreeStateManager>();
        ObstacleLayerMask = LayerMask.GetMask("Obstacle");

        _tileSize = GridManager.Instance.UnityGridSize;
        TreePosition = transform.position;
        CurrentHealth = MaxHealth;

        InteractionTiles = ObjectHelper.CollectInteractionTilesOfPosition(gameObject.transform.position);
    }


    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Vector3 tileSize = new(_tileSize, 1f, _tileSize);

        foreach(var interactionTile in InteractionTiles)
        {
            Gizmos.DrawWireCube(interactionTile, tileSize);
        }
    }

    public int GetRequiredLevelToCut()
        => _requiredLevelToCut;

    public GameObject TreeLogPrefab => _treeLogPrefab;

    public void SwitchToIdleModel()
    {
        if (_defaultTreeModel != null)
            _defaultTreeModel.SetActive(true);
        

        if (_treeTrunkModel != null)
            _treeTrunkModel.SetActive(false);
        
    }

    public void SwitchToDestroyedModel()
    {
        if (_defaultTreeModel != null)
            _defaultTreeModel.SetActive(false);
        

        if (_treeTrunkModel != null)
            _treeTrunkModel.SetActive(true);
    }

    public List<ContextMenuOption> GetContextMenuOptions(PlayerStateManager player)
    {
        var options = new List<ContextMenuOption>();

        if(_treeStateManager.IsInIdleState())
        {
            var moveCommand = new MoveCommand(transform.position);
            var woodcuttingCommand = new WoodcuttingCommand(this);

            options.Add(
                new(
                    "Chop tree",
                    () => player.AddCommands(moveCommand, woodcuttingCommand)
                )
            );
        }
        
        return options;
    }

    public void RecalculateInteractionTiles()
    {
        InteractionTiles = ObjectHelper.CollectInteractionTilesOfPosition(gameObject.transform.position);
    }
}