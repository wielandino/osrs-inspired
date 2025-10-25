using UnityEngine;

public class TreeLogStateManager : MonoBehaviour
{
    private TreeLogBaseState _currentState;

    public TreeLogIdleState IdleState = new();
    public TreeLogCarriedState CarriedState = new();
    public TreeLogBurningState BurningState = new();

    public bool IsInIdleState() => _currentState == IdleState;
    public bool IsInCarriedState() => _currentState == CarriedState;
    public bool IsInBurningState() => _currentState == BurningState;

    public TreeLog AttachedTreeLog;
    public PlayerStateManager CarriedByPlayer { get; private set; }

    private void Start()
    {
        AttachedTreeLog = gameObject.GetComponent<TreeLog>();
        _currentState = IdleState;
        _currentState.EnterState(this);
    }

    private void Update()
    {
        _currentState.UpdateState(this);
    }

    public void SwitchState(TreeLogBaseState state)
    {
        _currentState.ExitState(this);
        _currentState = state;
        state.EnterState(this);
    }

    public void OnInteract(PlayerStateManager player)
    {
        _currentState.OnInteract(this, player);
    }

    public void SetCarriedByPlayer(PlayerStateManager player)
    {
        CarriedByPlayer = player;
    }

    public void ClearCarriedByPlayer()
    {
        CarriedByPlayer = null;
    }
}