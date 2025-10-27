using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<Item> _items = new();

    public Item DebugItem;

    public List<Item> GetItems() => _items;

    public bool HasValidToolForSkill(SkillType skill, PlayerSkills playerSkills)
        => ToolValidator.GetBestToolForSkill(_items, skill, playerSkills) != null;
    

    public ISkillTool GetBestToolForSkill(SkillType skill, PlayerSkills playerSkills)
        => ToolValidator.GetBestToolForSkill(_items, skill, playerSkills);

    void Update()
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
}