using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Items/Normal Item")]
public class Item : ScriptableObject, IInventoryItemData
{
    [Header("Basic Item Info")]
    public GameObject Prefab;
    public string Name;
    [TextArea(3, 5)]
    public string Description;
    public bool IsStackable;
    public Sprite Icon;

    [Header("Item Interaction")]
    public ItemCallback Callback;

    [Header("Item Properties")]
    public ItemType ItemType;

    public string ItemName => Name;
    public string ItemDescription => Description;
    public Sprite IconSprite => Icon;
}