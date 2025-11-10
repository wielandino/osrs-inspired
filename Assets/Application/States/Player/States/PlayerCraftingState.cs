using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCraftingState : PlayerBaseState
{
    private CraftingRecipe _craftingRecipe;

    private List<Item> _itemsCombineOneOfSameType = new();
    private List<Item> _itemsCombineTwoOfSameType = new();

    private Coroutine _craftingCoroutine;

    public void SetCraftingRecipe(CraftingRecipe craftingRecipe, PlayerStateManager player)
    {
        _craftingRecipe = craftingRecipe;

        CollectAllCraftingItemsFromPlayerInventory(player);
    }

    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entered PlayerCraftingState");

        _craftingCoroutine = player.StartCoroutine(CraftingCoroutine(player));
    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (!CanContinueCrafting())
            player.SwitchToIdleState();
    }

    public override void ExitState(PlayerStateManager player)
    {
        Debug.Log("Exit PlayerCraftingState");

        player.StopCoroutine(_craftingCoroutine);
        _itemsCombineOneOfSameType.Clear();
        _itemsCombineTwoOfSameType.Clear();
    }

    private bool CanContinueCrafting()
    {
        bool hasItemOne = _itemsCombineOneOfSameType.Count > 0;
        bool hasItemTwo = _itemsCombineTwoOfSameType.Count > 0;
        
        return hasItemOne && hasItemTwo;
    }

    public void CollectAllCraftingItemsFromPlayerInventory(PlayerStateManager player)
    {
        _itemsCombineOneOfSameType = player.PlayerInventory.GetItems()
            .Where(item => item == _craftingRecipe.PrimaryIngredient.ItemToCombine)
            .ToList();

        _itemsCombineTwoOfSameType = player.PlayerInventory.GetItems()
            .Where(item => item == _craftingRecipe.SecondaryIngredient.ItemToCombine)
            .ToList();
    }

    private IEnumerator CraftingCoroutine(PlayerStateManager player)
    {
        while (CanContinueCrafting())
        {
            Item currentCombineOneItem = _itemsCombineOneOfSameType[0];
            Item currentCombineTwoItem = _itemsCombineTwoOfSameType[0];

            yield return new WaitForSeconds(3f);

            bool success = TryCraftAttempt(player);

            // Handle items based on success/failure
            HandleItemAfterCraft(player, currentCombineOneItem, _itemsCombineOneOfSameType,
                                _craftingRecipe.PrimaryIngredient, success);
            HandleItemAfterCraft(player, currentCombineTwoItem, _itemsCombineTwoOfSameType,
                                _craftingRecipe.SecondaryIngredient, success);

            // Add crafted item and XP on success
            if (success)
            {
                if (_craftingRecipe.ReturnItem != null)
                    player.PlayerInventory.AddItem(_craftingRecipe.ReturnItem);

                //player.PlayerSkills.GetCraftingSkill().AddExperience(_craftingRecipe.XPDrop);
            }
        }
    }
    
    private void HandleItemAfterCraft(PlayerStateManager player, Item item, List<Item> itemList,
                                      CraftingRecipeItemInfo recipeInfo, bool craftSuccess)
    {
        bool shouldDelete = craftSuccess 
            ? recipeInfo.ShouldDeleteItemAfterCraft 
            : recipeInfo.ShouldDeleteItemAfterFail;
        
        if (shouldDelete)
        {
            itemList.RemoveAt(0);
            player.PlayerInventory.RemoveItem(item);
        }
    }

    public bool TryCraftAttempt(PlayerStateManager player)
    {
        float playerCraftChance = player.PlayerSkills.GetCraftingSkill().GetChanceToCraftPerLevel();
        float finalCraftChance = CalculateCraftChance(playerCraftChance, _craftingRecipe.IncreasedFailureChance);
        float roll = Random.Range(0f, 1f);
        
        return roll < finalCraftChance;
    }
    
    public float CalculateCraftChance(float baseChance, float itemDifficulty)
    => Mathf.Clamp01(baseChance - itemDifficulty);

}