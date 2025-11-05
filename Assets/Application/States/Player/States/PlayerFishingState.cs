using System.Collections;
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

        player.StartCoroutine(FishingCoroutine(player));
    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (_fishingSpot.GetFishingCapacity() <= 0)
            player.SwitchState(player.IdleState);
    }

    public override void ExitState(PlayerStateManager player)
    {
    }

    private IEnumerator FishingCoroutine(PlayerStateManager player)
    {
        yield return new WaitForSeconds(1f);
    }
}