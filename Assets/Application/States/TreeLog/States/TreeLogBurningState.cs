using UnityEngine;

public class TreeLogBurningState : TreeLogBaseState
{
    private float _burnTime;
    private const float MAX_BURN_TIME = 30f; // 30 Sekunden brennen
    
    private GameObject _fireEffect;

    public override void EnterState(TreeLogStateManager treeLog)
    {
        Debug.Log($"TreeLog {treeLog.name} is now burning!");
        
        _burnTime = MAX_BURN_TIME;
        
        // Feuer-Effekt spawnen (falls vorhanden)
        // _fireEffect = Instantiate(fireEffectPrefab, treeLog.transform.position + Vector3.up, Quaternion.identity);
        // _fireEffect.transform.SetParent(treeLog.transform);
        
        // TreeLog ist wieder interaktiv aber mit anderen Möglichkeiten
        if (treeLog.AttachedTreeLog != null)
        {
            treeLog.AttachedTreeLog.SetInteractable(true);
        }
    }

    public override void UpdateState(TreeLogStateManager treeLog)
    {
        _burnTime -= Time.deltaTime;
        
        if (_burnTime <= 0)
        {
            // TreeLog ist abgebrannt - könnte zu Asche werden oder verschwinden
            Debug.Log($"TreeLog {treeLog.name} burned out!");
            
            // Optional: Asche spawnen oder TreeLog zerstören
            Object.Destroy(treeLog.gameObject);
        }
    }

    public override void ExitState(TreeLogStateManager treeLog)
    {
        // Feuer-Effekt entfernen
        if (_fireEffect != null)
        {
            Object.Destroy(_fireEffect);
        }
    }

    public override void OnInteract(TreeLogStateManager treeLog, PlayerStateManager player)
    {
        // Brennender TreeLog hat andere Interaktionen:
        
        if (HasWater(player))
        {
            // Spieler mit Wasser kann Feuer löschen
            Debug.Log($"Player extinguishes TreeLog {treeLog.name}");
            treeLog.SwitchState(treeLog.IdleState);
        }
        else if (HasFood(player))
        {
            // Spieler kann Essen am Feuer kochen
            Debug.Log($"Player cooks food at TreeLog {treeLog.name}");
            CookFood(player);
        }
        else if (HasTorch(player))
        {
            // Spieler kann Fackel entzünden
            Debug.Log($"Player lights torch at TreeLog {treeLog.name}");
            LightTorch(player);
        }
        else
        {
            Debug.Log("TreeLog is burning! Be careful!");
            // Optional: Spieler nimmt Schaden wenn er brennendes Holz anfasst
        }
    }
    
    private bool HasWater(PlayerStateManager player)
    {
        // return player.PlayerInventory.HasItem("Water");
        return false; // Placeholder
    }
    
    private bool HasFood(PlayerStateManager player)
    {
        // return player.PlayerInventory.HasItem("RawMeat") || player.PlayerInventory.HasItem("Fish");
        return false; // Placeholder
    }
    
    private bool HasTorch(PlayerStateManager player)
    {
        // return player.PlayerInventory.HasItem("Torch");
        return false; // Placeholder
    }
    
    private void CookFood(PlayerStateManager player)
    {
        // Koche Essen - transformiere rohes Essen in gekochtes
        // player.PlayerInventory.ReplaceItem("RawMeat", "CookedMeat");
    }
    
    private void LightTorch(PlayerStateManager player)
    {
        // Entzünde Fackel
        // player.PlayerInventory.ReplaceItem("Torch", "LitTorch");
    }
}