using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerMovementController _movementController;
    private PlayerStateManager _playerStateManager;

    private PlayerInputActions _playerInputActions;
    private PlayerMovementService _playerMovementService;

    void Awake()
    {
        _playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        _movementController = gameObject.GetComponent<PlayerMovementController>();
        _playerStateManager = gameObject.GetComponent<PlayerStateManager>();
        _playerMovementService = gameObject.GetComponent<PlayerMovementService>();
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();

        _playerInputActions.Gameplay.ContextMenu.performed += OnRightClick;
        _playerInputActions.Gameplay.LeftClick.performed += HandleMouseClick;
    }

    private void OnDisable()
    {
        _playerInputActions.Gameplay.ContextMenu.performed -= OnRightClick;
        _playerInputActions.Disable();
    }

    private void OnRightClick(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
            
        var mousePosition = _playerInputActions.Gameplay.MousePosition.ReadValue<Vector2>();
        var ray = Camera.main.ScreenPointToRay(mousePosition);

        if(Physics.Raycast(ray, out var hitInfo))
        {
            if (hitInfo.collider.TryGetComponent<IInteractable>(out var interactable) ||
                hitInfo.collider.transform.parent.TryGetComponent(out interactable))
            {
                ContextMenuPanel.Instance
                    .ShowContextMenuForObject(interactable.GetContextMenuOptions(_playerStateManager),
                                              mousePosition);
            }

            return;
        }
    }

    public void HandleMouseClick(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent<Tree>(out var tree))
            {
                HandleTreeInteraction(tree);
                return;
            }
            else if (hit.collider.TryGetComponent<TreeLog>(out var treeLog) ||
                     hit.collider.transform.parent.TryGetComponent(out treeLog))
            {
                if (treeLog.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                    treeLog = GetTopTreeLogAtPosition(hit.transform.position);

                HandleTreeLogInteraction(treeLog);
            }
            else if (hit.collider.TryGetComponent<Tile>(out var tile))
            {
                if (tile.GetTileType() != Tile.TileType.WalkableTile)
                    return;

                var moveCommand = new MoveCommand(hit.point);
                _playerStateManager.AddCommands(moveCommand);
            }
        }

        if (_playerStateManager.PlayerInventory.SelectedItem != null)
            _playerStateManager.PlayerInventory.DeSelectCurrentItem();
    }

    private void HandleTreeLogInteraction(TreeLog treeLog)
    {
        if (!treeLog.IsInteractable())
            return;

        if (_playerStateManager.PlayerInventory.SelectedItem != null)
        {
            var selectedItem = _playerStateManager.PlayerInventory.SelectedItem;

            if (selectedItem?.Callback != null)
            {
                Vector3 nearestTile = _playerMovementService.GetNearestInteractionTile(treeLog.InteractionTiles);
                var command = selectedItem.Callback.ExecuteCallback(treeLog.gameObject, _playerStateManager);
                var moveCommand = new MoveCommand(nearestTile);

                if (command != null)
                    _playerStateManager.AddCommands(moveCommand, command);

                return; 
            }
        }

        // Treelog can only collected if player and treelog is in idle
        if (_playerStateManager.IsInIdleState() &&
             treeLog.GetStateManager().IsInIdleState())
        {
            HandleTreeLogCarrying(treeLog);
            return;
        }
    }

    private void HandleTreeLogCarrying(TreeLog treeLog)
    {
        var carryTreeLogCommand = new CarryTreeLogCommand(treeLog);

        if (carryTreeLogCommand.CanExecute(_playerStateManager, out CommandErrorCode errorCode))
        {
            _playerStateManager.AddCommands(carryTreeLogCommand);
        }
        else
        {
            if (errorCode == CommandErrorCode.PlayerNotInInteractionTile)
            {
                Vector3 nearestTile = _playerMovementService.GetNearestInteractionTile(treeLog.InteractionTiles);
                var moveCommand = new MoveCommand(nearestTile);

                _playerStateManager.AddCommands(moveCommand, carryTreeLogCommand);
            }
            else
            {
                Debug.Log($"HandleTreeLogCarrying could not be executed. ErrorCode: {errorCode}");
            }
        }
    }

    public TreeLog GetTopTreeLogAtPosition(Vector3 position)
    {
        Vector3 rayStart = new(position.x, 50f, position.z);
        Ray ray = new(rayStart, Vector3.down);

        RaycastHit[] hits = Physics.RaycastAll(ray, 100f, LayerMask.GetMask("Obstacle"));
        
        TreeLog topTreeLog = null;
        float highestY = float.MinValue;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent<TreeLog>(out var treeLog) ||
                hit.collider.transform.parent.TryGetComponent(out treeLog))
            {
                if (treeLog.transform.position.y > highestY)
                {
                    highestY = treeLog.transform.position.y;
                    topTreeLog = treeLog;
                }
                
            }
        }
        
        return topTreeLog;
    }

    private void HandleTreeInteraction(Tree tree)
    {
        var woodcuttingCommand = new WoodcuttingCommand(tree);

        if (woodcuttingCommand.CanExecute(_playerStateManager, out CommandErrorCode errorCode))
        {
            _playerStateManager.AddCommands(woodcuttingCommand);
        }
        else
        {

            if (errorCode == CommandErrorCode.PlayerNotInInteractionTile)
            {
                Vector3 nearestTile = _playerMovementService.GetNearestInteractionTile(tree.InteractionTiles);
                var moveCommand = new MoveCommand(nearestTile);

                _playerStateManager.AddCommands(moveCommand, woodcuttingCommand);
            }
            else
            {
                Debug.Log($"HandleTreeInteraction could not be executed. ErrorCode: {errorCode}");
            }
        }
    }
}