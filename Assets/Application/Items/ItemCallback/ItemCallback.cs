using UnityEngine;

#nullable enable
public abstract class ItemCallback : ScriptableObject
{
    public PlayerCommandBase? ExecuteCallback(GameObject target, PlayerStateManager player)
    {
        PlayerCommandBase? command = null;

        if (CanCreateCommand(target, player))
            command = CreateCommand(target, player);


        player.PlayerInventory.SelectedItem = null;
        
        return command;
    }

    public abstract PlayerCommandBase CreateCommand(GameObject target, PlayerStateManager player);
    public abstract bool CanCreateCommand(GameObject target, PlayerStateManager player);
}