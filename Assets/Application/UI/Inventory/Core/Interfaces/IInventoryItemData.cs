using UnityEngine;

public interface IInventoryItemData
{
    public Sprite IconSprite { get; }
    public string ItemName { get; }
    public string ItemDescription { get; }
}