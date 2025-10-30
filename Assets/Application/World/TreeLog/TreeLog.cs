using System.Collections.Generic;
using UnityEngine;

public class TreeLog : MonoBehaviour, IInteractable, IHasInteractionTiles
{
    [SerializeField]
    private bool _isInteractable = true;

    private TreeLogStateManager _stateManager;
    private Collider _collider;
    
    private float _tileSize;
    public List<Vector3> InteractionTiles = new();
    
    private void Awake()
    {
        _stateManager = gameObject.GetComponent<TreeLogStateManager>();
        _collider = gameObject.GetComponent<Collider>();
    }

    void Start()
    {
        _tileSize = GridManager.Instance.UnityGridSize;

        InteractionTiles = ObjectHelper.CollectInteractionTilesOfPosition(gameObject.transform.position);
    }

    public void SetInteractable(bool interactable)
    {
        _isInteractable = interactable;

        if (_collider != null)
        {
            _collider.enabled = interactable;
        }
    }

    public bool IsInteractable()
    {
        return _isInteractable;
    }

    public TreeLogStateManager GetStateManager()
    {
        return _stateManager;
    }

    public void OnInteract(PlayerStateManager player)
    {
        if (!_isInteractable || _stateManager == null)
            return;

        _stateManager.OnInteract(player);
    }

    public List<ContextMenuOption> GetContextMenuOptions(PlayerStateManager player)
    {
        var options = new List<ContextMenuOption>();

        var carryTreeLogCommand = new CarryTreeLogCommand(this);
        var moveCommand = new MoveCommand(PlayerMovementService.Instance.GetNearestInteractionTile(InteractionTiles));

        if (!player.IsInCarryingState())
        {
            options.Add(
                new(
                    "Pick up",
                    () => player.ReplaceCommands(moveCommand, carryTreeLogCommand)
                )
            );
        }

        if (GetStateManager().IsInIdleState() &&
            player.IsInIdleState() &&
            player.PlayerInventory.HasValidToolForSkill(SkillType.Firemaking, player.PlayerSkills))
        {
            options.Add(
                new(
                    "Burn",
                    () => Debug.Log("Firemaking Command execute")
                )
            );
        }

        if (player.IsInCarryingState())
        {
            options.Add(
                new(
                    "Drop Treelog",
                    () => player.ReplaceCommands(DropTreeLogCommand.Create(player, transform.position))
                )
            );
        }

        return options;
    }

    public string GetDisplayName()
        => "Treelog";

    public void RecalculateInteractionTiles()
    {
        Debug.Log("Recalculation for Treelog started!");
        InteractionTiles = ObjectHelper.CollectInteractionTilesOfPosition(gameObject.transform.position);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Zeige freie Interaction Tiles in Grün
        Gizmos.color = Color.green;
        Vector3 tileSize = new(_tileSize, 0.1f, _tileSize);

        foreach(var interactionTile in InteractionTiles)
        {
            Gizmos.DrawWireCube(interactionTile, tileSize);
        }
    }
}