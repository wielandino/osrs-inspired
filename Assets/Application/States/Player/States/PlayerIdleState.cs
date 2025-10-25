using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Started Player Idle State");
    }

    public override void ExitState(PlayerStateManager player)
    {
    }

    public override void UpdateState(PlayerStateManager player)
    {

        //player.InputHandler.HandleMouseClick();
    }
}
