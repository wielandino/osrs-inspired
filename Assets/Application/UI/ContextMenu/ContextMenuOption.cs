using System;

public class ContextMenuOption
{
    public string DisplayText = string.Empty;
    public Action OnExecute;


    public ContextMenuOption(string displayText,
                              Action onExecute)
    {
        DisplayText = displayText;
        OnExecute = onExecute;
    }
}