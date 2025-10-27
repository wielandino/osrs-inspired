using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance;

    public GameObject InventoryPanel;
    public GameObject GridContainer;
    public InventoryGridElement GridElementPrefab;
    private List<InventoryGridElement> _gridElements = new();

    void OnEnable()
    {
        Instance = this;
    }

    void OnDisable()
    {
        Instance = null;
    }

    void Start()
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

        Destroy(gridElementInList.gameObject);
        _gridElements.Remove(gridElementInList);
    }

    private void ConfigurePlugins(InventoryGridElement gridElementObj, IInventoryGridElement gridElement)
    {
        IInventoryPlugin[] plugins = gridElementObj.gameObject.GetComponents<IInventoryPlugin>();

        foreach (var plugin in plugins)
            plugin.AddConfiguration(gridElement);
        
    }

    public void EnableInventoryPanel()
    {
        InventoryPanel.SetActive(true);
    }

    public void DisableInventoryPanel()
    {
        InventoryPanel.SetActive(false);
    }
    
}