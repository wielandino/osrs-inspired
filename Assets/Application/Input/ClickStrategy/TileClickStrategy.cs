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
}