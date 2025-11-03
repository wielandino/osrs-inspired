using UnityEngine;

public class MoveWithCarryingTreeLogState : PlayerCarryingBaseSubState
{

    private Vector3 _targetPosition;

    private PlayerCarryingState _parentState;
    private PlayerStateManager _player;

    public void SetTargetPosition(Vector3 TargetPosition)
    {
        _targetPosition = TargetPosition;
    }

    public override void EnterState(PlayerCarryingState parentState, PlayerStateManager player)
    {
        _parentState = parentState;
        _player = player;

        Debug.Log("Entered Substate MoveWithCarryingTreeLog from PlayerCarryingState");

        player.PlayerMovementController.OnMovementCompleted += OnMovementCompleted;
        player.PlayerMovementController.OnMovementCancelled += OnMovementCancelled;

        player.PlayerMovementController.StartMovement(_targetPosition);
    }

    public override void UpdateState(PlayerCarryingState parentState, PlayerStateManager player)
    {
    }

    public override void ExitState(PlayerCarryingState parentState, PlayerStateManager player)
    {
        player.PlayerMovementController.OnMovementCompleted -= OnMovementCompleted;
        player.PlayerMovementController.OnMovementCancelled -= OnMovementCancelled;

        _targetPosition = Vector3.zero;
    }    
    
    private void OnMovementCompleted()
    {
        _parentState.SwitchToIdleSubState(_player);
    }

    private void OnMovementCancelled()
    {
        _parentState.SwitchToIdleSubState(_player);
    }
}