using UnityEngine;
using UnityEngine.EventSystems;

public class OSRSInventoryGridElement : MonoBehaviour, IInventoryGridElement, IPointerClickHandler
{
    private IInventoryItemData _itemData;

    public void Initialize(IInventoryItemData itemData)
    {
        _itemData = itemData;
    }

    public IInventoryItemData GetItemData()
        => _itemData;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (gameObject.TryGetComponent<InventoryItemSelect>(out var selectPlugin))
                selectPlugin.AddSelectedItem();
        }
    }
}