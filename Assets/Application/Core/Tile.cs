using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, IInteractable
{
    public enum TileType
    {
        WalkableTile,
        WaterTile
    }

    [SerializeField]
    private TileType _tileType;

    public TileType GetTileType()
        => _tileType;

    public List<ContextMenuOption> GetContextMenuOptions(PlayerStateManager player)
    {
        var options = new List<ContextMenuOption>();

        var moveCommand = new MoveCommand(transform.position);

        options.Add(
            new(
                "Walk here",
                () => player.ReplaceCommands(moveCommand)
            )
        );

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
}