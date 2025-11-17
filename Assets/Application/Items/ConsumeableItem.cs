using UnityEngine;

[CreateAssetMenu(fileName = "ConsumeableItem", menuName = "ScriptableObjects/Items/ConsumeableItem")]
public class ConsumeableItem : Item
{
    [Header("Consumable Item Properties")]
    public float EnergyDrain;
    public float EnergyIncrease;

    public float HungerDrain;
    public float HungerIncrease;

    public void ConsumeItem()
    {
        float totalEnergyIncrease = EnergyIncrease - EnergyDrain;
        float totalHungerIncrease = HungerIncrease - HungerDrain;

        PlayerNeeds.Instance.ModifyNeed(NeedType.Energy, totalEnergyIncrease);
        PlayerNeeds.Instance.ModifyNeed(NeedType.Hunger, totalHungerIncrease);

        PlayerInventory.Instance.RemoveItem(this);
    }
}