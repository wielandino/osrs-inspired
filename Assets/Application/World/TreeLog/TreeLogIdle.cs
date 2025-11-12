using UnityEngine;

public class TreeLogIdle : MonoBehaviour, ITooltipProvider
{
    [SerializeField]
    private TreeLog _treeLog;

    private void OnEnable()
    {
        _treeLog.CanBeStacked = true;
    }

    public string GetTooltipText()
    {
        string toolTipText = "<color=white>Pick up</color> <color=yellow>treelog</color>";

        var selectedItem = PlayerInventory.Instance.SelectedItem;

        if (selectedItem != null && 
            ToolValidator.CanToolBeUsedForSkill(selectedItem, SkillType.Firemaking))
        {
            if (!_treeLog.IsTreeLogStacked())
                toolTipText = "<color=white>Set</color> <color=yellow>treelog</color> on fire";
        }
        
        return toolTipText;
    }
}