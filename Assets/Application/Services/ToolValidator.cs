using System.Collections.Generic;

public class ToolValidator
{
    public static bool CanPlayerUseTool(Item item, SkillType skill, PlayerSkill playerSkills)
    {
        if (item is not ISkillTool skillTool)
            return false;

        int playerLevel = GetPlayerSkillLevel(skill, playerSkills);
        return skillTool.CanPlayerUseForSkill(skill, playerLevel);
    }

    public static bool CanToolBeUsedForSkill(Item item, SkillType requiredSkillType)
    {
        if (item is not ISkillTool skillTool)
            return false;

        return skillTool.RequiredSkill == requiredSkillType;
    }

    public static ISkillTool GetBestToolForSkill(List<Item> inventory, SkillType skill, PlayerSkill playerSkills)
    {
        ISkillTool bestTool = null;
        float bestEfficiency = 0f;

        foreach (var item in inventory)
        {
            if (item is ISkillTool tool && CanPlayerUseTool(item, skill, playerSkills))
            {
                if (tool.EfficiencyBonus > bestEfficiency)
                {
                    bestEfficiency = tool.EfficiencyBonus;
                    bestTool = tool;
                }
            }
        }

        return bestTool;
    }

    private static int GetPlayerSkillLevel(SkillType skill, PlayerSkill playerSkills)
    {
        return skill switch
        {
            SkillType.Woodcutting => playerSkills.GetWoodcuttingSkill().CurrentLevel,
            SkillType.Fishing => playerSkills.GetFishingSkill().CurrentLevel,
            SkillType.Cooking => playerSkills.GetCookingSkill().CurrentLevel,
            SkillType.Crafting => playerSkills.GetCraftingSkill().CurrentLevel,
            SkillType.Firemaking => playerSkills.GetFiremakingSkill().CurrentLevel,
            _ => 1
        };
    }
}