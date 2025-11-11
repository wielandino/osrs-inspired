using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipeDatabase", menuName = "ScriptableObjects/Crafting/Recipe Database")]
public class CraftingRecipeDatabase : ScriptableObject
{
    [SerializeField] private List<CraftingRecipe> _allRecipes = new();
    
    private static CraftingRecipeDatabase _instance;
    public static CraftingRecipeDatabase Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<CraftingRecipeDatabase>("CraftingRecipeDatabase");
            return _instance;
        }
    }
    
    public CraftingRecipe FindRecipe(Item itemOne, Item itemTwo)
    {
        return _allRecipes.FirstOrDefault(recipe =>
            (recipe.PrimaryIngredient.ItemToCombine == itemOne && recipe.SecondaryIngredient.ItemToCombine == itemTwo) ||
            (recipe.PrimaryIngredient.ItemToCombine == itemTwo && recipe.SecondaryIngredient.ItemToCombine == itemOne)
        );
    }
    
    public List<CraftingRecipe> FindRecipesUsingItem(Item item)
    {
        return _allRecipes.Where(recipe =>
            recipe.PrimaryIngredient.ItemToCombine == item ||
            recipe.SecondaryIngredient.ItemToCombine == item
        ).ToList();
    }
    
    public List<CraftingRecipe> GetAvailableRecipes(PlayerStateManager player)
    {
        int playerLevel = player.PlayerSkills.GetCraftingSkill().CurrentLevel;
        
        return _allRecipes.Where(recipe => 
            recipe.RequiredLevel <= playerLevel
        ).ToList();
    }

    public bool PlayerHasRequiredItems(CraftingRecipe recipe, PlayerStateManager player)
    {
        var inventory = player.PlayerInventory.GetItems();
        
        bool hasItemOne = inventory.Contains(recipe.PrimaryIngredient.ItemToCombine);
        bool hasItemTwo = inventory.Contains(recipe.SecondaryIngredient.ItemToCombine);
        
        return hasItemOne && hasItemTwo;
    }
}