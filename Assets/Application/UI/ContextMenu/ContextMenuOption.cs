using System;

public class ContextMenuOption
{
    public string DisplayText = string.Empty;
    public string Label = string.Empty;
    public Action OnExecute;

    public ContextMenuOption(string displayText,
                             Action onExecute,
                             string label)
    {
        DisplayText = displayText;
        OnExecute = onExecute;
        Label = label;
    }
}