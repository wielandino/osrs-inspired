using System.Collections.Generic;
using UnityEngine;

public class TileClickStrategy : IClickStrategy
{
    private readonly PlayerStateManager _playerStateManager;

    public int Priority => 99;

    public TileClickStrategy(PlayerStateManager playerStateManager)
    {
        _playerStateManager = playerStateManager;
    }

    public bool CanHandle(RaycastHit hit)
    {
        if (!hit.collider.TryGetComponent<Tile>(out var tile))
            return false;

        return tile.GetTileType() == Tile.TileType.WalkableTile;
    }

    public void Handle(RaycastHit hit)
    {
        var moveCommand = new MoveCommand(hit.point);
        _playerStateManager.AddCommands(moveCommand);
    }

    public List<ContextMenuOption> GetContextMenuOptions(RaycastHit hit)
    {
        var options = new List<ContextMenuOption>();

        if (!hit.collider.TryGetComponent<Tile>(out var tile))
            return options;

        if (CanWalkToTile(tile))
            options.Add(CreateWalkHereOption(hit.point));
        

        if (CanDropTreeLog(tile))
            options.Add(CreateDropTreeLogOption(tile.transform.position));
        

        return options;
    }

    private bool CanWalkToTile(Tile tile)
        => tile.GetTileType() == Tile.TileType.WalkableTile;


    private bool CanDropTreeLog(Tile tile)
    {
        if (!_playerStateManager.IsInCarryingState())
            return false;

        if (tile.GetTileType() != Tile.TileType.WalkableTile)
            return false;

        if (ObjectHelper.HasObstacleAtPosition(tile.transform.position, out var obstacle))
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

    private ContextMenuOption CreateWalkHereOption(Vector3 position)
    {
        return new ContextMenuOption(
            displayText: "Walk here",
            onExecute: () => {
                var moveCommand = new MoveCommand(position);
                _playerStateManager.AddCommands(moveCommand);
            },

            label: "Tile"
        );
    }

    private ContextMenuOption CreateDropTreeLogOption(Vector3 position)
    {
        return new ContextMenuOption(
            displayText: "Drop TreelogTreelog",
            onExecute: () => {
                _playerStateManager.AddCommands(
                    DropTreeLogCommand.Create(_playerStateManager, position)
                );
            },

            label: "Tile"
        );
    }
}