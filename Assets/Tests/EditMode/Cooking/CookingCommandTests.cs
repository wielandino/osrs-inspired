using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CookingCommandTests
{
    private PlayerMockHelper _playerMock;
    private GameObject _treeLogObject;
    private TreeLog _treeLog;
    private List<GameObject> _testObjects = new List<GameObject>();

    [SetUp]
    public void SetUp()
    {
        _playerMock = new();

        _treeLogObject = CreateTreeLog();
        _treeLog = _treeLogObject.GetComponent<TreeLog>();
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
        
        if (_playerMock != null && _playerMock.GetPlayerGameObject() != null)
            Object.DestroyImmediate(_playerMock.GetPlayerGameObject());
    }

    [Test]
    public void Command_Should_Return_False_When_TreeLog_Not_Burning()
    {
        // Arrange
        _playerMock.GetPlayerGameObject().transform.position = new(1, 0, 0);
        
        var cookableItem = CreateTestCookableItem("Raw Fish", 1);
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(cookableItem);
        
        SetTreeLogBurning(false);
        
        var cookingCommand = new CookingCommand(_treeLog, cookableItem);

        // Act
        bool canExecute = cookingCommand.CanExecute(_playerMock.GetPlayerStateManager(), out CommandErrorCode errorCode);

        // Assert
        Assert.IsFalse(canExecute);
        Assert.AreEqual(CommandErrorCode.NoTarget, errorCode);
    }

    [Test]
    public void Command_Should_Return_False_When_Player_Not_In_IdleState()
    {
        // Arrange
        _playerMock.GetPlayerGameObject().transform.position = new(1, 0, 0);
        
        var cookableItem = CreateTestCookableItem("Raw Fish", 1);
        
        // Player in anderen State versetzen (z.B. Fishing State)
        SetPlayerState(_playerMock.GetPlayerStateManager(), false);
        
        var cookingCommand = new CookingCommand(_treeLog, cookableItem);

        // Act
        bool canExecute = cookingCommand.CanExecute(_playerMock.GetPlayerStateManager(), out CommandErrorCode errorCode);

        // Assert
        Assert.IsFalse(canExecute);
        Assert.AreEqual(CommandErrorCode.PlayerNotInIdleState, errorCode);
    }

    [Test]
    public void Command_Should_Return_False_When_Item_Has_No_ReturnItem()
    {
        // Arrange
        _playerMock.GetPlayerGameObject().transform.position = new(1, 0, 0);
        
        var cookableItem = CreateTestCookableItem("Raw Fish", 1);
        SetCookableItemReturnItem(cookableItem, null); // Kein Return Item!
        
        var cookingCommand = new CookingCommand(_treeLog, cookableItem);

        // Act
        bool canExecute = cookingCommand.CanExecute(_playerMock.GetPlayerStateManager(), out CommandErrorCode errorCode);

        // Assert
        Assert.IsFalse(canExecute);
        Assert.AreEqual(CommandErrorCode.FatalError, errorCode);
    }

    [Test]
    public void Command_Should_Return_False_When_Player_Not_In_InteractionTile()
    {
        // Arrange
        _playerMock.GetPlayerGameObject().transform.position = new(100, 0, 100); // Weit weg!
        
        var cookableItem = CreateTestCookableItem("Raw Fish", 1);
        
        var cookingCommand = new CookingCommand(_treeLog, cookableItem);

        // Act
        bool canExecute = cookingCommand.CanExecute(_playerMock.GetPlayerStateManager(), out CommandErrorCode errorCode);

        // Assert
        Assert.IsFalse(canExecute);
        Assert.AreEqual(CommandErrorCode.PlayerNotInInteractionTile, errorCode);
    }

    [Test]
    public void Command_Should_Return_False_When_Player_Level_Too_Low()
    {
        // Arrange
        _playerMock.GetPlayerGameObject().transform.position = new(1, 0, 0);
        
        var cookableItem = CreateTestCookableItem("Master Fish", 99); // Benötigt Level 99!
        
        // Player hat nur Level 1
        SetPlayerCookingLevel(_playerMock.GetPlayerStateManager(), 1);
        
        var cookingCommand = new CookingCommand(_treeLog, cookableItem);

        // Act
        bool canExecute = cookingCommand.CanExecute(_playerMock.GetPlayerStateManager(), out CommandErrorCode errorCode);

        // Assert
        Assert.IsFalse(canExecute);
        Assert.AreEqual(CommandErrorCode.PlayerSkillRequirementNotMet, errorCode);
    }

    [Test]
    public void Command_Should_Execute_Successfully_When_All_Conditions_Met()
    {
        // Arrange
        _playerMock.GetPlayerGameObject().transform.position = new(1, 0, 0);
        
        var cookableItem = CreateTestCookableItem("Raw Fish", 1);
        _playerMock.GetPlayerStateManager().PlayerInventory.AddItem(cookableItem);
        
        SetPlayerCookingLevel(_playerMock.GetPlayerStateManager(), 10);
        
        var cookingCommand = new CookingCommand(_treeLog, cookableItem);

        // Act
        bool canExecute = cookingCommand.CanExecute(_playerMock.GetPlayerStateManager(), out CommandErrorCode errorCode);

        // Assert
        Assert.IsTrue(canExecute);
        Assert.AreEqual(CommandErrorCode.Default, errorCode);
    }

    [Test]
    public void Command_Should_Execute_When_Player_On_Different_Valid_Tiles()
    {
        // Arrange
        var validTiles = new List<Vector3>
        {
            new(0, 0, 0),
            new(1, 0, 0),
            new(-1, 0, 0)
        };

        var cookableItem = CreateTestCookableItem("Raw Fish", 1);
        SetPlayerCookingLevel(_playerMock.GetPlayerStateManager(), 10);

        foreach (var tile in validTiles)
        {
            // Arrange
            _playerMock.GetPlayerGameObject().transform.position = tile;
            var cookingCommand = new CookingCommand(_treeLog, cookableItem);

            // Act
            bool canExecute = cookingCommand.CanExecute(_playerMock.GetPlayerStateManager(), out CommandErrorCode errorCode);

            // Assert
            Assert.IsTrue(canExecute);
        }
    }

    // Helper Methods
    private GameObject CreateTreeLog()
    {
        var treeLogObject = new GameObject("TestTreeLog");
        _testObjects.Add(treeLogObject);
        
        var treeLog = treeLogObject.AddComponent<TreeLog>();
        treeLogObject.transform.position = Vector3.zero;

        // StateManager mocken
        var stateManager = treeLogObject.AddComponent<TreeLogStateManager>();
        SetTreeLogStateManager(treeLog, stateManager);
        SetTreeLogBurningState(stateManager, true); // Standard: brennend

        var interactionTiles = new List<Vector3>
        {
            new(0, 0, 0),
            new(1, 0, 0),
            new(-1, 0, 0)
        };
        SetInteractionTiles(interactionTiles, treeLog);

        return treeLogObject;
    }

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
        TreeLogBaseState state = stateManager.BurningState;

        if (!isBurning)
            state = stateManager.IdleState;

        var field = typeof(TreeLogStateManager).GetField("_currentState",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(stateManager, state);
    }

    private void SetTreeLogBurning(bool isBurning)
    {
        var stateManager = _treeLog.GetComponent<TreeLogStateManager>();
        SetTreeLogBurningState(stateManager, isBurning);
    }

    private void SetInteractionTiles(List<Vector3> tiles, TreeLog treeLog)
    {
        var field = typeof(TreeLog).GetField("InteractionTiles",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        field?.SetValue(treeLog, tiles);
    }

    private void SetPlayerState(PlayerStateManager player, bool isIdle)
    {
        PlayerBaseState state = player.IdleState;

        if (!isIdle)
            state = player.FishingState;

        var field = typeof(PlayerStateManager).GetField("_currentState",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(player, state);
    }

    private void SetPlayerCookingLevel(PlayerStateManager player, int level)
    {
        var cookingSkill = player.PlayerSkills.GetCookingSkill();
        var field = typeof(CookingSkill).GetField("CurrentLevel",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        field?.SetValue(cookingSkill, level);
    }
}