using UnityEngine;

public class PlayerMockHelper
{
    private GameObject _playerGameObject;
    private readonly PlayerStateManager _playerStateManager;

    public PlayerMockHelper()
    {
        _playerGameObject = new GameObject("TestPlayer");
        _playerStateManager = _playerGameObject.AddComponent<PlayerStateManager>();
        _playerStateManager.PlayerInventory = _playerGameObject.AddComponent<PlayerInventory>();
        _playerStateManager.PlayerSkills = _playerGameObject.AddComponent<PlayerSkill>();
        _playerStateManager.PlayerMovementController = _playerGameObject.AddComponent<PlayerMovementController>();
    }

    public GameObject GetPlayerGameObject()
        => _playerGameObject;

    public PlayerStateManager GetPlayerStateManager()
        => _playerStateManager;
}