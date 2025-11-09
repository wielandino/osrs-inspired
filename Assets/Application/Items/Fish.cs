using UnityEngine;

[CreateAssetMenu(fileName = "Fish", menuName = "ScriptableObjects/Items/Fish")]
public class Fish : CookableItem
{
    [Header("Fishing Skill Configuration")]
    public int RequiredLevelToCatch;

    public float XPPerCatch;

    public float FishingSpotCapacityDrain;

    [Tooltip("Give this number in Percentage")]
    public float IncreasedFailureToCatch; 
}