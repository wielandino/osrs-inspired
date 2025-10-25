public abstract class TreeBaseState
{
    public abstract void EnterState(TreeStateManager tree);
    public abstract void UpdateState(TreeStateManager tree);
    public abstract void ExitState(TreeStateManager tree);
}
