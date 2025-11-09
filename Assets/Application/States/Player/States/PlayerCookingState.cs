using UnityEngine;

public class PlayerCookingState : PlayerBaseState
{
    private TreeLog _targetTreeLog;
    private CookableItem _itemToCook;

    private Coroutine _cookingCoroutine;

    public void SetTargetTreeLogAndItemToCook(TreeLog treelog, CookableItem item)
    {
        _targetTreeLog = treelog;
        _itemToCook = item;
    }

    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entered PlayerCookingState");
    }

    public override void UpdateState(PlayerStateManager player)
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState(PlayerStateManager player)
    {
        Debug.Log("Exit PlayerCookingState");
    }

    
}