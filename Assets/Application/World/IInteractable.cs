using System.Collections.Generic;

public interface IInteractable
{
    List<ContextMenuOption> GetContextMenuOptions(PlayerStateManager player);
    string GetDisplayName();
}