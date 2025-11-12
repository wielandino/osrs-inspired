using UnityEngine;

[CreateAssetMenu(fileName = "CookableItemCallback", menuName = "Items/Callbacks/CookableItem")]
public class CookableItemCallback : ItemCallback
{
    public override bool CanCreateCommand(GameObject target, PlayerStateManager player, Item sourceItem)
    {
        if (!target.TryGetComponent<TreeLog>(out var treeLog))
            return false;

        if (!treeLog.GetStateManager().IsInBurningState())
            return false;

        if (sourceItem is not CookableItem)
            return false;

        return true;
    }

    public override PlayerCommandBase CreateCommand(GameObject target, PlayerStateManager player, Item sourceItem)
        => new CookingCommand(target.GetComponent<TreeLog>(), sourceItem as CookableItem);
}