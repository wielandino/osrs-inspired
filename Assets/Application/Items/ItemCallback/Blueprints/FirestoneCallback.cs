using UnityEngine;

[CreateAssetMenu(fileName = "FirestoneCallback", menuName = "Items/Callbacks/Tinderbox")]
public class FirestoneCallback : ItemCallback
{
    
    public override bool CanCreateCommand(GameObject target, PlayerStateManager player)
    {
        if (!target.TryGetComponent<TreeLog>(out var treeLog))
            return false;
            
        if (!ToolValidator.CanPlayerUseTool(player.PlayerInventory.SelectedItem, 
                                           SkillType.Firemaking, 
                                           player.PlayerSkills))
            return false;
            
        return true;
    }
    
    public override PlayerCommandBase CreateCommand(GameObject target, PlayerStateManager player)
    {
        var treeLog = target.GetComponent<TreeLog>();
        return new BurnTreeLogCommand(treeLog);
    }
}