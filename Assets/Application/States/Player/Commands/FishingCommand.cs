public class FishingCommand : PlayerCommandBase
{
    private readonly FishingSpot _fishingSpot;
    private readonly ISkillTool _fishingRod;
    public FishingCommand(FishingSpot fishingSpot, ISkillTool fishingRod)
    {
        _fishingSpot = fishingSpot;
        _fishingRod = fishingRod;
    }

    public override bool CanExecute(PlayerStateManager player)
        => CanExecute(player, out _);

    public bool CanExecute(PlayerStateManager player, out CommandErrorCode errorCode)
    {
        errorCode = CommandErrorCode.Default;

        if (player.PlayerNeeds.GetNeedValue(NeedType.Energy) <= _fishingSpot.EnergyDrain)
        {
            errorCode = CommandErrorCode.PlayerNoEnergy;
            return false;
        }

        if (!player.IsInIdleState())
        {
            errorCode = CommandErrorCode.PlayerNotInIdleState;
            return false;
        }

        if (_fishingSpot.GetInteractionTiles() == null ||
           !_fishingSpot.GetInteractionTiles().Contains(player.transform.position))
        {
            errorCode = CommandErrorCode.PlayerNotInInteractionTile;
            return false;
        }

        if (!_fishingRod.CanPlayerUseForSkill(SkillType.Fishing, player.PlayerSkills.GetFishingSkill().CurrentLevel) ||
            player.PlayerSkills.GetFishingSkill().CurrentLevel < _fishingSpot.GetRequiredLevelToInteract())
        {
            errorCode = CommandErrorCode.PlayerSkillRequirementNotMet;
            return false;
        }
        
        if (_fishingSpot.GetFishingCapacity() <= 0 ||
            _fishingSpot.GetListOfPossibleFishesToCatch() == null ||
            _fishingSpot.GetListOfPossibleFishesToCatch().Count <= 0)
        {
            errorCode = CommandErrorCode.NoTarget;
            return false;
        }

        return true;
    }

    public override void ExecuteInternal(PlayerStateManager player)
    {
        player.SwitchToFishingState(_fishingSpot);
        _isComplete = true;
    }
}