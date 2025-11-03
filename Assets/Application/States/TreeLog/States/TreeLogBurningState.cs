using UnityEngine;

public class TreeLogBurningState : TreeLogBaseState
{
    private float _burnTime;
    private const float MAX_BURN_TIME = 30f; // 30 Sekunden brennen

    public override void EnterState(TreeLogStateManager treeLog)
    {
        Debug.Log($"TreeLog {treeLog.name} entered TreeLogBurning State");

        treeLog.AttachedTreeLog.TreeLogIdleStateObject.SetActive(false);
        treeLog.AttachedTreeLog.TreeLogBurningStateObject.SetActive(true);
        
        _burnTime = MAX_BURN_TIME;
    }

    public override void UpdateState(TreeLogStateManager treeLog)
    {
        _burnTime -= Time.deltaTime;
        
        if (_burnTime <= 0)
        { 
            Object.Destroy(treeLog.gameObject);
        }
    }

    public override void ExitState(TreeLogStateManager treeLog)
    {
        
    }

    public override void OnInteract(TreeLogStateManager treeLog, PlayerStateManager player)
    {
        
    }
}