using System.Collections.Generic;
using UnityEngine;

public class TreeLog : MonoBehaviour, IHasInteractionTiles, IInteractable
{
    private TreeLogStateManager _stateManager;
    
    public List<Vector3> InteractionTiles = new();
    private float _tileSize;
    
    public GameObject TreeLogIdleStateObject;
    public GameObject TreeLogBurningStateObject;

    public GameObject CurrentActiveStateObject;

    public bool _isInteractable;
    public float XPDropPerFiremaking = 5f;

    private void Awake()
    {
        _stateManager = gameObject.GetComponent<TreeLogStateManager>();
    }

    private void Start()
    {
        _tileSize = GridManager.Instance.UnityGridSize;
        InteractionTiles = ObjectHelper.CollectInteractionTilesOfPosition(gameObject.transform.position);
    }

    protected void SetInteractable(bool status)
    {
        _isInteractable = status;
    }

    public void OnInteract(PlayerStateManager player)
    {
        if (!_isInteractable || GetStateManager() == null)
            return;

        GetStateManager().OnInteractInCurrentState(player);
    }

    public Collider GetColliderFromActiveStateObject()
        => CurrentActiveStateObject.GetComponent<Collider>();

    public TreeLogStateManager GetStateManager()
        => _stateManager;
   
    public string GetDisplayName()
        => "Treelog";

    public void RecalculateInteractionTiles()
    {
        Debug.Log("Recalculation for Treelog started!");
        InteractionTiles = ObjectHelper.CollectInteractionTilesOfPosition(gameObject.transform.position);
    }

    public void SetCorrectYPositionForAllStateModelObjects(Vector3 targetPosition)
    {
        ObjectHelper.SetChildModelLocalYPosition(TreeLogIdleStateObject);
        ObjectHelper.SetChildModelLocalYPosition(TreeLogBurningStateObject);
    }

    public bool IsInteractable()
        => _isInteractable;

    public List<ContextMenuOption> GetContextMenuOptions(PlayerStateManager player)
    {
        var options = new List<ContextMenuOption>();

        var carryTreeLogCommand = new CarryTreeLogCommand(this);
        var moveCommand =
            new MoveCommand(PlayerMovementService.Instance.GetNearestInteractionTile(InteractionTiles));

        if (!player.IsInCarryingState())
        {
            options.Add(
                new(
                    "Pick up",
                    () => player.ReplaceCommands(moveCommand, carryTreeLogCommand)
                )
            );
        }

        if (player.IsInIdleState() &&
            player.PlayerInventory.HasValidToolForSkill(SkillType.Firemaking, player.PlayerSkills) &&
            !GetStateManager().IsInBurningState())
        {

            var burnTreeLogCommand = new BurnTreeLogCommand(this);

            options.Add(
                new(
                    "Burn",
                    () => player.ReplaceCommands(moveCommand, burnTreeLogCommand)
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

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Zeige freie Interaction Tiles in Grün
        Gizmos.color = Color.green;
        Vector3 tileSize = new(_tileSize, 0.1f, _tileSize);

        foreach (var interactionTile in InteractionTiles)
        {
            Gizmos.DrawWireCube(interactionTile, tileSize);
        }
    }
}