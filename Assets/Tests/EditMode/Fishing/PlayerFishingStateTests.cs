using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class PlayerFishingStateTests
{
    private PlayerMockHelper _playerMock;
    private GameObject _fishingSpotObject;
    private FishingSpot _fishingSpot;
    private PlayerFishingState _fishingState;

    [SetUp]
    public void SetUp()
    {
        // Player Mock
        _playerMock = new PlayerMockHelper();

        // FishingSpot
        _fishingSpotObject = new GameObject("TestFishingSpot");
        _fishingSpot = _fishingSpotObject.AddComponent<FishingSpot>();
        
        SetFishingSpotCapacity(40f);
        
        var fish = ScriptableObject.CreateInstance<Fish>();
        SetFishProperty(fish, "IncreasedFailureToCatch", 0.1f);
        SetFishProperty(fish, "FishingSpotCapacityDrain", 1f);
        
        var fishes = new List<Fish> { fish };
        SetAvailableFishes(fishes);

        // FishingState
        _fishingState = new PlayerFishingState();
        _fishingState.SetFishingSpot(_fishingSpot);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_fishingSpotObject);
        if (_playerMock.GetPlayerGameObject() != null)
            Object.DestroyImmediate(_playerMock.GetPlayerGameObject());
    }

    [Test]
    public void GetFishingRod_Should_Return_SelectedItem_When_Valid()
    {
        // Arrange
        var fishingRod = ItemMockHelper.GetFishingRodLevel1();
        _playerMock.GetPlayerStateManager().PlayerInventory.SelectedItem = fishingRod;

        // Act
        var result = _fishingState.GetFishingRod(_playerMock.GetPlayerStateManager());

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(fishingRod, result);
    }

    [Test]
    public void GetFishingRod_Should_Return_Null_When_SelectedItem_Invalid()
    {
        // Arrange
        var invalidItem = ScriptableObject.CreateInstance<Item>();
        _playerMock.GetPlayerStateManager().PlayerInventory.SelectedItem = invalidItem;

        // Act
        var result = _fishingState.GetFishingRod(_playerMock.GetPlayerStateManager());

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void GetFishingRod_Should_Return_BestTool_When_NoItemSelected()
    {
        // Arrange
        var fishingRod = ItemMockHelper.GetFishingRodLevel1();
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(fishingRod);
        _playerMock.GetPlayerStateManager().PlayerInventory.SelectedItem = null;

        // Act
        var result = _fishingState.GetFishingRod(_playerMock.GetPlayerStateManager());

        // Assert
        Assert.IsNotNull(result);
    }

    [Test]
    public void CalculateCatchChance_Should_Return_Correct_Value()
    {
        // Arrange
        float baseChance = 0.5f;      // 50% Base
        float fishDifficulty = 0.1f;  // -10%
        float toolBonus = 0.2f;       // +20%
        
        // Expected: 0.5 - 0.1 + 0.2 = 0.6 (60%)

        // Act
        float result = _fishingState.CalculateCatchChance(baseChance, fishDifficulty, toolBonus);

        // Assert
        Assert.AreEqual(0.6f, result, 0.001f);
    }

    [Test]
    public void CalculateCatchChance_Should_Clamp_Between_0_And_1()
    {
        float tooHigh = _fishingState.CalculateCatchChance(1.5f, 0f, 0.5f); // 2.0
        Assert.AreEqual(1f, tooHigh, "Sollte auf 1.0 clampen");

        float tooLow = _fishingState.CalculateCatchChance(0.1f, 0.5f, 0f); // -0.4
        Assert.AreEqual(0f, tooLow, "Sollte auf 0.0 clampen");
    }

    [Test]
    public void CalculateCatchChance_HighSkill_HighTool_Should_Give_High_Chance()
    {
        // Arrange
        float baseChance = 0.99f;
        float fishDifficulty = 0.05f;
        float toolBonus = 0.15f;

        // Act
        float result = _fishingState.CalculateCatchChance(baseChance, fishDifficulty, toolBonus);

        // Assert
        Assert.Greater(result, 0.9f);
    }

    [Test]
    public void CalculateCatchChance_LowSkill_NoTool_Should_Give_Low_Chance()
    {
        // Arrange
        float baseChance = 0.01f;
        float fishDifficulty = 0.3f;
        float toolBonus = 0f;

        // Act
        float result = _fishingState.CalculateCatchChance(baseChance, fishDifficulty, toolBonus);

        // Assert
        Assert.Less(result, 0.1f);
    }

    [Test]
    public void TryFishingAttempt_Should_Return_False_When_Spot_Empty()
    {
        // Arrange
        SetFishingSpotCapacity(0f);
        
        var tool = ItemMockHelper.GetFishingRodLevel1();
        var fish = CreateTestFish();

        // Act
        bool success = _fishingState.TryFishingAttempt(_playerMock.GetPlayerStateManager(),
                                                       tool,
                                                       fish,
                                                       out bool spotIsEmpty);

        // Assert
        Assert.IsFalse(success);
        Assert.IsTrue(spotIsEmpty);
    }

    [Test]
    public void TryFishingAttempt_Should_Add_Fish_To_Inventory_On_Success()
    {
        // Arrange
        Random.InitState(12345);
        
        var tool = ItemMockHelper.GetFishingRodLevel1();
        var fish = CreateTestFish();
        
        var inventory = _playerMock.GetPlayerStateManager().PlayerInventory;

        // Act - mehrere Versuche für statistischen Test
        int successCount = 0;
        for (int i = 0; i < 100; i++)
        {
            if (_fishingState.TryFishingAttempt(_playerMock.GetPlayerStateManager(), tool, fish, out _))
                successCount++;
            
        }

        int fishesInInventory = inventory.GetItems().Where(x => x == fish).Count();

        // Assert
        Assert.Greater(successCount, 0);
        Assert.AreEqual(fishesInInventory, successCount);
    }

    [Test]
    public void TryFishingAttempt_Should_Reduce_Spot_Capacity_On_Success()
    {
        // Arrange
        Random.InitState(99999);
        
        float initialCapacity = 40f;
        SetFishingSpotCapacity(initialCapacity);
        
        var tool = ItemMockHelper.GetFishingRodLevel1();
        var fish = CreateTestFish();
        SetFishProperty(fish, "FishingSpotCapacityDrain", 5f);

        // Act
        bool hadSuccess = false;
        float capacityAfter = initialCapacity;
        
        for (int i = 0; i < 100; i++)
        {
            if (_fishingState.TryFishingAttempt(_playerMock.GetPlayerStateManager(), tool, fish, out _))
            {
                hadSuccess = true;
                capacityAfter = _fishingSpot.GetFishingCapacity();
                break;
            }
        }

        // Assert
        Assert.IsTrue(hadSuccess);
        Assert.Less(capacityAfter, initialCapacity);
    }

    [Test]
    public void SelectRandomFish_Should_Return_Fish_From_Available_List()
    {
        // Arrange
        var fish1 = CreateTestFish();
        var fish2 = CreateTestFish();
        var availableFishes = new List<Fish> { fish1, fish2 };
        SetAvailableFishes(availableFishes);

        // Act
        var selectedFish = _fishingState.SelectRandomFish();

        // Assert
        Assert.IsNotNull(selectedFish);
        Assert.IsTrue(availableFishes.Contains(selectedFish));
    }

    [Test]
    public void SelectRandomFish_Should_Eventually_Select_All_Fishes()
    {
        // Arrange
        var fish1 = CreateTestFish();
        var fish2 = CreateTestFish();
        var fish3 = CreateTestFish();
        var availableFishes = new List<Fish> { fish1, fish2, fish3 };
        SetAvailableFishes(availableFishes);

        var selectedFishes = new HashSet<Fish>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            var selectedFish = _fishingState.SelectRandomFish();
            selectedFishes.Add(selectedFish);
        }

        // Assert
        Assert.AreEqual(3, selectedFishes.Count);
    }

    private Fish CreateTestFish()
    {
        var fish = ScriptableObject.CreateInstance<Fish>();
        SetFishProperty(fish, "IncreasedFailureToCatch", 0.1f);
        SetFishProperty(fish, "FishingSpotCapacityDrain", 1f);
        return fish;
    }

    private void SetFishProperty(Fish fish, string propertyName, float value)
    {
        var field = typeof(Fish).GetField(propertyName,
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public);
        field?.SetValue(fish, value);
    }

    private void SetFishingSpotCapacity(float capacity)
    {
        var field = typeof(FishingSpot).GetField("_fishingCapacity",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(_fishingSpot, capacity);
    }

    private void SetAvailableFishes(List<Fish> fishes)
    {
        var field = typeof(FishingSpot).GetField("_possibleFishesToCatch",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(_fishingSpot, fishes);
    }
}