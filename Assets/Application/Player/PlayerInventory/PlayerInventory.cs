using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<Item> _items = new();

    public Item DebugItem;

    public List<Item> GetItems() => _items;

    [SerializeField]
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
            AddItem(DebugItem);
    }

    public void AddItem(Item item)
    {
        _items.Add(item);

        InventoryUIController.Instance.AddGridElement(item);
    }

    public bool RemoveItem(Item item)
    {
        return _items.Remove(item);
    }
    
    private void OnItemSelectedFromUI(IInventoryItemData itemData)
    {
        if (itemData is Item item)
        {
            var itemInList = _items.Where(x => x == item).FirstOrDefault();

            if (itemInList == null)
                return;

            SelectedItem = itemInList;

            Debug.Log($"Item {item.ItemName} is currently in selected mode");
        }
    }

    private void OnItemRemovedFromUI(IInventoryItemData itemData)
    {
        if (itemData is Item item)
        {
            bool removed = _items.Remove(item);
            
            if (removed)
            {
                Debug.Log($"Item {item.ItemName} wurde aus Inventar entfernt");
            }
            else
            {
                Debug.LogWarning($"Item {item.ItemName} war nicht im Inventar!");
            }
        }
    }
}