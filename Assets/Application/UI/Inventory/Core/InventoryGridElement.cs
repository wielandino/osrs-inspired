using UnityEngine;

public class InventoryGridElement : MonoBehaviour
{
    protected InventoryUIController _inventoryUIController;
    protected InventoryGridElement _gridElement;

    private void Start()
    {
        _gridElement = gameObject.GetComponent<InventoryGridElement>();
        _inventoryUIController = InventoryUIController.Instance;
    }
}