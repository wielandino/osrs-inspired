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

        return true;
    }

    public override PlayerCommandBase CreateCommand(GameObject target, PlayerStateManager player, Item sourceItem)
    {
        throw new System.NotImplementedException();
    }
}