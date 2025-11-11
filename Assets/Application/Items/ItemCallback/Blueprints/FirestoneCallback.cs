using UnityEngine;

[CreateAssetMenu(fileName = "FirestoneCallback", menuName = "Items/Callbacks/Tinderbox")]
public class FirestoneCallback : ItemCallback
{
    
    public override bool CanCreateCommand(GameObject target, PlayerStateManager player, Item sourceItem)
    {
        if (!target.TryGetComponent<TreeLog>(out var treeLog))
            return false;

        if (!ToolValidator.CanPlayerUseTool(player.PlayerInventory.SelectedItem,
                                           SkillType.Firemaking,
                                           player.PlayerSkills))
            return false;
        
        if (sourceItem is not ISkillTool tool)
            return false;

        if (!ToolValidator.CanToolBeUsedForSkill(sourceItem, SkillType.Woodcutting))
            return false;

        return true;
    }
    
    public override PlayerCommandBase CreateCommand(GameObject target, PlayerStateManager player, Item sourceItem)
    {
        var treeLog = target.GetComponent<TreeLog>();
        return new BurnTreeLogCommand(treeLog);
    }
}