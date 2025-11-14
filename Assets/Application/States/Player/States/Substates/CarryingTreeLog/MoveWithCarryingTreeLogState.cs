using System.Collections;
using UnityEngine;

public class MoveWithCarryingTreeLogState : PlayerCarryingBaseSubState
{

    private Vector3 _targetPosition;

    private PlayerCarryingState _parentState;
    private PlayerStateManager _player;

    private Coroutine _drainEnergyCoroutine;

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
        
        if(!parentState.IsStateExecutesBlocked())
            _drainEnergyCoroutine = player.StartCoroutine(DrainEnergyWhileMovingCoroutine(parentState, player));
    }

    public override void UpdateState(PlayerCarryingState parentState, PlayerStateManager player)
    {
    }

    public override void ExitState(PlayerCarryingState parentState, PlayerStateManager player)
    {
        player.StopCoroutine(_drainEnergyCoroutine);
        
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

    private IEnumerator DrainEnergyWhileMovingCoroutine(PlayerCarryingState parentState, PlayerStateManager player)
    {
        while(!player.PlayerNeeds.IsNeedDepleted(NeedType.Energy))
        {
            player.PlayerNeeds.ModifyNeed(NeedType.Energy, -parentState.GetCarriedTreeLog().CarryEnergyDrain);
            yield return new WaitForSeconds(2f);
        }
    }
}