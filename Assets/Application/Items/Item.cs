using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Items/Normal Item")]
public class Item : ScriptableObject
{
    [Header("Basic Item Info")]
    public GameObject Prefab;
    public string Name;
    [TextArea(3, 5)]
    public string Description;
    public bool IsStackable;
    public Sprite Icon;

    [Header("Item Properties")]
    public ItemType ItemType;
}