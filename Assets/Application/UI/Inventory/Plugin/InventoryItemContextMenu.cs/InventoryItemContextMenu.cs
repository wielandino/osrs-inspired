using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryItemContextMenu : MonoBehaviour, IInventoryPlugin
{
    private IInventoryGridElement _gridElement;
    private readonly List<InventoryItemContextMenuOptions> _contextMenuOptions = new();

    public void AddConfiguration(IInventoryGridElement gridElement)
    {
        Debug.Log($"Added {this.name} Plugin");
        
        _gridElement = gridElement;
    }

    public void AddContextOption(string label, Action callback, int priority = 0)
    {
        _contextMenuOptions.Add(new InventoryItemContextMenuOptions
        {
            Label = label,
            Callback = callback,
            Priority = priority
        });

        _contextMenuOptions.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }

    public void RemoveContextOption(InventoryItemContextMenuOptions contextMenuOption)
    {
        var contextMenuOptionInList = _contextMenuOptions.Where(x => x == contextMenuOption).FirstOrDefault();

        if (contextMenuOptionInList != null)
            _contextMenuOptions.Remove(contextMenuOptionInList);
    }

    public List<InventoryItemContextMenuOptions> GetContextMenuOptions()
        => _contextMenuOptions;

    public void ClearContextOptions()
    {
        _contextMenuOptions.Clear();
    }
}