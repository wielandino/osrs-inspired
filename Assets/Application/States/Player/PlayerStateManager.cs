using System.Collections.Generic;
using UnityEngine;


public class PlayerStateManager : MonoBehaviour
{
    public PlayerInputHandler InputHandler { get; private set; }

    [HideInInspector]
    public PlayerMovementController PlayerMovementController;
    public PlayerSkill PlayerSkills;
    public PlayerInventory PlayerInventory;
    public PlayerNeeds PlayerNeeds;

    private PlayerBaseState _currentState;

    public PlayerIdleState IdleState = new();
    public PlayerMoveState MoveState = new();
    public PlayerWoodcuttingState WoodcuttingState = new();
    public PlayerCarryingState CarryingState = new();
    public PlayerFishingState FishingState = new();
    public PlayerCookingState CookingState = new();
    public PlayerCraftingState CraftingState = new();

    private readonly Queue<PlayerCommandBase> _commandQueue = new();
    private PlayerCommandBase _currentCommand;
    public int CommandCounts;

    public bool IsInIdleState() => _currentState == IdleState;
    public bool IsInMoveState() => _currentState == MoveState;
    public bool IsInWoodcuttingState() => _currentState == WoodcuttingState;
    public bool IsInCarryingState() => _currentState == CarryingState;
    public bool IsInFishingState() => _currentState == FishingState;
    public bool IsInCookingState() => _currentState == CookingState;
    public bool IsInCraftingState() => _currentState == CraftingState;

    private void Start()
    {
        InputHandler = gameObject.GetComponent<PlayerInputHandler>();
        PlayerMovementController = gameObject.GetComponent<PlayerMovementController>();

        _currentState = IdleState;
        _currentState.EnterState(this);
    }

    private void Update()
    {
        CommandCounts = _commandQueue.Count;

        _currentState.UpdateState(this);

        if (CommandCounts > 0 || _currentCommand != null)
            ProcessCommandQueue();
    }

    private void ProcessCommandQueue()
    {
        if (_currentCommand == null && _commandQueue.Count > 0)
            _currentCommand = _commandQueue.Dequeue();

        if (_currentCommand != null)
        {
            if (!_currentCommand.IsCommandStarted())
            {
                if (_currentCommand.CanExecute(this))
                {
                    Debug.LogWarning($"{_currentCommand.GetType().Name} is executed!");
                    _currentCommand.Execute(this);
                }
                else
                {
                    _currentCommand = null;
                    return;
                }
            }

            if (_currentCommand.IsCommandStarted() && _currentCommand.IsComplete(this))
            {
                Debug.LogWarning($"{_currentCommand.GetType().Name} is completed!");

                _currentCommand = null;
            }  
        }
    }

    private void AddCommand(PlayerCommandBase newCommand)
    {
        _commandQueue.Enqueue(newCommand);
    }


    public void ClearCommands()
    {
        _currentCommand?.Cancel(this);
        _currentCommand = null;
        _commandQueue.Clear();
    }

    public void AddCommands(params PlayerCommandBase[] commands)
    {
        ClearCommands();

        foreach (var command in commands)
            AddCommand(command);
    }

    private void SwitchState(PlayerBaseState state)
    {
        _currentState?.ExitState(this);

        _currentState = state;
        state.EnterState(this);
    }

    public void SwitchToIdleState()
    {
        SwitchState(IdleState);
    }

    public void SwitchToMoveState(Vector3 targetPosition)
    {
        MoveState.SetTargetPosition(targetPosition);
        SwitchState(MoveState);
    }

    public void SwitchToWoodcuttingState(Tree tree, ISkillTool woodcuttingAxe)
    {
        WoodcuttingState.InititalWoodcuttingState(tree, woodcuttingAxe);
        SwitchState(WoodcuttingState);
    }

    public void SwitchToCarryingState(TreeLog treeLog)
    {
        CarryingState.SetCarriedTreeLog(treeLog);
        SwitchState(CarryingState);
    }

    public void SwitchToFishingState(FishingSpot fishingSpot)
    {
        FishingState.SetFishingSpot(fishingSpot);
        SwitchState(FishingState);
    }

    public void SwitchToCookingState(TreeLog treeLog, CookableItem item)
    {
        CookingState.SetTargetTreeLogAndItemToCook(treeLog, item, this);
        SwitchState(CookingState);
    }

    public void SwitchToCraftingState(CraftingRecipe recipe)
    {
        CraftingState.SetCraftingRecipe(recipe, this);
        SwitchState(CraftingState);
    }

}
