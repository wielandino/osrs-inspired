using UnityEngine;


public abstract class ItemCallback : ScriptableObject
{
    public abstract PlayerCommandBase CreateCommand(GameObject target, PlayerStateManager player);
    public abstract bool CanCreateCommand(GameObject target, PlayerStateManager player);
}