using UnityEngine;

[CreateAssetMenu(fileName = "Fish", menuName = "ScriptableObjects/Items/ConsumeableItem")]
public class ConsumeableItem : Item
{
    [Header("Consumable Item Properties")]
    public int SomePropertie;
}