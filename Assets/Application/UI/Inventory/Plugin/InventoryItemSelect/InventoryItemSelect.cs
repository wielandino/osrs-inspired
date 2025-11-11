using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemSelect : MonoBehaviour, IInventoryPlugin
{
    public static Action<IInventoryItemData> OnItemSelected;
    public static Action OnItemSelectedCleared;
    private static InventoryItemSelectContainer _container;

    private static InventoryItemSelectContainer Container
    {
        get
        {
            _container ??= new InventoryItemSelectContainer();
            
            return _container;
        }
    }

    private IInventoryGridElement _gridElement;

    public void AddSelectedItem()
    {
        Container.AddSelectedItem(_gridElement);
        OnItemSelected?.Invoke(_gridElement.GetItemData());
        
        Debug.Log($"Item selected: {_gridElement.GetItemData().ItemName}");
    }

    public void AddConfiguration(IInventoryGridElement gridElement)
    {
        Debug.Log($"Added InventoryPlugin {this.name}");

        _gridElement = gridElement;
    }

    public void ClearAllSelectedItems()
    {
        Container.ClearAllSelectedItems();
        OnItemSelectedCleared?.Invoke();
    }

    public List<IInventoryGridElement> GetSelectedItems()
        => Container.GetSelectedItems();
}