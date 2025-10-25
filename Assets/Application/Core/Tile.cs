using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, IInteractable
{
    public List<ContextMenuOption> GetContextMenuOptions(PlayerStateManager player)
    {
        var options = new List<ContextMenuOption>();

        var moveCommand = new MoveCommand(transform.position);

        options.Add(
            new(
                "Walk here",
                (p) => p.ReplaceCommands(moveCommand),
                (p) => true // Immer verfügbar
            )
        );

        if(player.IsInCarryingState())
        {
            options.Add(
                new(
                    "Drop Treelog",
                    (p) => p.ReplaceCommands(DropTreeLogCommand.Create(p, transform.position)),
                    (p) => true
                )
            );
        }
        
        return options;
    }

    public string GetDisplayName()
        => "Tile";
}