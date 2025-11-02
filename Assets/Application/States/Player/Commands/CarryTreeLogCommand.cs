using UnityEditor.PackageManager;

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


    public bool CanExecute(PlayerStateManager player, out CommandErrorCode errorCode)
    {
        errorCode = CommandErrorCode.Default;

        if (player.IsInCarryingState())
        {
            errorCode = CommandErrorCode.PlayerAlreadyPerformingTask;
            return false;
        }

        if (!_treeLog.InteractionTiles.Contains(player.transform.position))
        {
            errorCode = CommandErrorCode.PlayerNotInInteractionTile;
            return false;
        }

        if(!_treeLog.GetStateManager().IsInIdleState())
        {
            errorCode = CommandErrorCode.TreeLogNotInIdleState;
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