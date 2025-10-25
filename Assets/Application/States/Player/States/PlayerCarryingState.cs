using UnityEngine;

public class PlayerCarryingState : PlayerBaseState
{
    private TreeLog _carriedTreeLog;

    private PlayerCarryingBaseSubState _currentSubState;

    public IdleWithCarryingTreeLogState IdleWithCarryingTreeLogState;
    public MoveWithCarryingTreeLogState MoveWithCarryingTreeLogState;

    public bool IsInIdleState() => _currentSubState == IdleWithCarryingTreeLogState;


    public PlayerCarryingState()
    {
        IdleWithCarryingTreeLogState = new();
        MoveWithCarryingTreeLogState = new();

        _currentSubState = IdleWithCarryingTreeLogState;
    }

    public void SetCarriedTreeLog(TreeLog treeLog)
    {
        _carriedTreeLog = treeLog;
    }

    public TreeLog GetCarriedTreeLog()
    {
        return _carriedTreeLog;
    }

    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log($"Player entered Carrying state with TreeLog: {_carriedTreeLog?.name}");
        _carriedTreeLog.InteractionTiles.Clear();
        _carriedTreeLog.gameObject.layer = LayerMask.NameToLayer("GroundItem");
        GridManager.Instance.UpdateGraph();

        _currentSubState.EnterState(this, player);

        player.PlayerMovementController = player.GetComponent<PlayerMovementController>();
    }

    public override void UpdateState(PlayerStateManager player)
    {
        // Prüfe ob TreeLog noch existiert
        if (_carriedTreeLog == null)
        {
            player.SwitchState(player.IdleState);
        }

        _currentSubState.UpdateState(this, player);
    }

    public void DropTreeLog(PlayerStateManager player, DropTreeLogCommand command, Vector3 targetPosition)
    {
        _carriedTreeLog.transform.SetParent(null);
        _carriedTreeLog.GetStateManager().ClearCarriedByPlayer();

        targetPosition.y = ObjectHelper.CalculateCorrectYPosition(targetPosition, _carriedTreeLog.gameObject);
        _carriedTreeLog.transform.position = targetPosition;

        _carriedTreeLog.gameObject.layer = LayerMask.NameToLayer("Obstacle");
        GridManager.Instance.UpdateGraphOfObject(_carriedTreeLog.GetComponent<Collider>());

        player.SwitchState(player.IdleState);
    }

    public override void ExitState(PlayerStateManager player)
    {
        // Cleanup bleibt gleich
        if (_carriedTreeLog != null)
        {
            var treeLogStateManager = _carriedTreeLog.GetStateManager();
            if (treeLogStateManager != null && treeLogStateManager.IsInCarriedState())
            {
                treeLogStateManager.SwitchState(treeLogStateManager.IdleState);
            }
        }

        _carriedTreeLog = null;
    }

    public void SwitchToMoveSubState(PlayerStateManager player, Vector3 targetPosition)
    {
        _currentSubState?.ExitState(this, player);

        MoveWithCarryingTreeLogState.SetTargetPosition(targetPosition);
        _currentSubState = MoveWithCarryingTreeLogState;
        _currentSubState.EnterState(this, player);
    }

    public void SwitchToIdleSubState(PlayerStateManager player)
    {
        _currentSubState?.ExitState(this, player);

        _currentSubState = IdleWithCarryingTreeLogState;
        _currentSubState.EnterState(this, player);
    }
}