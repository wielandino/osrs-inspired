using System.Collections.Generic;
using UnityEngine;

public class TreeClickStrategy : IClickStrategy
{
    private readonly PlayerMovementService _movementService;
    private readonly PlayerStateManager _playerStateManager;

    public TreeClickStrategy(PlayerMovementService movementService, PlayerStateManager playerStateManager)
    {
        _movementService = movementService;
        _playerStateManager = playerStateManager;
    }

    public bool CanHandle(RaycastHit hit)
        => hit.collider.TryGetComponent<Tree>(out _);

    public void Handle(RaycastHit hit)
    {
        if (!hit.collider.TryGetComponent<Tree>(out var tree))
            return;

        var selectedItem = _playerStateManager.PlayerInventory.SelectedItem;

        // Check first if the player has a Item selected that can be used for the skill
        if (selectedItem != null)
        {
            if (!ToolValidator.CanToolBeUsedForSkill(selectedItem, SkillType.Woodcutting))
                return;

            ExecuteToolCallback(tree, selectedItem);
            return;
        }

        // If there is no tool get the best Woodcutting Axe that the player can use
        if (_playerStateManager.PlayerInventory.HasValidToolForSkill(SkillType.Woodcutting,
                                                                     _playerStateManager.PlayerSkills))
        {
            ExecuteWithBestTool(tree);
            return;
        }

        // Examine
        Debug.Log("This is a tree");
    }

    private void ExecuteToolCallback(Tree tree, Item selectedItem)
    {
        if (selectedItem?.Callback == null)
            return;

        Vector3 nearestTile = _movementService.GetNearestInteractionTile(tree.InteractionTiles);
        var command = selectedItem.Callback.ExecuteCallback(tree.gameObject, _playerStateManager, selectedItem);

        if (command != null)
        {
            var moveCommand = new MoveCommand(nearestTile);
            _playerStateManager.AddCommands(moveCommand, command);
        }

        _playerStateManager.PlayerInventory.DeSelectCurrentItem();
    }

    private void ExecuteWithBestTool(Tree tree)
    {
        if (!_playerStateManager.PlayerInventory.HasValidToolForSkill(SkillType.Woodcutting,
                                                                      _playerStateManager.PlayerSkills))
            return;

        var bestAxe = _playerStateManager.PlayerInventory.GetBestToolForSkill(SkillType.Woodcutting,
                                                                      _playerStateManager.PlayerSkills
        );

        var woodcuttingCommand = new WoodcuttingCommand(tree, bestAxe);

        if (woodcuttingCommand.CanExecute(_playerStateManager, out var errorCode))
        {
            _playerStateManager.AddCommands(woodcuttingCommand);
        }
        else if (errorCode == CommandErrorCode.PlayerNotInInteractionTile)
        {
            Vector3 nearestTile = _movementService.GetNearestInteractionTile(tree.InteractionTiles);
            var moveCommand = new MoveCommand(nearestTile);
            _playerStateManager.AddCommands(moveCommand, woodcuttingCommand);
        }
    }

    public List<ContextMenuOption> GetContextMenuOptions(RaycastHit hit)
    {
        var options = new List<ContextMenuOption>();

        if (!hit.collider.TryGetComponent<Tree>(out var tree))
            return options;

        if (CanChopTree(tree))
            options.Add(CreateChopTreeOption(tree));

        options.Add(new ContextMenuOption(
            displayText: "Examine",
            onExecute: () => Debug.Log($"A tree."),
            label: tree.TreeName
        ));

        return options;
    }

    private bool CanChopTree(Tree tree)
    {
        return tree.GetStateManager().IsInIdleState() &&
               _playerStateManager.PlayerInventory.HasValidToolForSkill(
                   SkillType.Woodcutting,
                   _playerStateManager.PlayerSkills
               );
    }
    
    private ContextMenuOption CreateChopTreeOption(Tree tree)
    {
        var bestAxe = _playerStateManager.PlayerInventory.GetBestToolForSkill(
            SkillType.Woodcutting, 
            _playerStateManager.PlayerSkills
        );

        return new ContextMenuOption(
            displayText: $"Chop {tree.TreeName}",
            onExecute: () => {
                var moveCommand = new MoveCommand(
                    _movementService.GetNearestInteractionTile(tree.InteractionTiles)
                );
                var woodcuttingCommand = new WoodcuttingCommand(tree, bestAxe);
                _playerStateManager.AddCommands(moveCommand, woodcuttingCommand);
            },

            label: tree.TreeName
        );
    }
}