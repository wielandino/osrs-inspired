using UnityEngine;

#nullable enable
public abstract class ItemCallback : ScriptableObject
{
    public PlayerCommandBase? ExecuteCallback(GameObject target, PlayerStateManager player, Item sourceItem)
    {
        PlayerCommandBase? command = null;

        if (CanCreateCommand(target, player, sourceItem))
            command = CreateCommand(target, player, sourceItem);


        player.PlayerInventory.DeSelectCurrentItem();
        
        return command;
    }

    public abstract PlayerCommandBase CreateCommand(GameObject target, PlayerStateManager player, Item sourceItem);
    public abstract bool CanCreateCommand(GameObject target, PlayerStateManager player, Item sourceItem);
}