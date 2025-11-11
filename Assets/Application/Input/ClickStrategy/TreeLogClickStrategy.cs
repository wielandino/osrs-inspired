using UnityEngine;

public class TreeLogClickStrategy : IClickStrategy
{
    private readonly PlayerStateManager _playerStateManager;
    private readonly PlayerMovementService _movementService;
    
    public int Priority => 2;

    public TreeLogClickStrategy(PlayerMovementService movementService,
                                PlayerStateManager playerStateManager)
    {
        _movementService = movementService;
        _playerStateManager = playerStateManager;
    }

    public bool CanHandle(RaycastHit hit)
    {
        return hit.collider.TryGetComponent<TreeLog>(out _) || 
               hit.collider.transform.parent.TryGetComponent<TreeLog>(out _);
    }

    public void Handle(RaycastHit hit)
    {
        TreeLog treeLog = null;
        
        if (hit.collider.TryGetComponent<TreeLog>(out var log))
            treeLog = log;
        else if (hit.collider.transform.parent.TryGetComponent<TreeLog>(out var parentLog))
            treeLog = parentLog;

        if (treeLog == null)
            return;

        if (treeLog.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            treeLog = GetTopTreeLogAtPosition(hit.transform.position);

        if (!treeLog.IsInteractable())
            return;

        var selectedItem = _playerStateManager.PlayerInventory.SelectedItem;

        if (selectedItem != null && 
            ToolValidator.CanToolBeUsedForSkill(selectedItem, SkillType.Firemaking))
        {
            ExecuteFiremaking(treeLog, selectedItem, _playerStateManager);
            return;
        }

        if (_playerStateManager.IsInIdleState() && treeLog.GetStateManager().IsInIdleState())
        {
            ExecuteCarrying(treeLog, _playerStateManager);
        }
    }

    private void ExecuteFiremaking(TreeLog treeLog, Item selectedItem, PlayerStateManager playerState)
    {
        if (selectedItem?.Callback == null)
            return;

        Vector3 nearestTile = _movementService.GetNearestInteractionTile(treeLog.InteractionTiles);
        var command = selectedItem.Callback.ExecuteCallback(treeLog.gameObject, playerState, selectedItem);
        
        if (command != null)
        {
            var moveCommand = new MoveCommand(nearestTile);
            playerState.AddCommands(moveCommand, command);
        }
    }

    private void ExecuteCarrying(TreeLog treeLog, PlayerStateManager playerState)
    {
        var carryCommand = new CarryTreeLogCommand(treeLog);

        if (carryCommand.CanExecute(playerState, out var errorCode))
        {
            playerState.AddCommands(carryCommand);
        }
        else if (errorCode == CommandErrorCode.PlayerNotInInteractionTile)
        {
            Vector3 nearestTile = _movementService.GetNearestInteractionTile(treeLog.InteractionTiles);
            var moveCommand = new MoveCommand(nearestTile);
            playerState.AddCommands(moveCommand, carryCommand);
        }
    }

    private TreeLog GetTopTreeLogAtPosition(Vector3 position)
    {
        Vector3 rayStart = new(position.x, 50f, position.z);
        Ray ray = new(rayStart, Vector3.down);

        RaycastHit[] hits = Physics.RaycastAll(ray, 100f, LayerMask.GetMask("Obstacle"));
        
        TreeLog topTreeLog = null;
        float highestY = float.MinValue;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent<TreeLog>(out var treeLog) ||
                hit.collider.transform.parent.TryGetComponent(out treeLog))
            {
                if (treeLog.transform.position.y > highestY)
                {
                    highestY = treeLog.transform.position.y;
                    topTreeLog = treeLog;
                }
                
            }
        }
        
        return topTreeLog;
    }
}