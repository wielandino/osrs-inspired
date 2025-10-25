using System.Collections.Generic;
using UnityEngine;

public class ToolValidator
{
    public static bool CanPlayerUseTool(Item item, SkillType skill, PlayerSkills playerSkills)
    {
        if (item is not ISkillTool skillTool)
            return false;

        int playerLevel = GetPlayerSkillLevel(skill, playerSkills);
        return skillTool.CanUseForSkill(skill, playerLevel);
    }

    public static ISkillTool GetBestToolForSkill(List<Item> inventory, SkillType skill, PlayerSkills playerSkills)
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

    private static int GetPlayerSkillLevel(SkillType skill, PlayerSkills playerSkills)
    {
        return skill switch
        {
            SkillType.Woodcutting => playerSkills.WoodcuttingLevel,
            // Weitere Skills hier hinzuf�gen
            _ => 1
        };
    }
}