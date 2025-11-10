public class CraftingCommand : PlayerCommandBase
{
    private CraftingRecipe _recipe;

    public CraftingCommand(Item itemOne, Item itemTwo)
    {
        _recipe = CraftingRecipeDatabase.Instance.FindRecipe(itemOne, itemTwo);
    }

    public override bool CanExecute(PlayerStateManager player)
        => CanExecute(player, out _);

    public bool CanExecute(PlayerStateManager player, out CommandErrorCode errorCode)
    {
        errorCode = CommandErrorCode.Default;

        if (!player.IsInIdleState())
        {
            errorCode = CommandErrorCode.PlayerNotInIdleState;
            return false;
        }

        if (_recipe == null)
        {
            errorCode = CommandErrorCode.NoTarget;
            return false;
        }

        if (_recipe.ReturnItem == null)
        {
            errorCode = CommandErrorCode.FatalError;
            return false;
        }

        if (player.PlayerSkills.GetCraftingSkill().CurrentLevel < _recipe.RequiredLevel)
        {
            errorCode = CommandErrorCode.PlayerSkillRequirementNotMet;
            return false;
        }

        if (!PlayerHasRequiredItems(player))
        {
            errorCode = CommandErrorCode.NoTarget;
            return false;
        }

        return true;
    }

    public override void ExecuteInternal(PlayerStateManager player)
    {
        player.SwitchToCraftingState(_recipe);
        _isComplete = true;
    }

    private bool PlayerHasRequiredItems(PlayerStateManager player)
    {
        var inventory = player.PlayerInventory.GetItems();
        
        bool hasItemOne = inventory.Contains(_recipe.PrimaryIngredient.ItemToCombine);
        bool hasItemTwo = inventory.Contains(_recipe.SecondaryIngredient.ItemToCombine);
        
        return hasItemOne && hasItemTwo;
    }
}