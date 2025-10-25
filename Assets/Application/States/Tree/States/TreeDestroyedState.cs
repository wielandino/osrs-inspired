using UnityEngine;
using static UnityEditor.Progress;

public class TreeDestroyedState : TreeBaseState
{
    private float _treeRespawnTime;

    public override void EnterState(TreeStateManager tree)
    {
        Debug.Log("Tree in Destroy State!");

        _treeRespawnTime = tree.AttachedTree.RespawnTime;
        tree.AttachedTree.SwitchToDestroyedModel();
    }

    public override void UpdateState(TreeStateManager tree)
    {
        _treeRespawnTime -= Time.deltaTime;

        if (_treeRespawnTime < 0)
        {
            Debug.Log("Tree respawned!");
            tree.SwitchState(tree.IdleState);
        }
    }

    public override void ExitState(TreeStateManager tree)
    {
        throw new System.NotImplementedException();
    }
}
