using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FishingCommandTests
{
    private PlayerMockHelper _playerMock;

    private GameObject _fishingSpotObject;
    private FishingSpot _fishingSpot;

    [SetUp]
    public void SetUp()
    {
        _playerMock = new();

        _fishingSpotObject = new GameObject("TestFishingSpot");
        _fishingSpot = _fishingSpotObject.AddComponent<FishingSpot>();
        _fishingSpotObject.transform.position = Vector3.zero;

        SetFishingSpotCapacity(40f);
        SetFishingSpotRequiredLevel(1);

        var fish = ScriptableObject.CreateInstance<Fish>();
        var fishes = new List<Fish> { fish };
        SetAvailableFishes(fishes);

        var interactionTiles = new List<Vector3>
        {
            new(0, 0, 0),
            new(1, 0, 0),
            new(-1, 0, 0)
        };

        SetInteractionTiles(interactionTiles);
    }

    [Test]
    public void Command_Should_Return_False_And_CommandError_When_Player_Is_Not_In_InteractionTile()
    {
        // Arrange
        _playerMock.GetPlayerGameObject().transform.position = new(0, 100, 20);

        var fishingCommand = new FishingCommand(_fishingSpot);

        // Act
        bool canExecute = fishingCommand.CanExecute(_playerMock.GetPlayerStateManager(), out CommandErrorCode errorCode);

        // Assert
        Assert.IsFalse(canExecute);
        Assert.AreEqual(CommandErrorCode.PlayerNotInInteractionTile, errorCode);
    }

    private void SetFishingSpotCapacity(float capacity)
    {
        var field = typeof(FishingSpot).GetField("_fishingCapacity", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(_fishingSpot, capacity);
    }

    private void SetFishingSpotRequiredLevel(int level)
    {
        var field = typeof(FishingSpot).GetField("_requiredLevelToInteract", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(_fishingSpot, level);
    }

    private void SetAvailableFishes(List<Fish> fishes)
    {
        var field = typeof(FishingSpot).GetField("_possibleFishesToCatch", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(_fishingSpot, fishes);
    }

    private void SetInteractionTiles(List<Vector3> tiles)
    {
        var field = typeof(FishingSpot).GetField("_interactionTiles", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(_fishingSpot, tiles);
    }
}
