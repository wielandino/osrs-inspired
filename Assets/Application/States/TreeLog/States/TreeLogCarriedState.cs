using UnityEngine;

public class TreeLogCarriedState : TreeLogBaseState
{
    public override void EnterState(TreeLogStateManager treeLog)
    {
        Debug.Log($"TreeLog is now being carried");

        DespawnController.Instance.RemoveFromDespawn(treeLog.AttachedTreeLog.gameObject);

        var rigidbody = treeLog.GetComponent<Rigidbody>();

        if (rigidbody != null)
            rigidbody.isKinematic = true;
        
    }

    public override void UpdateState(TreeLogStateManager treeLog)
    {
        if (treeLog.CarriedByPlayer == null)
            treeLog.SwitchState(treeLog.IdleState);
        
    }

    public override void ExitState(TreeLogStateManager treeLog)
    {
        var rigidbody = treeLog.GetComponent<Rigidbody>();

        if (rigidbody != null)
            rigidbody.isKinematic = false;
        
        treeLog.transform.SetParent(null);
        treeLog.ClearCarriedByPlayer();
    }

    public override void OnInteract(TreeLogStateManager treeLog, PlayerStateManager player)
    {
        if (player == treeLog.CarriedByPlayer)
        {
            Debug.Log($"Player drops TreeLog {treeLog.name}");
            
            treeLog.SwitchState(treeLog.IdleState);
            player.SwitchToIdleState();
        }
    }
}