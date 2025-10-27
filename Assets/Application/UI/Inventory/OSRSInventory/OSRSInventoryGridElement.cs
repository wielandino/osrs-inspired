using UnityEngine;

public class OSRSInventoryGridElement : MonoBehaviour, IInventoryGridElement
{
    private IInventoryItemData _itemData;

    public void Initialize(IInventoryItemData itemData)
    {
        _itemData = itemData;
    }

    public IInventoryItemData GetItemData()
        => _itemData;
}