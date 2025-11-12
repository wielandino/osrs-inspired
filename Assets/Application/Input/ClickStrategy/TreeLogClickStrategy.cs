using System.Collections.Generic;
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

        if (selectedItem != null)
        {
            if (ToolValidator.CanToolBeUsedForSkill(selectedItem, SkillType.Firemaking))
            {
                if (!treeLog.IsTreeLogStacked() && treeLog.GetStateManager().IsInIdleState())
                {
                    ExecuteFiremaking(treeLog, selectedItem);
                    return;
                }
            }

            if (treeLog.GetStateManager().IsInBurningState())
            {
                if (selectedItem is CookableItem)
                {
                    ExecuteCooking(treeLog, selectedItem);
                    return;
                }
            }
        }
        
        if (_playerStateManager.IsInIdleState() && treeLog.GetStateManager().IsInIdleState())
            ExecuteCarrying(treeLog);
        
    }

    private void ExecuteCooking(TreeLog treeLog, Item selectedItem)
    {
        if (selectedItem?.Callback == null ||
            selectedItem?.Callback is not CookableItemCallback)
            return;

        Vector3 nearestTile = _movementService.GetNearestInteractionTile(treeLog.InteractionTiles);
        var command = selectedItem.Callback.ExecuteCallback(treeLog.gameObject, _playerStateManager, selectedItem);

        if (command != null)
        {
            var moveCommand = new MoveCommand(nearestTile);
            _playerStateManager.AddCommands(moveCommand, command);
        }

        _playerStateManager.PlayerInventory.DeSelectCurrentItem();
    }

    private void ExecuteFiremaking(TreeLog treeLog, Item selectedItem)
    {
        if (selectedItem?.Callback == null)
            return;

        Vector3 nearestTile = _movementService.GetNearestInteractionTile(treeLog.InteractionTiles);
        var command = selectedItem.Callback.ExecuteCallback(treeLog.gameObject, _playerStateManager, selectedItem);

        if (command != null)
        {
            var moveCommand = new MoveCommand(nearestTile);
            _playerStateManager.AddCommands(moveCommand, command);
        }

        _playerStateManager.PlayerInventory.DeSelectCurrentItem();
    }

    private void ExecuteCarrying(TreeLog treeLog)
    {
        var carryCommand = new CarryTreeLogCommand(treeLog);

        if (carryCommand.CanExecute(_playerStateManager, out var errorCode))
        {
            _playerStateManager.AddCommands(carryCommand);
        }
        else if (errorCode == CommandErrorCode.PlayerNotInInteractionTile)
        {
            Vector3 nearestTile = _movementService.GetNearestInteractionTile(treeLog.InteractionTiles);
            var moveCommand = new MoveCommand(nearestTile);
            _playerStateManager.AddCommands(moveCommand, carryCommand);
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

    public List<ContextMenuOption> GetContextMenuOptions(RaycastHit hit)
    {
        var options = new List<ContextMenuOption>();

        TreeLog treeLog = GetTreeLogFromHit(hit);
        if (treeLog == null)
            return options;

        if (CanPickUp(treeLog))
            options.Add(CreatePickUpOption(treeLog));
        

        if (CanBurn(treeLog))
            options.Add(CreateBurnOption(treeLog));
        

        if (CanDrop(treeLog))
            options.Add(CreateDropOption(treeLog));


        if (CanCook(treeLog))
            options.Add(CreateCookOption(treeLog));

        return options;
    }

    private TreeLog GetTreeLogFromHit(RaycastHit hit)
    {
        TreeLog treeLog = null;

        if (hit.collider.TryGetComponent<TreeLog>(out var log))
            treeLog = log;
        else if (hit.collider.transform.parent.TryGetComponent<TreeLog>(out var parentLog))
            treeLog = parentLog;

        if (treeLog != null && treeLog.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            treeLog = GetTopTreeLogAtPosition(hit.transform.position);

        return treeLog;
    }

    private bool CanCook(TreeLog treeLog)
    {
        if (!treeLog.GetStateManager().IsInBurningState())
            return false;

        var selectedItem = _playerStateManager.PlayerInventory.SelectedItem;

        if (selectedItem is not CookableItem)
            return false;

        var cookingCommand = new CookingCommand(treeLog, selectedItem as CookableItem);

        if (!cookingCommand.CanExecute(_playerStateManager))
            return false;

        return true;
    }

    private bool CanPickUp(TreeLog treeLog)
    {
        return !_playerStateManager.IsInCarryingState() &&
               treeLog.IsInteractable() &&
               _playerStateManager.IsInIdleState() &&
               treeLog.GetStateManager().IsInIdleState();
    }

    private bool CanBurn(TreeLog treeLog)
    {
        return _playerStateManager.IsInIdleState() &&
               _playerStateManager.PlayerInventory.HasValidToolForSkill(SkillType.Firemaking,
                                                                        _playerStateManager.PlayerSkills) &&
                                                                        !treeLog.GetStateManager().IsInBurningState() &&
                                                                        !treeLog.IsTreeLogStacked();
    }

    private bool CanDrop(TreeLog treeLog)
    {
        if (!_playerStateManager.IsInCarryingState())
            return false;

        if (ObjectHelper.HasObstacleAtPosition(treeLog.transform.position, out var obstacle))
        {
            if (obstacle == null)
                return true;

            if(obstacle.TryGetComponent<TreeLog>(out var obstacleTreeLog) ||
               obstacle.transform.parent.TryGetComponent(out obstacleTreeLog))

            if (obstacleTreeLog != null)
                return obstacleTreeLog.CanBeStacked;

            return false;
        }
        

        return true;
    }

    private ContextMenuOption CreateCookOption(TreeLog treeLog)
    {
        return new ContextMenuOption(
            "Cook",
            () =>
                {
                    var cookCommand =
                        new CookingCommand(treeLog, _playerStateManager.PlayerInventory.SelectedItem as CookableItem);
                        
                    var moveCommand = new MoveCommand(
                        _movementService.GetNearestInteractionTile(treeLog.InteractionTiles)
                    );
                    _playerStateManager.AddCommands(moveCommand, cookCommand);
                }
        );
    }

    private ContextMenuOption CreatePickUpOption(TreeLog treeLog)
    {
        return new ContextMenuOption(
            "Pick up",
            () =>
                {
                    var carryCommand = new CarryTreeLogCommand(treeLog);
                    var moveCommand = new MoveCommand(
                        _movementService.GetNearestInteractionTile(treeLog.InteractionTiles)
                    );
                    _playerStateManager.AddCommands(moveCommand, carryCommand);
                }
        );
    }

    private ContextMenuOption CreateBurnOption(TreeLog treeLog)
    {
        return new ContextMenuOption(
            "Burn",
            () =>
                {
                    var burnCommand = new BurnTreeLogCommand(treeLog);
                    var moveCommand = new MoveCommand(
                        _movementService.GetNearestInteractionTile(treeLog.InteractionTiles)
                    );
                    _playerStateManager.AddCommands(moveCommand, burnCommand);
                }
        );
    }
    
    private ContextMenuOption CreateDropOption(TreeLog treeLog)
    {
        return new ContextMenuOption(
            "Drop Treelog",
            () => _playerStateManager.AddCommands(DropTreeLogCommand.Create(_playerStateManager, treeLog.transform.position))
        );
    }
}