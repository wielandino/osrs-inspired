using UnityEngine;

public class WoodcuttingCommand : PlayerCommandBase
{
    private Tree _targetTree;

    public WoodcuttingCommand(Tree tree)
    {
        _targetTree = tree;
    }

    public override void Cancel(PlayerStateManager player)
    {

    }

    public override bool CanExecute(PlayerStateManager player)
    {
        return CanExecute(player, out _);
    }

    public bool CanExecute(PlayerStateManager player, out string errorMessage)
    {
        errorMessage = string.Empty;

        // Prüfe zuerst ob Spieler einen TreeLog trägt
        if (player.IsInCarryingState())
        {
            errorMessage = "Du kannst nicht Holz hacken während du einen TreeLog trägst";
            return false;
        }

        if (_targetTree == null)
        {
            errorMessage = "Kein Baum ausgewählt";
            return false;
        }

        if (!_targetTree.InteractionTiles.Contains(player.transform.position))
        {
            errorMessage = "Du bist zu weit vom Baum entfernt";
            return false;
        }

        var targetTreeState = _targetTree.GetComponent<TreeStateManager>();

        if (targetTreeState.IsInDestroyedState())
        {
            errorMessage = "Dieser Baum ist bereits gefällt";
            return false;
        }

        if (player.PlayerSkills.WoodcuttingLevel < _targetTree.GetRequiredLevelToCut())
        {
            errorMessage = $"Du benötigst Holzfäller-Level {_targetTree.GetRequiredLevelToCut()}";
            return false;
        }

        if (!player.PlayerInventory.HasValidToolForSkill(SkillType.Woodcutting, player.PlayerSkills))
        {
            errorMessage = "Du hast kein geeignetes Werkzeug zum Holzfällen";
            return false;
        }

        return true;
    }

    public override void ExecuteInternal(PlayerStateManager player)
    {
        player.SwitchToWoodcuttingState(_targetTree);
    }

    public override bool IsComplete(PlayerStateManager player)
        => player.IsInWoodcuttingState();
}