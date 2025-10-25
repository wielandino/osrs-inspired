using System;

public class ContextMenuOption
{
    public string DisplayText = string.Empty;
    public Action<PlayerStateManager> OnExecute;
    public Func<PlayerStateManager, bool> IsAvailable;


    public ContextMenuOption(string displayText,
                              Action<PlayerStateManager> onExecute,
                              Func<PlayerStateManager, bool> availabilityCheck)
    {
        DisplayText = displayText;
        OnExecute = onExecute;
        IsAvailable = availabilityCheck ?? (_ => true);
    }
}