using System.Collections;
using UnityEngine;

public class PlayerWoodcuttingState : PlayerBaseState
{
    private Tree _currentTree;
    private PlayerStateManager _player;
    private Coroutine _woodcuttingCoroutine;
    private bool _treeWasChopped = false;

    public void SetTargetTree(Tree tree)
    {
        _currentTree = tree;
    }

    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entered Woodcutting State!");
        _player = player;
        _treeWasChopped = false;

        // Starte Coroutine statt while-Schleife
        _woodcuttingCoroutine = _player.StartCoroutine(CuttingWoodCoroutine());
    }

    public override void UpdateState(PlayerStateManager player)
    {
        // Input wird weiterhin verarbeitet
        //player.InputHandler.HandleMouseClick();
    }

    // Cleanup beim Verlassen des States
    public override void ExitState(PlayerStateManager player)
    {
        Debug.Log("Exit PlayerWoodcuttingState!");
        if (_woodcuttingCoroutine != null)
        {
            _player.StopCoroutine(_woodcuttingCoroutine);
            _woodcuttingCoroutine = null;
        }
    }

    private IEnumerator CuttingWoodCoroutine()
    {
        var bestTool = _player.PlayerInventory.GetBestToolForSkill(SkillType.Woodcutting, _player.PlayerSkills);
        var treeStateManager = _currentTree.GetComponent<TreeStateManager>();

        while (_currentTree.CurrentHealth > 0 && !treeStateManager.IsInDestroyedState())
        {
            // Schaden pro "Schlag"
            float damage = _player.PlayerSkills.WoodcuttingBonusDamage + bestTool.EfficiencyBonus;
            _currentTree.CurrentHealth -= damage;

            Debug.Log($"Tree health: {_currentTree.CurrentHealth}");

            // Prüfe ob Baum jetzt gefällt ist
            if (_currentTree.CurrentHealth <= 0)
            {
                _treeWasChopped = true;
                break;
            }

            // Warte zwischen Schlägen (z.B. 1 Sekunde)
            yield return new WaitForSeconds(1f);
        }

        // Baum ist gefällt
        if (_treeWasChopped)
        {
            Debug.Log("Tree chopped down!");
            _player.PlayerSkills.IncreaseWoodcuttingXP(_currentTree.XPDropPerCut);

            // TreeLog spawnen
            SpawnTreeLog();

            // Baum zu DestroyedState wechseln
            treeStateManager.SwitchState(treeStateManager.DestroyedState);
        }

        _player.SwitchState(_player.IdleState);
    }

    private void SpawnTreeLog()
    {
        // Prüfe ob der Baum ein TreeLog Prefab hat
        if (_currentTree.TreeLogPrefab != null)
        {
            // Verwende den TreeLogSpawnController für das Spawning
            TreeLogSpawnController.Instance.SpawnLog(_currentTree, _currentTree.TreeLogPrefab);
        }
        else
        {
            Debug.LogWarning($"Tree {_currentTree.name} has no TreeLogPrefab assigned!");
        }
    }
}