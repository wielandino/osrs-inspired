using UnityEngine;
using UnityEngine.EventSystems;

public class IdleWithCarryingTreeLogState : PlayerCarryingBaseSubState
{
    public override void EnterState(PlayerCarryingState parentState, PlayerStateManager player)
    {
        Debug.Log("Entered Substate IdleWithCarryingTreeLogState from PlayerCarryingState");
    }

    public override void UpdateState(PlayerCarryingState parentState, PlayerStateManager player)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (!Input.GetMouseButtonDown(0))
            return;


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent<Tile>(out var tile))
            {
                var moveWithCarryingTreeLogCommand = new MoveCommand(hit.transform.position);
                player.AddCommands(moveWithCarryingTreeLogCommand);
            }
        }
    }

    public override void ExitState(PlayerCarryingState parentState, PlayerStateManager player)
    {
    }
}