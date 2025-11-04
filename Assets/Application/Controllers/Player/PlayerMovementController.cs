using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speedModifier = 1f;

    private PlayerStateManager _player;
    private PlayerMovementService _movementService;
    private Seeker _seeker;
    private List<Vector3> _pathPoints = new();
    private int _currentPathIndex;
    private float _moveProgress;
    private Vector3 _startPos;
    private Vector3 _targetPosition = Vector3.zero;
    private bool _isPathCallbackRegistered = false;
    private bool _isMoving = false;
    public event System.Action OnMovementStarted;
    public event System.Action OnMovementCompleted;
    public event System.Action OnMovementCancelled;

    public bool IsMoving => _isMoving;

    private void Awake()
    {
        _player = gameObject.GetComponent<PlayerStateManager>();
        _movementService = gameObject.GetComponent<PlayerMovementService>();
        _seeker = gameObject.GetComponent<Seeker>();
    }

    private void Start()
    {
        if (!_isPathCallbackRegistered)
        {
            _seeker.pathCallback += OnPathComplete;
            _isPathCallbackRegistered = true;
        }
    }

    private void Update()
    {
        if (_isMoving)
            HandleTileMovement();
    }

    public void StartMovement(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
        _seeker.StartPath(_player.transform.position, targetPosition);
    }

    public void StopMovement()
    {
        _isMoving = false;
        ResetMovementState();
        OnMovementCancelled?.Invoke();
    }

    public void SetSpeedModifier(float modifier)
    {
        speedModifier = modifier;
    }

    private void OnPathComplete(Path p)
    {
        if (p.error)
        {
            _isMoving = false;
            OnMovementCancelled?.Invoke();
            return;
        }

        _pathPoints = p.vectorPath;
        _currentPathIndex = 0;
        _moveProgress = 0f;

        if (_pathPoints.Count > 1)
        {
            _isMoving = true;
            StartNextMovement();
            OnMovementStarted?.Invoke();
        }
    }

    private void StartNextMovement()
    {
        _startPos = SnapToGrid(_pathPoints[_currentPathIndex]);
        _targetPosition = SnapToGrid(_pathPoints[_currentPathIndex + 1]);

        Vector3 direction = (_targetPosition - _startPos).normalized;
        
        if (direction != Vector3.zero)
        {
            float targetYRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, targetYRotation, 0);
            
            var groundSnap = gameObject.GetComponent<SimpleGroundSnap>();
            Vector3 worldNormal = groundSnap.GetGroundNormal(_targetPosition);
            
            if (groundSnap.meshTransform != null)
            {
                Vector3 localNormal = transform.InverseTransformDirection(worldNormal);
                
                float xRotation = -Mathf.Asin(-localNormal.z) * Mathf.Rad2Deg;
                float zRotation = -Mathf.Asin(localNormal.x) * Mathf.Rad2Deg; 
                
                groundSnap.meshTransform.localRotation = Quaternion.Euler(xRotation, 0, zRotation);
            }
        }
        
        _moveProgress = 0f;
    }

    private Vector3 SnapToGrid(Vector3 position)
    {
        var groundSnap = gameObject.GetComponent<SimpleGroundSnap>();

        Vector3 gridPosition = new(
            Mathf.Round(position.x),
            0f,
            Mathf.Round(position.z)
        );

        float groundHeight = groundSnap.GetGroundHeight(gridPosition);
        gridPosition.y = groundHeight;

        return gridPosition;
    }

    private void HandleTileMovement()
    {
        float adjustedTimePerTile = _movementService.TimePerTile / speedModifier;

        _moveProgress += Time.deltaTime / adjustedTimePerTile;
        _moveProgress = Mathf.Clamp01(_moveProgress);

        _player.transform.position = Vector3.Lerp(_startPos, _targetPosition, _moveProgress);

        if (_moveProgress >= 1f)
        {
            _currentPathIndex++;
            if (_currentPathIndex < _pathPoints.Count - 1)
            {
                StartNextMovement();
            }
            else
            {
                _player.transform.position = SnapToGrid(_targetPosition);
                _isMoving = false;
                ResetMovementState();
                OnMovementCompleted?.Invoke();
            }
        }
    }

    private void ResetMovementState()
    {
        _pathPoints.Clear();
        _currentPathIndex = 0;
        _moveProgress = 0f;
    }

    private void OnDestroy()
    {
        if (_isPathCallbackRegistered)
            _seeker.pathCallback -= OnPathComplete;
        
    }
}