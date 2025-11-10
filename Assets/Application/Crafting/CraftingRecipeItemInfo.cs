using System;

[Serializable]
public class CraftingRecipeItemInfo
{
    public Item ItemToCombine;
    public bool ShouldDeleteItemAfterCraft;
    public bool ShouldDeleteItemAfterFail = true;
}