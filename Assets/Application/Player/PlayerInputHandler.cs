using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerStateManager _playerStateManager;
    private PlayerMovementService _playerMovementService;
    
    
    private List<IClickStrategy> _clickStrategies;
    private PlayerInputActions _playerInputActions;

    private void Awake()
    {
        _playerStateManager = gameObject.GetComponent<PlayerStateManager>();
        _playerMovementService = gameObject.GetComponent<PlayerMovementService>();
        _playerInputActions = new PlayerInputActions();
        InitializeClickStrategies();
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

    private void InitializeClickStrategies()
    {
        _clickStrategies = new List<IClickStrategy>
        {
            new TreeClickStrategy(_playerMovementService, _playerStateManager),
            new TileClickStrategy(_playerStateManager),
            new TreeLogClickStrategy(_playerMovementService, _playerStateManager)
        };

        _clickStrategies = _clickStrategies.OrderBy(s => s.Priority).ToList();
    }

    public void HandleMouseClick(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (!TryGetRaycastHit(out var hit))
            return;

        foreach (var strategy in _clickStrategies)
        {
            if (strategy.CanHandle(hit))
            {
                strategy.Handle(hit);
                break;
            }
        }
    }

    private bool TryGetRaycastHit(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit);
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
}