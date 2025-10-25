public interface IPlayerCommand
{
    public void ExecuteInternal(PlayerStateManager player);
    public bool CanExecute(PlayerStateManager player);
    public bool IsComplete(PlayerStateManager player);
    public void Cancel(PlayerStateManager player);
}