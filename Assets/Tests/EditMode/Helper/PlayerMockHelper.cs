using UnityEngine;

public class PlayerMockHelper
{
    private readonly GameObject _playerGameObject;
    private readonly PlayerStateManager _playerStateManager;

    public PlayerMockHelper()
    {
        _playerGameObject = new GameObject("PlayerMockHelperTestObject");
        _playerStateManager = _playerGameObject.AddComponent<PlayerStateManager>();
        _playerStateManager.PlayerInventory = _playerGameObject.AddComponent<PlayerInventory>();
        _playerStateManager.PlayerSkills = _playerGameObject.AddComponent<PlayerSkill>();
        _playerGameObject.AddComponent<FishingSkill>();
        _playerGameObject.AddComponent<WoodcuttingSkill>();
        _playerStateManager.PlayerMovementController = _playerGameObject.AddComponent<PlayerMovementController>();




        _playerStateManager.SwitchToIdleState();
    }

    public GameObject GetPlayerGameObject()
        => _playerGameObject;

    public PlayerStateManager GetPlayerStateManager()
        => _playerStateManager;
}