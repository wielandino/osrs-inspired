using System.Collections;
using UnityEngine;

public class PlayerWoodcuttingState : PlayerBaseState
{
    private Tree _currentTree;
    private PlayerStateManager _player;
    private Coroutine _woodcuttingCoroutine;
    private bool _treeWasChopped = false;
    private ISkillTool _woodcuttingAxe;

    public void InititalWoodcuttingState(Tree tree, ISkillTool woodcuttingAxe)
    {
        _currentTree = tree;
        _woodcuttingAxe = woodcuttingAxe;
    }

    public override void EnterState(PlayerStateManager player)
    {
        _player = player;
        _treeWasChopped = false;

        _woodcuttingCoroutine = _player.StartCoroutine(CuttingWoodCoroutine());
    }

    public override void UpdateState(PlayerStateManager player)
    {
    }

    public override void ExitState(PlayerStateManager player)
    {
        if (_woodcuttingCoroutine != null)
        {
            _player.StopCoroutine(_woodcuttingCoroutine);
            _woodcuttingCoroutine = null;
        }
    }

    private IEnumerator CuttingWoodCoroutine()
    {
        Debug.Log($"Cutting Wood with EffiencyBonus '{_woodcuttingAxe.EfficiencyBonus}'");
        var tool = _woodcuttingAxe;
        var treeStateManager = _currentTree.GetComponent<TreeStateManager>();

        while (_currentTree.CurrentHealth > 0 && !treeStateManager.IsInDestroyedState())
        {
            float damage = _player.PlayerSkills.GetWoodcuttingSkill().BonusDamage + tool.EfficiencyBonus;
            _currentTree.CurrentHealth -= damage;

            if (_currentTree.CurrentHealth <= 0)
            {
                _treeWasChopped = true;
                break;
            }

            yield return new WaitForSeconds(1f);
        }

        if (_treeWasChopped)
        {
            _player.PlayerSkills.GetWoodcuttingSkill().IncreaseWoodcuttingXP(_currentTree.XPDropPerCut);

            SpawnTreeLog();
            treeStateManager.SwitchState(treeStateManager.DestroyedState);
        }

        _player.SwitchToIdleState();
    }

    private void SpawnTreeLog()
    {
        if (_currentTree.TreeLogPrefab != null)
            TreeLogSpawnController.Instance.SpawnLog(_currentTree, _currentTree.TreeLogPrefab);
    }
}