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

        ISkillTool fishingRod = null;

        if (player.PlayerInventory.SelectedItem != null)
        {
            if (!ToolValidator.CanPlayerUseTool(player.PlayerInventory.SelectedItem,
                                                SkillType.Fishing,
                                                player.PlayerSkills))
                return;

            fishingRod = player.PlayerInventory.SelectedItem as ISkillTool;
        }
        else
        {
            fishingRod = player.PlayerInventory.GetBestToolForSkill(SkillType.Fishing, player.PlayerSkills);
        }

        if (fishingRod == null)
        {
            player.SwitchState(player.IdleState);
            return;
        }

        player.StartCoroutine(FishingCoroutine(player, fishingRod));
    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (_fishingSpot.GetFishingCapacity() <= 0)
            player.SwitchState(player.IdleState);
    }

    public override void ExitState(PlayerStateManager player)
    {
    }

    private IEnumerator FishingCoroutine(PlayerStateManager player, ISkillTool tool)
    {
        var listOfPossibleFishes = _fishingSpot.GetListOfPossibleFishesToCatch();
        int numberOfFishes = listOfPossibleFishes.Count;
        float playerCatchChance = player.PlayerSkills.GetFishingSkill()
                                        .GetChanceToCatchFishPerLevel();

        while (_fishingSpot.GetFishingCapacity() > 0)
        {
            int fishIndexToCatch = Random.Range(0, numberOfFishes);
            var fishToCatch = listOfPossibleFishes[fishIndexToCatch];
            float playerCatchChanceValue =
                Mathf.Clamp01(playerCatchChance - fishToCatch.IncreasedFailureToCatch + tool.EfficiencyBonus);

            float calculatedCatchValue = Random.Range(0f, 1f);

            if (calculatedCatchValue < playerCatchChanceValue)
            {
                player.PlayerInventory.AddItem(fishToCatch);
                _fishingSpot.ReduceFishingCapacity(fishToCatch.FishingSpotCapacityDrain);
            }
        }

        yield return new WaitForSeconds(3f);
    }
}