using UnityEngine;

public class TreeLogIdleState : TreeLogBaseState
{
    public override void EnterState(TreeLogStateManager treeLog)
    {
        Debug.Log($"TreeLog entered Idle state");

        if (treeLog.AttachedTreeLog != null)
            treeLog.AttachedTreeLog.SetInteractable(true);
        

        var interactionTiles = ObjectHelper.CollectInteractionTilesOfPosition(treeLog.transform.position);
        treeLog.AttachedTreeLog.InteractionTiles = interactionTiles;

        ObjectHelper.UpdateAdjacentInteractionTiles(treeLog.transform.position);
    }

    public override void UpdateState(TreeLogStateManager treeLog)
    {
        // Idle - keine spezielle Update-Logik nötig
    }

    public override void ExitState(TreeLogStateManager treeLog)
    {
        // TreeLog wird nicht mehr interaktiv wenn aufgehoben
        if (treeLog.AttachedTreeLog != null)
        {
            treeLog.AttachedTreeLog.SetInteractable(false);
        }
    }

    public override void OnInteract(TreeLogStateManager treeLog, PlayerStateManager player)
    {
        if (!player.IsInIdleState())
            return;

        Debug.Log($"Player picks up TreeLog {treeLog.name}");

        var previousPosition = treeLog.transform.position;

        treeLog.SetCarriedByPlayer(player);
        treeLog.SwitchState(treeLog.CarriedState);
        
        treeLog.transform.SetParent(player.transform);
        treeLog.transform.localPosition = Vector3.up * 2f;

        ObjectHelper.UpdateAdjacentInteractionTiles(previousPosition);

        player.SwitchToCarryingState(treeLog.AttachedTreeLog);
    }
}