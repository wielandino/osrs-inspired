public class BurnTreeLogCommand : PlayerCommandBase
{
    private readonly TreeLog _treeLog;

    public BurnTreeLogCommand(TreeLog treeLog)
    {
        _treeLog = treeLog;
    }

    public override bool CanExecute(PlayerStateManager player)
        => CanExecute(player, out _);


    public bool CanExecute(PlayerStateManager player, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (!player.IsInIdleState())
        {
            errorMessage = "PlayerNotInIdleState";
            return false;
        }

        if (!_treeLog.GetStateManager().IsInIdleState())
        {
            errorMessage = "TreeLogNotInIdleState";
            return false;
        }

        if (!_treeLog.InteractionTiles.Contains(player.transform.position))
        {
            errorMessage = "PlayerNotInInteractionTile";
            return false;
        }

        return true;
    }

    public override void ExecuteInternal(PlayerStateManager player)
    {
        throw new System.NotImplementedException();
    }
}