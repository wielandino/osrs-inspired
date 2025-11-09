using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCookingState : PlayerBaseState
{
    private TreeLog _targetTreeLog;
    private CookableItem _itemToCook;

    private List<CookableItem> _itemsToCookOfSameType = new();

    private Coroutine _cookingCoroutine;

    public void SetTargetTreeLogAndItemToCook(TreeLog treelog, CookableItem item, PlayerStateManager player)
    {
        _targetTreeLog = treelog;
        _itemToCook = item;

        CollectAllCookableItemsFromPlayerInventory(player);
    }

    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entered PlayerCookingState");

        
        _cookingCoroutine = player.StartCoroutine(CookingCoroutine(player));
    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (!_targetTreeLog.GetStateManager().IsInBurningState() || _itemsToCookOfSameType.Count <= 0)
            player.SwitchToIdleState();
    }

    public override void ExitState(PlayerStateManager player)
    {
        Debug.Log("Exit PlayerCookingState");

        player.StopCoroutine(_cookingCoroutine);
        _itemsToCookOfSameType.Clear();
    }

    public void CollectAllCookableItemsFromPlayerInventory(PlayerStateManager player)
    {
        _itemsToCookOfSameType = player.PlayerInventory.GetItems()
            .OfType<CookableItem>()
            .Where(item => item == _itemToCook)
            .ToList();
    }

    private IEnumerator CookingCoroutine(PlayerStateManager player)
    {
        while (_itemsToCookOfSameType.Count > 0 && _targetTreeLog.GetStateManager().IsInBurningState())
        {
            CookableItem currentItem = _itemsToCookOfSameType[0];
            
            yield return new WaitForSeconds(3f);
            
            bool success = TryCookingAttempt(player, currentItem);
            
            _itemsToCookOfSameType.RemoveAt(0);
            player.PlayerInventory.RemoveItem(currentItem);
            
            if (success && currentItem.ReturnItem != null)
                player.PlayerInventory.AddItem(currentItem.ReturnItem);
        }
    }

    public bool TryCookingAttempt(PlayerStateManager player, CookableItem item)
    {
        float playerCookChance = player.PlayerSkills.GetCookingSkill().GetChanceToCookPerLevel();
        float finalCookChance = CalculateCookChance(playerCookChance, item.IncreasedFailureToCook);
        float roll = Random.Range(0f, 1f);
        
        return roll < finalCookChance;
    }
    
    public float CalculateCookChance(float baseChance, float itemDifficulty)
    => Mathf.Clamp01(baseChance - itemDifficulty);

}