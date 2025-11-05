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
        var bestTool = _player.PlayerInventory.GetBestToolForSkill(SkillType.Woodcutting, _player.PlayerSkills);
        var treeStateManager = _currentTree.GetComponent<TreeStateManager>();

        while (_currentTree.CurrentHealth > 0 && !treeStateManager.IsInDestroyedState())
        {
            float damage = _player.PlayerSkills.GetWoodcuttingSkill().BonusDamage + bestTool.EfficiencyBonus;
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

        _player.SwitchState(_player.IdleState);
    }

    private void SpawnTreeLog()
    {
        if (_currentTree.TreeLogPrefab != null)
            TreeLogSpawnController.Instance.SpawnLog(_currentTree, _currentTree.TreeLogPrefab);
    }
}