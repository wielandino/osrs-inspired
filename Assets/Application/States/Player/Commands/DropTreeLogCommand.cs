using System.Collections.Generic;
using UnityEngine;

public class DropTreeLogCommand : PlayerCommandBase
{

    private Vector3 _targetPosition;

    public DropTreeLogCommand(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }

    public static PlayerCommandBase[] Create(PlayerStateManager player, Vector3 targetPosition)
    {
        var commands = new List<PlayerCommandBase>();

        var interactionTiles = ObjectHelper.CollectInteractionTilesOfPosition(targetPosition);

        if (interactionTiles == null)
            return commands.ToArray();
        
        var nearestInteractionTile = PlayerMovementService.Instance.GetNearestInteractionTile(interactionTiles);

        if(!PlayerMovementService.Instance.IsPlayerInInteractionTile(interactionTiles))
        {
            commands.Add(new MoveCommand(nearestInteractionTile));
        }
        
        commands.Add(new DropTreeLogCommand(targetPosition));
        return commands.ToArray();
    }

    public override void Cancel(PlayerStateManager player)
    {
        throw new System.NotImplementedException();
    }

     public override bool CanExecute(PlayerStateManager player)
    {
        if (!player.IsInCarryingState())
            return false;
            
        var carriedTreeLog = player.CarryingState.GetCarriedTreeLog();
        if (carriedTreeLog == null)
            return false;
        
        // Jetzt sollte der Spieler in der Nähe sein (durch MoveCommand)
        var listOfInteractionTiles = ObjectHelper.CollectInteractionTilesOfPosition(_targetPosition);
        if (listOfInteractionTiles.Count > 0)
        {
            return PlayerMovementService.Instance.IsPlayerInInteractionTile(listOfInteractionTiles);
        }
        
        return true; // Kein Interaction Tile nötig
    }

    public override void ExecuteInternal(PlayerStateManager player)
    {
        var carriedTreeLog = player.CarryingState.GetCarriedTreeLog();
        player.CarryingState.DropTreeLog(player, this, _targetPosition);

        _isComplete =
            player.IsInIdleState() &&
            !player.IsInCarryingState() &&
            carriedTreeLog.GetStateManager().IsInIdleState();
    }

    public override bool IsComplete(PlayerStateManager player)
    {
        return _isComplete;
    }
}