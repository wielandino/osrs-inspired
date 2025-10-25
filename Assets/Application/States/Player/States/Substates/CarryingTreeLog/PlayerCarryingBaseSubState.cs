public abstract class PlayerCarryingBaseSubState
{
    public abstract void EnterState(PlayerCarryingState parentState, PlayerStateManager player);
    public abstract void UpdateState(PlayerCarryingState parentState, PlayerStateManager player);
    public abstract void ExitState(PlayerCarryingState parentState, PlayerStateManager player);
}