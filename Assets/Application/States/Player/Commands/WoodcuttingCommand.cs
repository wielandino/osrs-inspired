public class WoodcuttingCommand : PlayerCommandBase
{
    private readonly Tree _targetTree;
    private readonly ISkillTool _woodcuttingAxe;

    public WoodcuttingCommand(Tree tree, ISkillTool woodcuttingAxe)
    {
        _targetTree = tree;
        _woodcuttingAxe = woodcuttingAxe;
    }

    public override void Cancel(PlayerStateManager player)
    {

    }

    public override bool CanExecute(PlayerStateManager player)
    {
        return CanExecute(player, out _);
    }

    public bool CanExecute(PlayerStateManager player, out CommandErrorCode errorCode)
    {
        var targetTreeState = _targetTree.GetComponent<TreeStateManager>();

        errorCode = CommandErrorCode.Default;

        if (!player.IsInIdleState())
        {
            errorCode = CommandErrorCode.PlayerNotInIdleState;
            return false;
        }

        if (_targetTree == null || targetTreeState.IsInDestroyedState())
        {
            errorCode = CommandErrorCode.NoTarget;
            return false;
        }

        if (!_targetTree.InteractionTiles.Contains(player.transform.position))
        {
            errorCode = CommandErrorCode.PlayerNotInInteractionTile;
            return false;
        }

        if (player.PlayerSkills.GetWoodcuttingSkill().CurrentLevel < _targetTree.GetRequiredLevelToCut() ||
            !_woodcuttingAxe.CanPlayerUseForSkill(SkillType.Woodcutting, player.PlayerSkills.GetWoodcuttingSkill().CurrentLevel))
        {
            errorCode = CommandErrorCode.PlayerSkillRequirementNotMet;
            return false;
        }

        return true;
    }

    public override void ExecuteInternal(PlayerStateManager player)
    {
        player.SwitchToWoodcuttingState(_targetTree, _woodcuttingAxe);
    }

    public override bool IsComplete(PlayerStateManager player)
        => player.IsInWoodcuttingState();
}