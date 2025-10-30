using UnityEngine;

public class CarryTreeLogCommand : PlayerCommandBase
{
    private readonly TreeLog _treeLog;

    public CarryTreeLogCommand(TreeLog treeLog)
    {
        _treeLog = treeLog;
    }

    public override void Cancel(PlayerStateManager player)
    {
    }

    public override bool CanExecute(PlayerStateManager player)
        => CanExecute(player, out _);


    public bool CanExecute(PlayerStateManager player, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (player.IsInCarryingState())
        {
            errorMessage = "IsInCarryingState";
            return false;
        }

        if (!_treeLog.InteractionTiles.Contains(player.transform.position))
        {
            errorMessage = "NotInInteractionTile";
            return false;
        }

        return true;
    }

    public override void ExecuteInternal(PlayerStateManager player)
    {
        _treeLog.OnInteract(player);        
        _isComplete = true;
    }

    public override bool IsComplete(PlayerStateManager player)
        => _isComplete;
}