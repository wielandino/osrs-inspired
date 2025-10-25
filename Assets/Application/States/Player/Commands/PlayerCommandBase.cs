public abstract class PlayerCommandBase : IPlayerCommand
{
    protected bool _isComplete = false;
    protected bool _isStarted = false;

    public void Execute(PlayerStateManager player)
    {
        if (!_isStarted)
            _isStarted = true;

        ExecuteInternal(player);
    }

    public abstract void ExecuteInternal(PlayerStateManager player);
    public abstract bool CanExecute(PlayerStateManager player);

    public virtual bool IsComplete(PlayerStateManager player) => _isComplete;

    public virtual void Cancel(PlayerStateManager player)
    {
        _isComplete = true;
        _isStarted = false;
    }

    public virtual bool IsCommandStarted()
        => _isStarted;
}