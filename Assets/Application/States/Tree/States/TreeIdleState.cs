public class TreeIdleState : TreeBaseState
{
    public override void EnterState(TreeStateManager tree)
    {
        tree.AttachedTree.CurrentHealth = tree.AttachedTree.MaxHealth;
        tree.AttachedTree.SwitchToIdleModel();
    }

    public override void UpdateState(TreeStateManager tree)
    {
        if (tree.AttachedTree.CurrentHealth <= 0)
        {
            tree.SwitchState(tree.DestroyedState);
            return;
        }



    }

    public override void ExitState(TreeStateManager tree)
    {
    }


}
