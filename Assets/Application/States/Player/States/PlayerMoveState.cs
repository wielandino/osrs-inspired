using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    private Vector3 _targetPosition;
    private PlayerStateManager _player;

    public void SetTargetPosition(Vector3 position)
    {
        _targetPosition = position;
    }

    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entered PlayerMoveState");
        _player = player;

        if (player.PlayerMovementController == null)
        {
            Debug.LogError("PlayerMovementController is null! Make sure it's attached to the GameObject.");
            return;
        }

        player.PlayerMovementController.OnMovementCompleted += OnMovementCompleted;
        player.PlayerMovementController.OnMovementCancelled += OnMovementCancelled;

        player.PlayerMovementController.StartMovement(_targetPosition);
    }

    public override void UpdateState(PlayerStateManager player)
    {
    }

    public override void ExitState(PlayerStateManager player)
    {
        if (player.PlayerMovementController != null)
        {
            player.PlayerMovementController.OnMovementCompleted -= OnMovementCompleted;
            player.PlayerMovementController.OnMovementCancelled -= OnMovementCancelled;
        }
    }

    private void OnMovementCompleted()
    {
        _player.SwitchToIdleState();
    }

    private void OnMovementCancelled()
    {
        _player.SwitchToIdleState();
    }
}
