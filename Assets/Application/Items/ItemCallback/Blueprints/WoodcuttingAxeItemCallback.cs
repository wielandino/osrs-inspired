using UnityEngine;

[CreateAssetMenu(fileName = "WoodcuttingAxeItemCallback", menuName = "Items/Callbacks/WoodcuttingAxeItem")]
public class WoodcuttingAxeItemCallback : ItemCallback
{
    public override bool CanCreateCommand(GameObject target, PlayerStateManager player, Item sourceItem)
    {
        if (!target.TryGetComponent<Tree>(out var tree))
            return false;

        if (!tree.GetStateManager().IsInIdleState())
            return false;

        if (sourceItem is not ISkillTool tool)
            return false;

        if (!ToolValidator.CanToolBeUsedForSkill(sourceItem, SkillType.Woodcutting))
            return false;

        return true;
    }

    public override PlayerCommandBase CreateCommand(GameObject target, PlayerStateManager player, Item sourceItem)
    {
        var tree = target.GetComponent<Tree>();
        return new WoodcuttingCommand(tree, sourceItem as ISkillTool);
    }
}