using System.Collections;
using UnityEngine;

public class PlayerFishingState : PlayerBaseState
{
    private FishingSpot _fishingSpot;

    private Coroutine _fishingCoroutine;

    public void SetFishingSpot(FishingSpot fishingSpot)
    {
        _fishingSpot = fishingSpot;
    }


    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entered PlayerFishingState");

        var fishingRod = GetFishingRod(player);

        if (fishingRod == null)
        {
            player.SwitchState(player.IdleState);
            return;
        }

        _fishingCoroutine = player.StartCoroutine(FishingCoroutine(player, fishingRod));
    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (_fishingSpot.GetFishingCapacity() <= 0)
            player.SwitchState(player.IdleState);
    }

    public override void ExitState(PlayerStateManager player)
    {
        player.StopCoroutine(_fishingCoroutine);
    }

    private IEnumerator FishingCoroutine(PlayerStateManager player, ISkillTool tool)
    {
        while (_fishingSpot.GetFishingCapacity() > 0)
        {
            var fishToCatch = SelectRandomFish();

            TryFishingAttempt(player, tool, fishToCatch, out bool spotIsEmpty);

            if (spotIsEmpty)
                break;

            yield return new WaitForSeconds(3f);
        }
    }

    public ISkillTool GetFishingRod(PlayerStateManager player)
    {
        if (player.PlayerInventory.SelectedItem != null)
        {
            if (!ToolValidator.CanPlayerUseTool(player.PlayerInventory.SelectedItem,
                                                SkillType.Fishing,
                                                player.PlayerSkills))
                return null;

            return player.PlayerInventory.SelectedItem as ISkillTool;
        }
        else
        {
            return player.PlayerInventory.GetBestToolForSkill(SkillType.Fishing, player.PlayerSkills);
        }
    }

    public bool TryFishingAttempt(PlayerStateManager player, ISkillTool tool, Fish fish, out bool spotIsEmpty)
    {
        spotIsEmpty = _fishingSpot.GetFishingCapacity() <= 0;

        if (spotIsEmpty)
            return false;

        float playerCatchChance = player.PlayerSkills.GetFishingSkill().GetChanceToCatchFishPerLevel();

        float finalCatchChance = CalculateCatchChance(playerCatchChance,
                                                      fish.IncreasedFailureToCatch,
                                                      tool.EfficiencyBonus);

        float roll = Random.Range(0f, 1f);
        bool success = roll < finalCatchChance;

        if (success)
        {
            player.PlayerInventory.AddItem(fish);
            _fishingSpot.ReduceFishingCapacity(fish.FishingSpotCapacityDrain);
        }

        return success;
    }

    public Fish SelectRandomFish()
    {
        var listOfPossibleFishes = _fishingSpot.GetListOfPossibleFishesToCatch();
        int fishIndex = Random.Range(0, listOfPossibleFishes.Count);
        return listOfPossibleFishes[fishIndex];
    }

     public float CalculateCatchChance(float baseChance, float fishDifficulty, float toolBonus)
        => Mathf.Clamp01(baseChance - fishDifficulty + toolBonus);
    
}