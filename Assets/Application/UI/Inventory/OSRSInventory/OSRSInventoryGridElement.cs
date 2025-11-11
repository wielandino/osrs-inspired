using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class OSRSInventoryGridElement : InventoryGridElement, IInventoryGridElement, IPointerClickHandler
{
    private IInventoryItemData _itemData;
    private InventoryItemContextMenu _contextMenuPlugin;
    private InventoryItemSelect _selectItemPlugin;
    
    public static Action<IInventoryItemData, IInventoryItemData> OnItemCombined;

    public void Initialize(IInventoryItemData itemData)
    {
        _itemData = itemData;

        _contextMenuPlugin = gameObject.GetComponent<InventoryItemContextMenu>();
        _selectItemPlugin = gameObject.GetComponent<InventoryItemSelect>();

        SetupContextMenu();
    }

    public IInventoryItemData GetItemData()
        => _itemData;

    private void SetupContextMenu()
    {
        if (_contextMenuPlugin == null)
            return;

        _contextMenuPlugin.ClearContextOptions();
        _contextMenuPlugin.AddContextOption("Select", _selectItemPlugin.AddSelectedItem, priority: 100);
        _contextMenuPlugin.AddContextOption("Destroy", DestroyItem, priority: 90);

        if (_selectItemPlugin.GetSelectedItems().Count > 0)
            _contextMenuPlugin.AddContextOption("Combine", CombineSelectedItems, priority: 80);
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (_selectItemPlugin == null)
                return;

            _selectItemPlugin.AddSelectedItem();

        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (_contextMenuPlugin == null)
                return;

            var mousePosition = Mouse.current.position.ReadValue();

            List<ContextMenuOption> contextMenuOptions = ConvertItemContextMenuOptions();
            ContextMenuPanel.Instance.ShowContextMenuForObject(contextMenuOptions, mousePosition);
        }
    }

    private List<ContextMenuOption> ConvertItemContextMenuOptions()
    {
        var contextMenuOptions = new List<ContextMenuOption>();

        if (_contextMenuPlugin.GetContextMenuOptions().Count > 0)
        {
            var contextMenuItemOptions = _contextMenuPlugin.GetContextMenuOptions();

            foreach (var contextMenuItemOption in contextMenuItemOptions)
            {
                contextMenuOptions.Add(new(
                    displayText: contextMenuItemOption.Label,
                    onExecute: contextMenuItemOption.Callback
                ));
            }
        }

        return contextMenuOptions;
    }
    
    private void DestroyItem()
    {
        _inventoryUIController.RemoveGridElement(_gridElement);
    }

    private void CombineSelectedItems()
    {
        var primaryItem = GetItemData();
        var secondaryItem = _selectItemPlugin.GetSelectedItems().First().GetItemData();

        OnItemCombined?.Invoke(primaryItem, secondaryItem);
    }

    private void OnDisable()
    {
        _selectItemPlugin.ClearAllSelectedItems();
    }
}