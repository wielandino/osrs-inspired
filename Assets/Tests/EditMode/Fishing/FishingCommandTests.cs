using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class FishingCommandTests
{
    private PlayerMockHelper _playerMock;
    private GameObject _fishingSpotObject;
    private FishingSpot _fishingSpot;
    private List<GameObject> _testObjects = new List<GameObject>();

    [SetUp]
    public void SetUp()
    {
        // Player Mock
        _playerMock = new();

        // FishingSpot Setup mit Standard-Konfiguration
        _fishingSpotObject = CreateFishingSpot();
        _fishingSpot = _fishingSpotObject.GetComponent<FishingSpot>();
    }

    [TearDown]
    public void TearDown()
    {
        foreach (var obj in _testObjects)
        {
            if (obj != null)
                Object.DestroyImmediate(obj);
        }
        _testObjects.Clear();
        
        // Player Mock Cleanup (falls nötig)
        if (_playerMock != null && _playerMock.GetPlayerGameObject() != null)
            Object.DestroyImmediate(_playerMock.GetPlayerGameObject());
    }

    [Test]
    public void Command_Should_Return_False_When_Player_Not_In_InteractionTile()
    {
        // Arrange
        _playerMock.GetPlayerGameObject().transform.position = new(100, 0, 100); // Weit weg!
        var fishingCommand = new FishingCommand(_fishingSpot);

        // Act
        bool canExecute = fishingCommand.CanExecute(_playerMock.GetPlayerStateManager(), out CommandErrorCode errorCode);

        // Assert
        Assert.IsFalse(canExecute, "Command sollte nicht ausführbar sein wenn Player zu weit weg ist");
        Assert.AreEqual(CommandErrorCode.PlayerNotInInteractionTile, errorCode);
    }

    [Test]
    public void Command_Should_Return_False_When_Player_Has_No_Tool()
    {
        // Arrange
        _playerMock.GetPlayerGameObject().transform.position = new(1, 0, 0);

        var fishingCommand = new FishingCommand(_fishingSpot);

        // Act
        bool canExecute = fishingCommand.CanExecute(_playerMock.GetPlayerStateManager(), out CommandErrorCode errorCode);

        // Assert
        Assert.IsFalse(canExecute, "Command sollte nicht ausführbar sein ohne Tool");
        Assert.AreEqual(CommandErrorCode.PlayerSkillRequirementNotMet, errorCode);
    }

    [Test]
    public void Command_Should_Return_False_When_No_Fishes_Available()
    {
        // Arrange
        _playerMock.GetPlayerGameObject().transform.position = new(1, 0, 0);
        
        var fishingRod = ItemMockHelper.GetFishingRodLevel1();
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(fishingRod);
        
        SetAvailableFishes(new List<Fish>(), _fishingSpot);
        
        var fishingCommand = new FishingCommand(_fishingSpot);

        // Act
        bool canExecute = fishingCommand.CanExecute(_playerMock.GetPlayerStateManager(), out CommandErrorCode errorCode);

        // Assert
        Assert.IsFalse(canExecute, "Command sollte nicht ausführbar sein ohne Fische");
        Assert.AreEqual(CommandErrorCode.NoTarget, errorCode);
    }

    [Test]
    public void Command_Should_Return_False_When_FishingSpot_Capacity_Is_Zero()
    {
        // Arrange
        _playerMock.GetPlayerGameObject().transform.position = new(1, 0, 0);
        
        var fishingRod = ItemMockHelper.GetFishingRodLevel1();
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(fishingRod);
        
        SetFishingSpotCapacity(0f, _fishingSpot);
        
        var fishingCommand = new FishingCommand(_fishingSpot);

        // Act
        bool canExecute = fishingCommand.CanExecute(_playerMock.GetPlayerStateManager(), out CommandErrorCode errorCode);

        // Assert
        Assert.IsFalse(canExecute, "Command sollte nicht ausführbar sein wenn Capacity leer ist");
        Assert.AreEqual(CommandErrorCode.NoTarget, errorCode);
    }

    [Test]
    public void Command_Should_Execute_Successfully_When_All_Conditions_Met()
    {
        // Arrange
        _playerMock.GetPlayerGameObject().transform.position = new(1, 0, 0); // Auf gültigem Tile
        
        var fishingRod = ItemMockHelper.GetFishingRodLevel1();
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(fishingRod);
        
        var fishingCommand = new FishingCommand(_fishingSpot);

        // Act
        bool canExecute = fishingCommand.CanExecute(_playerMock.GetPlayerStateManager(), out CommandErrorCode errorCode);

        // Assert
        Assert.IsTrue(canExecute, "Command sollte ausführbar sein wenn alle Bedingungen erfüllt sind");
        Assert.AreEqual(CommandErrorCode.Default, errorCode);
    }

    [Test]
    public void Command_Should_Execute_When_Player_On_Different_Valid_Tiles()
    {
        var validTiles = new List<Vector3>
        {
            new(0, 0, 0),
            new(1, 0, 0),
            new(-1, 0, 0)
        };

        var fishingRod = ItemMockHelper.GetFishingRodLevel1();
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(fishingRod);

        foreach (var tile in validTiles)
        {
            // Arrange
            _playerMock.GetPlayerGameObject().transform.position = tile;
            var fishingCommand = new FishingCommand(_fishingSpot);

            // Act
            bool canExecute = fishingCommand.CanExecute(_playerMock.GetPlayerStateManager(), out CommandErrorCode errorCode);

            // Assert
            Assert.IsTrue(canExecute, $"Command sollte ausführbar sein auf Tile {tile}");
        }
    }

    private GameObject CreateFishingSpot()
    {
        var fishingSpotObject = new GameObject("TestFishingSpot");
        _testObjects.Add(fishingSpotObject); // Für Cleanup registrieren
        
        var fishingSpot = fishingSpotObject.AddComponent<FishingSpot>();
        fishingSpotObject.transform.position = Vector3.zero;

        // Standard-Setup
        SetFishingSpotCapacity(40f, fishingSpot);
        SetFishingSpotRequiredLevel(1, fishingSpot);

        var fish = ScriptableObject.CreateInstance<Fish>();
        var fishes = new List<Fish> { fish };
        SetAvailableFishes(fishes, fishingSpot);

        var interactionTiles = new List<Vector3>
        {
            new(0, 0, 0),
            new(1, 0, 0),
            new(-1, 0, 0)
        };
        SetInteractionTiles(interactionTiles, fishingSpot);

        return fishingSpotObject;
    }

    private void SetFishingSpotCapacity(float capacity, FishingSpot fishingSpot)
    {
        var field = typeof(FishingSpot).GetField("_fishingCapacity",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(fishingSpot, capacity);
    }

    private void SetFishingSpotRequiredLevel(int level, FishingSpot fishingSpot)
    {
        var field = typeof(FishingSpot).GetField("_requiredLevelToInteract",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(fishingSpot, level);
    }

    private void SetAvailableFishes(List<Fish> fishes, FishingSpot fishingSpot)
    {
        var field = typeof(FishingSpot).GetField("_possibleFishesToCatch",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(fishingSpot, fishes);
    }

    private void SetInteractionTiles(List<Vector3> tiles, FishingSpot fishingSpot)
    {
        var field = typeof(FishingSpot).GetField("_interactionTiles",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(fishingSpot, tiles);
    }
}