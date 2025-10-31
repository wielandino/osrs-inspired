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

    void OnEnable()
    {
        _playerInputActions.Enable();

        _playerInputActions.Gameplay.ContextMenu.performed += OnRightClick;
        _playerInputActions.Gameplay.LeftClick.performed += HandleMouseClick;
    }

    void OnDisable()
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
            var interactable = hitInfo.collider.GetComponent<IInteractable>();

            Debug.Log(hitInfo.transform.position);

            if (interactable == null)
                return;

            ContextMenuPanel.Instance.ShowContextMenuForObject(interactable.GetContextMenuOptions(_playerStateManager),
                                                               mousePosition);
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
            else if (hit.collider.TryGetComponent<TreeLog>(out var treeLog))
            {
                if (treeLog.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                    treeLog = GetTopTreeLogAtPosition(hit.transform.position);
                    
                HandleTreeLogInteraction(treeLog);        
            }
            else if (hit.collider.TryGetComponent<Tile>(out var tile))
            {
                var moveCommand = new MoveCommand(hit.point);
                _playerStateManager.ReplaceCommands(moveCommand);
            }
        }
        
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
                if (selectedItem.Callback.CanCreateCommand(treeLog.gameObject, _playerStateManager))
                {
                    var command = selectedItem.Callback.CreateCommand(treeLog.gameObject, _playerStateManager);
                    
                    if (command.CanExecute(_playerStateManager))
                    {
                        _playerStateManager.ReplaceCommands(command);
                        return;
                    }
                }
            }
        }

        // Treelog can only collected if player and treelog is in idle
        if (_playerStateManager.IsInIdleState() && treeLog.GetStateManager().IsInIdleState())
        {
            HandleTreeLogCarrying(treeLog);
            return;
        }
    }

    private void HandleTreeLogBurningInteraction(TreeLog treelog)
    {
        Debug.Log("HandleTreeLogBurningInteraction");
    }

    private void HandleTreeLogCarrying(TreeLog treeLog)
    {
        var carryTreeLogCommand = new CarryTreeLogCommand(treeLog);

        if (carryTreeLogCommand.CanExecute(_playerStateManager, out CommandErrorCode errorCode))
        {
            _playerStateManager.ReplaceCommands(carryTreeLogCommand);
        }
        else
        {
            if (errorCode == CommandErrorCode.PlayerNotInInteractionTile)
            {
                Vector3 nearestTile = _playerMovementService.GetNearestInteractionTile(treeLog.InteractionTiles);
                var moveCommand = new MoveCommand(nearestTile);

                _playerStateManager.ReplaceCommands(moveCommand, carryTreeLogCommand);
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

        // Alle TreeLogs treffen
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f, LayerMask.GetMask("Obstacle"));
        
        TreeLog topTreeLog = null;
        float highestY = float.MinValue;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent<TreeLog>(out var treeLog))
            {
                // Nimm den höchsten
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
            _playerStateManager.ReplaceCommands(woodcuttingCommand);
        }
        else
        {
            // Prüfe ob es nur ein Positionsproblem ist
            if (errorCode == CommandErrorCode.PlayerNotInInteractionTile)
            {
                Vector3 nearestTile = _playerMovementService.GetNearestInteractionTile(tree.InteractionTiles);
                var moveCommand = new MoveCommand(nearestTile);

                // Ersetze Queue mit: Bewegen -> Holzhacken
                _playerStateManager.ReplaceCommands(moveCommand, woodcuttingCommand);
            }
            else
            {
                Debug.Log($"HandleTreeInteraction could not be executed. ErrorCode: {errorCode}");
            }
        }
    }
}