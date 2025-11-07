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
        => _items.Remove(item);
    
    
    private void OnItemSelectedFromUI(IInventoryItemData itemData)
    {
        if (itemData is Item item)
        {
            var itemInList = _items.Where(x => x == item).FirstOrDefault();

            if (itemInList == null)
                return;

            SelectedItem = itemInList;
        }
    }

    private void OnItemRemovedFromUI(IInventoryItemData itemData)
    {
        if (itemData is Item item)
            _items.Remove(item);
    }
}