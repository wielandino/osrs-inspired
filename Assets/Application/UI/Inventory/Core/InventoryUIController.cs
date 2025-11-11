using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance;

    public GameObject InventoryPanel;
    public GameObject GridContainer;
    public InventoryGridElement GridElementPrefab;

    private readonly List<InventoryGridElement> _gridElements = new();

    private bool _isPanelActive;

    public event Action<IInventoryItemData> OnGridElementRemoved;

    private void OnEnable()
    {
        Instance = this;
    }

    private void OnDisable()
    {
        Instance = null;
    }

    private void Start()
    {
        InventoryPanel.SetActive(false);

        if (InventoryPanel == null)
            Debug.LogWarning("InventoryUIController does not have a InventoryPanel GameObject");

        if (GridContainer == null)
            Debug.LogWarning("InventoryUIController does not have a GridContainer GameObject");

        if (GridElementPrefab == null)
            Debug.LogWarning("InventoryUIController does not have a GridElementPrefab GameObject");
    }

    public void AddGridElement(IInventoryItemData itemData)
    {
        GameObject gridElementObj = Instantiate(GridElementPrefab.gameObject, GridContainer.transform);

        if (!gridElementObj.TryGetComponent<InventoryGridElement>(out var component))
        {
            Debug.LogError("Missing InventoryGridElement from Core!");
            return;
        }

        IInventoryGridElement gridElement = gridElementObj.GetComponent<IInventoryGridElement>();
        
        if (gridElement == null)
        {
            Debug.LogError("Missing IInventoryGridElement Component. Implement one!");
            Destroy(gridElementObj);
            return;
        }

        gridElement.Initialize(itemData);
        _gridElements.Add(component);

        ConfigurePlugins(component, gridElement);
    }

    public void RemoveGridElement(InventoryGridElement gridElement)
    {
        var gridElementInList = _gridElements.Where(x => x == gridElement).FirstOrDefault();

        if (gridElementInList == null)
        {
            Debug.LogError("No GridElement of type 'InventoryGridElement' found in List");
            return;
        }

        IInventoryGridElement gridElementInterface = gridElementInList.GetComponent<IInventoryGridElement>();
        IInventoryItemData itemData = gridElementInterface?.GetItemData();

        Destroy(gridElementInList.gameObject);
        _gridElements.Remove(gridElementInList);

        OnGridElementRemoved?.Invoke(itemData);
    }

    private void ConfigurePlugins(InventoryGridElement gridElementObj, IInventoryGridElement gridElement)
    {
        IInventoryPlugin[] plugins = gridElementObj.gameObject.GetComponents<IInventoryPlugin>();

        foreach (var plugin in plugins)
            plugin.AddConfiguration(gridElement);
        
    }

    public void TriggerInventoryPanel()
    {
        InventoryPanel.SetActive(!_isPanelActive);
        _isPanelActive = !_isPanelActive;
    }

    public InventoryGridElement GetInventoryGridElementByItemData(IInventoryItemData inventoryItemData)
        => _gridElements.FirstOrDefault(x =>
                x.gameObject.GetComponent<IInventoryGridElement>().GetItemData() == inventoryItemData);
    
}