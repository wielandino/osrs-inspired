using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "ScriptableObjects/Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [Header("Recipe Information")]
    public CraftingRecipeItemInfo PrimaryIngredient = new();    
    public CraftingRecipeItemInfo SecondaryIngredient = new();
    public Item ReturnItem;

    [Header("Crafting Skill Configuration")]
    public int RequiredLevel;
    public int XPDrop;
    public float IncreasedFailureChance;

}