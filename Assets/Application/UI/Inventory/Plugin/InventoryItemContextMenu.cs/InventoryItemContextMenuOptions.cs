using System;

[Serializable]
public class InventoryItemContextMenuOptions
{
    public string Label;
    public Action Callback;
    public int Priority;
}