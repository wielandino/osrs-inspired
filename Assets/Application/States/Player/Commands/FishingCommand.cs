public class FishingCommand : PlayerCommandBase
{
    private readonly FishingSpot _fishingSpot;

    public FishingCommand(FishingSpot fishingSpot)
    {
        _fishingSpot = fishingSpot;
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

        if (!_fishingSpot.GetInteractionTiles().Contains(player.transform.position))
        {
            errorCode = CommandErrorCode.PlayerNotInInteractionTile;
            return false;
        }

        if(!player.PlayerInventory.HasValidToolForSkill(SkillType.Fishing, player.PlayerSkills) ||
            player.PlayerSkills.GetFishingSkill().CurrentLevel < _fishingSpot.GetRequiredLevelToInteract())
        {
            errorCode = CommandErrorCode.PlayerSkillRequirementNotMet;
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