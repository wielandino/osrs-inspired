using UnityEngine;

public class TreeLogIdleState : TreeLogBaseState
{
    public override void EnterState(TreeLogStateManager treeLog)
    {
        Debug.Log($"TreeLog entered Idle state");

        // TreeLog ist am Boden, Collider aktivieren
        if (treeLog.AttachedTreeLog != null)
        {
            treeLog.AttachedTreeLog.SetInteractable(true);
        }

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
        // Nur aufheben wenn Spieler im Idle State ist
        if (!player.IsInIdleState())
        {
            Debug.Log("Du kannst den TreeLog nicht aufheben während du etwas anderes machst");
            return;
        }

        Debug.Log($"Player picks up TreeLog {treeLog.name}");

        var previousPosition = treeLog.transform.position;

        // Spieler trägt jetzt den TreeLog
        treeLog.SetCarriedByPlayer(player);
        treeLog.SwitchState(treeLog.CarriedState);
        
        // Optional: TreeLog als Child des Players setzen
        treeLog.transform.SetParent(player.transform);
        treeLog.transform.localPosition = Vector3.up * 2f; // Über dem Spieler

        ObjectHelper.UpdateAdjacentInteractionTiles(previousPosition);

        // Spieler wechselt zu CarryingState
        player.SwitchToCarryingState(treeLog.AttachedTreeLog);
    }
}