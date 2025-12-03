public abstract class TreeLogBaseState
{
    public abstract void EnterState(TreeLogStateManager treeLog);
    public abstract void UpdateState(TreeLogStateManager treeLog);
    public abstract void ExitState(TreeLogStateManager treeLog);
    public abstract void OnInteract(TreeLogStateManager treeLog, PlayerStateManager player);
}