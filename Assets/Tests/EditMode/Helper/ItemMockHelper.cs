using UnityEngine;

public static class ItemMockHelper
{
    public static ToolItem GetFishingRodLevel1()
    {
        var fishingRod = ScriptableObject.CreateInstance<ToolItem>();

        var requiredSkillField = typeof(ToolItem).GetField("_requiredSkill",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        requiredSkillField?.SetValue(fishingRod, SkillType.Fishing);

        var requiredLevelField = typeof(ToolItem).GetField("_requiredLevel", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        requiredLevelField?.SetValue(fishingRod, 1);

        return fishingRod;
    }
}