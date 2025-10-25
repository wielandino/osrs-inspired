using UnityEngine;

public class MoveCommand : PlayerCommandBase
{
    private Vector3 _targetPosition;

    public MoveCommand(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }

    public override bool CanExecute(PlayerStateManager player)
      => !_isStarted;

    public override void ExecuteInternal(PlayerStateManager player)
    {
        if (player.IsInCarryingState())
            player.CarryingState.SwitchToMoveSubState(player, _targetPosition);
        else
            player.SwitchToMoveState(_targetPosition);        
    }

    public override bool IsComplete(PlayerStateManager player)
    {
        // Movement ist fertig wenn wir im Idle State sind und die Position erreicht haben
        if (!player.IsInCarryingState())
        {
            if (player.IsInIdleState())
                return true;
        }
        else if (player.IsInCarryingState())
        {
            if (player.CarryingState.IsInIdleState())
                return true;
        }

        return false;
    }
}