public class CookingCommand : PlayerCommandBase
{
    private readonly TreeLog _targetTreeLog;
    private readonly CookableItem _itemToCook;

    public CookingCommand(TreeLog treeLog, CookableItem item)
    {
        _targetTreeLog = treeLog;
        _itemToCook = item;
    }

    public override bool CanExecute(PlayerStateManager player)
        => CanExecute(player, out _);

    public bool CanExecute(PlayerStateManager player, out CommandErrorCode errorCode)
    {
        errorCode = CommandErrorCode.Default;

        if (!_targetTreeLog.GetStateManager().IsInBurningState())
        {
            errorCode = CommandErrorCode.NoTarget;
            return false;
        }

        if (!player.IsInIdleState())
        {
            errorCode = CommandErrorCode.PlayerNotInIdleState;
            return false;
        }

        if (_itemToCook.ReturnItem == null)
        {
            errorCode = CommandErrorCode.FatalError;
            return false;
        }

        if (!_targetTreeLog.InteractionTiles.Contains(player.transform.position))
        {
            errorCode = CommandErrorCode.PlayerNotInInteractionTile;
            return false;
        }
        
        if (player.PlayerSkills.GetCookingSkill().CurrentLevel < _itemToCook.RequiredCookingLevel)
        {
            errorCode = CommandErrorCode.PlayerSkillRequirementNotMet;
            return false;
        }

        return true;
    }

    public override void ExecuteInternal(PlayerStateManager player)
    {
        player.SwitchToCookingState(_targetTreeLog, _itemToCook);
        _isComplete = true;
    }
}