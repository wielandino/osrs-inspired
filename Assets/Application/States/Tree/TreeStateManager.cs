using UnityEngine;


public class TreeStateManager : MonoBehaviour
{
    private TreeBaseState _currentState;

    public TreeIdleState IdleState = new();
    public TreeDestroyedState DestroyedState = new();

    public bool IsInIdleState() => _currentState == IdleState;
    public bool IsInDestroyedState() => _currentState == DestroyedState;

    public Tree AttachedTree;

    private void Start()
    {
        AttachedTree = gameObject.GetComponent<Tree>();
        _currentState = IdleState;
        
        _currentState.EnterState(this);
    }

    private void Update()
    {
        _currentState.UpdateState(this);
    }

    public void SwitchState(TreeBaseState state)
    {
        _currentState = state;
        state.EnterState(this);
    }

}
