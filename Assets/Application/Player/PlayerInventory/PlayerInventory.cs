using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<Item> _items = new();

    public List<Item> GetItems() => _items;

    public bool HasValidToolForSkill(SkillType skill, PlayerSkills playerSkills)
        => ToolValidator.GetBestToolForSkill(_items, skill, playerSkills) != null;
    

    public ISkillTool GetBestToolForSkill(SkillType skill, PlayerSkills playerSkills)
        => ToolValidator.GetBestToolForSkill(_items, skill, playerSkills);

    public void AddItem(Item item)
    {
        _items.Add(item);
    }

    public bool RemoveItem(Item item)
    {
        return _items.Remove(item);
    }
}