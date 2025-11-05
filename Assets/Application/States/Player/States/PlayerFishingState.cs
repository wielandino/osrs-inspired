using UnityEngine;

public class PlayerFishingState : PlayerBaseState
{
    private FishingSpot _fishingSpot;

    public void SetFishingSpot(FishingSpot fishingSpot)
    {
        _fishingSpot = fishingSpot;
    }


    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entered PlayerFishingState");
    }

    public override void UpdateState(PlayerStateManager player)
    {
        // TODO: Player Fishing animation
    }

    public override void ExitState(PlayerStateManager player)
    {
    }
}