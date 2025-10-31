using System;
using UnityEngine;

public class InventoryItemSelect : MonoBehaviour, IInventoryPlugin
{
    public static Action<IInventoryItemData> OnItemSelected;
    private static InventoryItemSelectContainer _container;

    private static InventoryItemSelectContainer Container
    {
        get
        {
            if (_container == null)
            {
                Debug.Log($"⚠️ CONTAINER WIRD ERSTELLT!");
                _container = new InventoryItemSelectContainer();
            }
            else
            {
                Debug.Log($"✓ Container existiert bereits, wird wiederverwendet");
            }
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
    }
}