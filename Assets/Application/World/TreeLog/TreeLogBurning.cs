using UnityEngine;

public class TreeLogBurning : MonoBehaviour, ITooltipProvider
{
    [SerializeField]
    private TreeLog _treeLog;

    private void OnEnable()
    {
        _treeLog.CanBeStacked = false;
    }

    public string GetTooltipText()
    {
        string toolTipText = "<color=white>Burning</color> <color=yellow>treelog</color>";

        var selectedItem = PlayerInventory.Instance.SelectedItem;

        if (selectedItem != null && selectedItem is CookableItem)
            toolTipText = $"<color=white>Cook</color> <color=yellow>{selectedItem.Name}</color>";
        
        return toolTipText;
    }
}
