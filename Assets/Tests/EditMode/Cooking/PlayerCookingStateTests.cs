using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class PlayerCookingStateTests
{
    private PlayerMockHelper _playerMock;
    private GameObject _treeLogObject;
    private TreeLog _treeLog;
    private PlayerCookingState _cookingState;

    [SetUp]
    public void SetUp()
    {
        // Player Mock
        _playerMock = new PlayerMockHelper();

        // TreeLog
        _treeLogObject = new GameObject("TestTreeLog");
        _treeLog = _treeLogObject.AddComponent<TreeLog>();
        
        // StateManager für TreeLog mocken
        var stateManager = _treeLogObject.AddComponent<TreeLogStateManager>();
        SetTreeLogStateManager(_treeLog, stateManager);
        SetTreeLogBurningState(stateManager, true);

        // CookingState
        _cookingState = new PlayerCookingState();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_treeLogObject);
        if (_playerMock.GetPlayerGameObject() != null)
            Object.DestroyImmediate(_playerMock.GetPlayerGameObject());
    }

    [Test]
    public void CollectAllCookableItemsFromPlayerInventory_Should_Find_All_Same_Type_Items()
    {
        // Arrange
        var rawFish = CreateTestCookableItem("Raw Fish", 1);
        
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(rawFish);
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(rawFish);
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(rawFish);
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(rawFish);

        _cookingState.SetTargetTreeLogAndItemToCook(_treeLog, rawFish, _playerMock.GetPlayerStateManager());

        // Act
        var itemsToCook = GetItemsToCookOfSameType(_cookingState);

        // Assert
        Assert.AreEqual(4, itemsToCook.Count);
        Assert.IsTrue(itemsToCook.All(x => x == rawFish));
    }

    [Test]
    public void CollectAllCookableItemsFromPlayerInventory_Should_Only_Find_Matching_Items()
    {
        // Arrange
        var rawFish = CreateTestCookableItem("Raw Fish", 1);
        var rawMeat = CreateTestCookableItem("Raw Meat", 1);
        
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(rawFish);
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(rawFish);
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(rawMeat);
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(rawMeat);

        _cookingState.SetTargetTreeLogAndItemToCook(_treeLog, rawFish, _playerMock.GetPlayerStateManager());

        // Act
        var itemsToCook = GetItemsToCookOfSameType(_cookingState);

        // Assert
        Assert.AreEqual(2, itemsToCook.Count);
        Assert.IsTrue(itemsToCook.All(x => x == rawFish));
    }

    [Test]
    public void CollectAllCookableItemsFromPlayerInventory_Should_Return_Empty_When_No_Items()
    {
        // Arrange
        var rawFish = CreateTestCookableItem("Raw Fish", 1);
        
        _cookingState.SetTargetTreeLogAndItemToCook(_treeLog, rawFish, _playerMock.GetPlayerStateManager());

        // Act
        var itemsToCook = GetItemsToCookOfSameType(_cookingState);

        // Assert
        Assert.AreEqual(0, itemsToCook.Count);
    }

    [Test]
    public void CollectAllCookableItemsFromPlayerInventory_Should_Ignore_Non_Cookable_Items()
    {
        // Arrange
        var rawFish = CreateTestCookableItem("Raw Fish", 1);
        var normalItem = ScriptableObject.CreateInstance<Item>();
        
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(rawFish);
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(normalItem);
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(rawFish);

        _cookingState.SetTargetTreeLogAndItemToCook(_treeLog, rawFish, _playerMock.GetPlayerStateManager());

        // Act
        var itemsToCook = GetItemsToCookOfSameType(_cookingState);

        // Assert
        Assert.AreEqual(2, itemsToCook.Count);
    }

    [Test]
    public void CalculateCookChance_Should_Return_Correct_Value()
    {
        // Arrange
        float baseChance = 0.7f;     
        float itemDifficulty = 0.2f;  
        
        // Expected: 0.7 - 0.2 = 0.5 (50%)

        // Act
        float result = _cookingState.CalculateCookChance(baseChance, itemDifficulty);

        // Assert
        Assert.AreEqual(0.5f, result, 0.001f);
    }

    [Test]
    public void CalculateCookChance_Should_Clamp_Between_0_And_1()
    {
        // Too high
        float tooHigh = _cookingState.CalculateCookChance(1.5f, 0f);
        Assert.AreEqual(1f, tooHigh);

        // Too low
        float tooLow = _cookingState.CalculateCookChance(0.1f, 0.5f);
        Assert.AreEqual(0f, tooLow);
    }

    [Test]
    public void CalculateCookChance_HighSkill_Should_Give_High_Chance()
    {
        // Arrange
        float baseChance = 0.95f;
        float itemDifficulty = 0.05f;

        // Act
        float result = _cookingState.CalculateCookChance(baseChance, itemDifficulty);

        // Assert
        Assert.Greater(result, 0.85f);
        Assert.AreEqual(0.9f, result, 0.001f);
    }

    [Test]
    public void CalculateCookChance_LowSkill_Should_Give_Low_Chance()
    {
        // Arrange
        float baseChance = 0.1f;
        float itemDifficulty = 0.3f;

        // Act
        float result = _cookingState.CalculateCookChance(baseChance, itemDifficulty);

        // Assert
        Assert.Less(result, 0.1f);
        Assert.AreEqual(0f, result, 0.001f);
    }

    [Test]
    public void TryCookingAttempt_Should_Return_True_Or_False_Based_On_Chance()
    {
        // Arrange
        Random.InitState(12345);
        
        var cookableItem = CreateTestCookableItem("Raw Fish", 1);
        SetCookableItemDifficulty(cookableItem, 0.1f);
        
        var cookedFish = ScriptableObject.CreateInstance<Item>();
        SetCookableItemReturnItem(cookableItem, cookedFish);

        int successCount = 0;
        int totalAttempts = 100;
        
        for (int i = 0; i < totalAttempts; i++)
        {
            if (_cookingState.TryCookingAttempt(_playerMock.GetPlayerStateManager(), cookableItem))
                successCount++;
        }

        // Assert
        Assert.Greater(successCount, 0);
        Assert.Less(successCount, totalAttempts);
    }

    [Test]
    public void TryCookingAttempt_With_Perfect_Chance_Should_Always_Succeed()
    {
        // Arrange
        var cookableItem = CreateTestCookableItem("Easy Fish", 1);
        SetCookableItemDifficulty(cookableItem, 0f); // Keine Schwierigkeit
        
        var cookingSkill = _playerMock.GetPlayerStateManager().PlayerSkills.GetCookingSkill();
        SetSkillChance(cookingSkill, 1.0f);

        // Act
        int successCount = 0;
        for (int i = 0; i < 20; i++)
        {
            if (_cookingState.TryCookingAttempt(_playerMock.GetPlayerStateManager(), cookableItem))
                successCount++;
        }

        // Assert
        Assert.AreEqual(20, successCount);
    }

    [Test]
    public void TryCookingAttempt_With_Zero_Chance_Should_Always_Fail()
    {
        // Arrange
        var cookableItem = CreateTestCookableItem("Impossible Fish", 1);
        SetCookableItemDifficulty(cookableItem, 1.0f);
        
        var cookingSkill = _playerMock.GetPlayerStateManager().PlayerSkills.GetCookingSkill();
        SetSkillChance(cookingSkill, 0f);

        // Act
        int successCount = 0;
        for (int i = 0; i < 20; i++)
        {
            if (_cookingState.TryCookingAttempt(_playerMock.GetPlayerStateManager(), cookableItem))
                successCount++;
        }

        // Assert
        Assert.AreEqual(0, successCount);
    }

    // Helper Methods
    private CookableItem CreateTestCookableItem(string name, int requiredLevel)
    {
        var item = ScriptableObject.CreateInstance<CookableItem>();
        SetItemName(item, name);
        SetCookableItemRequiredLevel(item, requiredLevel);
        SetCookableItemDifficulty(item, 0.1f);
        
        var returnItem = ScriptableObject.CreateInstance<Item>();
        SetCookableItemReturnItem(item, returnItem);
        
        return item;
    }

    private void SetItemName(Item item, string name)
    {
        var field = typeof(Item).GetField("Name",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        field?.SetValue(item, name);
    }

    private void SetCookableItemRequiredLevel(CookableItem item, int level)
    {
        var field = typeof(CookableItem).GetField("RequiredCookingLevel",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        field?.SetValue(item, level);
    }

    private void SetCookableItemDifficulty(CookableItem item, float difficulty)
    {
        var field = typeof(CookableItem).GetField("IncreasedFailureToCook",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        field?.SetValue(item, difficulty);
    }

    private void SetCookableItemReturnItem(CookableItem item, Item returnItem)
    {
        var field = typeof(CookableItem).GetField("ReturnItem",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        field?.SetValue(item, returnItem);
    }

    private void SetTreeLogStateManager(TreeLog treeLog, TreeLogStateManager stateManager)
    {
        var field = typeof(TreeLog).GetField("_stateManager",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(treeLog, stateManager);
    }

    private void SetTreeLogBurningState(TreeLogStateManager stateManager, bool isBurning)
    {
        var field = typeof(TreeLogStateManager).GetField("_isBurning",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(stateManager, isBurning);
    }

    private List<CookableItem> GetItemsToCookOfSameType(PlayerCookingState cookingState)
    {
        var field = typeof(PlayerCookingState).GetField("_itemsToCookOfSameType",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field?.GetValue(cookingState) as List<CookableItem>;
    }

    private void SetSkillChance(CookingSkill cookingSkill, float chance)
    {

        var field = typeof(CookingSkill).GetField("_chanceToCookPerLevel",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(cookingSkill, chance);
    }
}