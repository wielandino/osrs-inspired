using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] 
    private List<Item> _items = new();

    public List<Item> GetItems() => _items;

    public Item SelectedItem;

    public bool HasValidToolForSkill(SkillType skill, PlayerSkill playerSkills)
        => ToolValidator.GetBestToolForSkill(_items, skill, playerSkills) != null;
    

    public ISkillTool GetBestToolForSkill(SkillType skill, PlayerSkill playerSkills)
        => ToolValidator.GetBestToolForSkill(_items, skill, playerSkills);

    private void OnEnable()
    {
        if (InventoryUIController.Instance != null)
            InventoryUIController.Instance.OnGridElementRemoved += OnItemRemovedFromUI;

        InventoryItemSelect.OnItemSelected += OnItemSelectedFromUI;
        OSRSInventoryGridElement.OnItemCombined += OnItemCombinedFromUI;
    }

    private void Update()
    {
        //DEBUG!
        if (Input.GetKeyDown(KeyCode.Q))
            AddAllItemsToUI();
    }

    // Only for debug!
    private void AddAllItemsToUI()
    {
        foreach (var item in _items)
            InventoryUIController.Instance.AddGridElement(item);
    }

    public void AddItem(Item item)
    {
        _items.Add(item);

        if (InventoryUIController.Instance != null)
            InventoryUIController.Instance.AddGridElement(item);
    }

    public bool RemoveItem(Item item)
    {
        var isItemRemoved = _items.Remove(item);

        if (isItemRemoved)
        {
            var gridElementToRemove = InventoryUIController.Instance.GetInventoryGridElementByItemData(item);
            
            if (gridElementToRemove != null)
                InventoryUIController.Instance.RemoveGridElement(gridElementToRemove);
        }

        return isItemRemoved;
    }


    private void OnItemCombinedFromUI(IInventoryItemData primaryItem, IInventoryItemData secondaryItem)
    {
        if (!TryGetItemFromInventory(primaryItem, out Item primaryItemInInventory))
            return;
        

        if (!TryGetItemFromInventory(secondaryItem, out Item secondaryItemInInventory))
            return;
        

        if (!TryGetPlayerStateManager(out PlayerStateManager playerStateManager))
            return;
        
        var craftingCommand = new CraftingCommand(primaryItemInInventory, secondaryItemInInventory);
        playerStateManager.AddCommands(craftingCommand);
    }

    private bool TryGetItemFromInventory(IInventoryItemData itemData, out Item foundItem)
    {
        foundItem = null;

        if (itemData == null)
            return false;

        if (itemData is not Item itemCasted)
            return false;

        foundItem = _items.FirstOrDefault(x => x == itemCasted);
        return foundItem != null;
    }

    private bool TryGetPlayerStateManager(out PlayerStateManager stateManager)
    {
        stateManager = null;

        if (Player.Instance == null)
            return false;

        stateManager = Player.Instance.GetPlayerStateManager();
        return stateManager != null;
    }

    private bool IsItemFromUIInInventory(IInventoryItemData itemData)
    {
        if (itemData is not Item)
            return false;

        var itemInList = _items.Where(x => x == itemData as Item).FirstOrDefault();

        if (itemInList == null)
            return false;

        return true;
    }

    private void OnItemSelectedFromUI(IInventoryItemData itemData)
    {
        if (!IsItemFromUIInInventory(itemData))
            return;

        var itemInList = _items.Where(x => x == itemData as Item).First();
        SelectedItem = itemInList;
    }

    private void OnItemRemovedFromUI(IInventoryItemData itemData)
    {
        if (itemData is Item item && _items.Where(x => x == item) != null)
            _items.Remove(item);
    }

    public void DeSelectCurrentItem()
    {
        if (SelectedItem != null)
            SelectedItem = null;
    }

    
}