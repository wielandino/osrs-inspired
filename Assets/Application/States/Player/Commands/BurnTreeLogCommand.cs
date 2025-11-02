using System.Diagnostics;

public class BurnTreeLogCommand : PlayerCommandBase
{
    private readonly TreeLog _treeLog;

    public BurnTreeLogCommand(TreeLog treeLog)
    {
        _treeLog = treeLog;
    }

    public override bool CanExecute(PlayerStateManager player)
        => CanExecute(player, out _);


    public bool CanExecute(PlayerStateManager player, out CommandErrorCode errorCode)
    {
        errorCode = CommandErrorCode.Default;

        if (!player.IsInIdleState())
        {
            errorCode = CommandErrorCode.PlayerNotInIdleState;
            return false;
        }

        if (!_treeLog.GetStateManager().IsInIdleState())
        {
            errorCode = CommandErrorCode.TreeLogNotInIdleState;
            return false;
        }

        if (!_treeLog.InteractionTiles.Contains(player.transform.position))
        {
            errorCode = CommandErrorCode.PlayerNotInInteractionTile;
            return false;
        }

        return true;
    }

    public override void ExecuteInternal(PlayerStateManager player)
    {
        _treeLog.GetStateManager().SwitchState(_treeLog.GetStateManager().BurningState);
        _isComplete = true;
    }
}