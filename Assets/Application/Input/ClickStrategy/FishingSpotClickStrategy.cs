using System.Collections.Generic;
using UnityEngine;

public class FishingSpotClickStrategy : IClickStrategy
{
    private readonly PlayerMovementService _movementService;
    private readonly PlayerStateManager _playerStateManager;

    public FishingSpotClickStrategy(PlayerMovementService movementService, PlayerStateManager playerStateManager)
    {
        _movementService = movementService;
        _playerStateManager = playerStateManager;
    }

    public bool CanHandle(RaycastHit hit)
        => hit.collider.TryGetComponent<FishingSpot>(out _);

    public void Handle(RaycastHit hit)
    {
        if (!hit.collider.TryGetComponent<FishingSpot>(out var fishingSpot))
            return;

        var selectedItem = _playerStateManager.PlayerInventory.SelectedItem;

        // Check first if the player has a Item selected that can be used for the skill
        if (selectedItem != null)
        {
            if (!ToolValidator.CanToolBeUsedForSkill(selectedItem, SkillType.Fishing))
                return;

            ExecuteToolCallback(fishingSpot, selectedItem);
            return;
        }

        // If there is no tool get the best Fishing rod that the player can use
        if (_playerStateManager.PlayerInventory.HasValidToolForSkill(SkillType.Fishing,
                                                                     _playerStateManager.PlayerSkills))
        {
            ExecuteWithBestTool(fishingSpot);
            return;
        }

        // Examine
        Debug.Log("This is a Fishing place");
    }

    private void ExecuteToolCallback(FishingSpot fishingSpot, Item selectedItem)
    {
        if (selectedItem?.Callback == null)
            return;

        Vector3 nearestTile = _movementService.GetNearestInteractionTile(fishingSpot.GetInteractionTiles());
        var command = selectedItem.Callback.ExecuteCallback(fishingSpot.gameObject, _playerStateManager, selectedItem);

        if (command != null)
        {
            var moveCommand = new MoveCommand(nearestTile);
            _playerStateManager.AddCommands(moveCommand, command);
        }

        _playerStateManager.PlayerInventory.DeSelectCurrentItem();
    }

    private void ExecuteWithBestTool(FishingSpot fishingSpot)
    {
        if (!_playerStateManager.PlayerInventory.HasValidToolForSkill(SkillType.Fishing,
                                                                      _playerStateManager.PlayerSkills))
            return;

        var bestFishingRod = _playerStateManager.PlayerInventory.GetBestToolForSkill(SkillType.Fishing,
                                                                      _playerStateManager.PlayerSkills
        );

        var fishingCommand = new FishingCommand(fishingSpot, bestFishingRod);

        if (fishingCommand.CanExecute(_playerStateManager, out var errorCode))
        {
            _playerStateManager.AddCommands(fishingCommand);
        }
        else if (errorCode == CommandErrorCode.PlayerNotInInteractionTile)
        {
            Vector3 nearestTile = _movementService.GetNearestInteractionTile(fishingSpot.GetInteractionTiles());
            var moveCommand = new MoveCommand(nearestTile);
            _playerStateManager.AddCommands(moveCommand, fishingCommand);
        }
    }

    public List<ContextMenuOption> GetContextMenuOptions(RaycastHit hit)
    {
        var options = new List<ContextMenuOption>();

        if (!hit.collider.TryGetComponent<FishingSpot>(out var fishingSpot))
            return options;

        var bestFishingRod = _playerStateManager.PlayerInventory.GetBestToolForSkill(SkillType.Fishing,
                                                                                     _playerStateManager.PlayerSkills
        );
        
        if (CanFishSpot(fishingSpot, bestFishingRod))
            options.Add(CreateFishSpotOption(fishingSpot, bestFishingRod));

        options.Add(new ContextMenuOption(
            displayText: "Examine",
            onExecute: () => Debug.Log($"A Fishing place."),
            label: "Fishing place"
        ));



        return options;
    }

    private bool CanFishSpot(FishingSpot fishingSpot, ISkillTool fishingRod)
    {
        return fishingSpot.GetFishingCapacity() > 0 &&
               _playerStateManager.PlayerInventory.HasValidToolForSkill(SkillType.Fishing,
                                                                        _playerStateManager.PlayerSkills);
    }

    private ContextMenuOption CreateFishSpotOption(FishingSpot fishingSpot, ISkillTool bestFishingRod)
    {
        return new ContextMenuOption(
            displayText: "Fishing",
            onExecute: () =>
                {
                    var moveCommand = new MoveCommand(
                        _movementService.GetNearestInteractionTile(fishingSpot.GetInteractionTiles())
                    );
                    var fishingCommand = new FishingCommand(fishingSpot, bestFishingRod);
                    _playerStateManager.AddCommands(moveCommand, fishingCommand);
                },

            label: "Fishing place"
        );
    }
}