using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class OSRSInventoryGridElement : MonoBehaviour, IInventoryGridElement, IPointerClickHandler
{
    private IInventoryItemData _itemData;
    private InventoryItemContextMenu _contextMenuPlugin;
    private InventoryItemSelect _selectItemPlugin;

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

        if(_contextMenuPlugin.GetContextMenuOptions().Count > 0)
        {
            var contextMenuItemOptions = _contextMenuPlugin.GetContextMenuOptions();

            foreach(var contextMenuItemOption in contextMenuItemOptions)
            {
                contextMenuOptions.Add(new(
                    displayText: contextMenuItemOption.Label,
                    onExecute: contextMenuItemOption.Callback
                ));
            }
        }

        return contextMenuOptions;
    }

    private void DebugContext()
    {
        Debug.Log($"Debug from ContextMenu");
    }
}