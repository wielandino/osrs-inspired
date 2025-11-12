using UnityEngine;

[CreateAssetMenu(fileName = "FishingRodItemCallback", menuName = "Items/Callbacks/FishingRodItem")]
public class FishingRodItemCallback : ItemCallback
{
    public override bool CanCreateCommand(GameObject target, PlayerStateManager player, Item sourceItem)
    {
        if (!target.TryGetComponent<FishingSpot>(out var fishingSpot))
            return false;

        if (fishingSpot.GetFishingCapacity() <= 0)
            return false;

        if (sourceItem is not ISkillTool tool)
            return false;

        if (!ToolValidator.CanToolBeUsedForSkill(sourceItem, SkillType.Fishing))
            return false;

        return true;
    }

    public override PlayerCommandBase CreateCommand(GameObject target, PlayerStateManager player, Item sourceItem)
    {
        var fishingSpot = target.GetComponent<FishingSpot>();
        return new FishingCommand(fishingSpot, sourceItem as ISkillTool);
    }
}