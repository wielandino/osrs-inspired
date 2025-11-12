using UnityEngine;

public class CookableItem : Item
{
    [Header("Cooking Skill Configuration")]
    public Item ReturnItem;
    public int RequiredCookingLevel;
    public float IncreasedFailureToCook;
    public float XPDrop;
}