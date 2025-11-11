using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToolItem", menuName = "ScriptableObjects/Items/Tool Item")]
public class ToolItem : Item, ISkillTool
{
    [Header("Tool Properties")]
    [SerializeField] private SkillType _requiredSkill;
    [SerializeField] private int _requiredLevel;
    [SerializeField] private float _efficiencyBonus = 1.0f;
    [SerializeField] private float _durabilityMax = 100f;
    [SerializeField] private SkillRequirement[] _additionalSkillRequirements;

    public SkillType RequiredSkill => _requiredSkill;
    public int RequiredLevel => _requiredLevel;
    public float EfficiencyBonus => _efficiencyBonus;
    public float DurabilityMax => _durabilityMax;

    public bool CanPlayerUseForSkill(SkillType skill, int playerLevel)
    {
        if (_requiredSkill == skill && playerLevel >= _requiredLevel)
            return true;

        // Check if Items has multiple purposes
        foreach (var requirement in _additionalSkillRequirements)
        {
            if (requirement.Skill == skill && playerLevel >= requirement.RequiredLevel)
                return true;
        }

        return false;
    }

    public SkillRequirement[] GetAllSkillRequirements()
    {
        var allRequirements = new List<SkillRequirement>
        {
            new() { Skill = _requiredSkill, RequiredLevel = _requiredLevel }
        };

        if (_additionalSkillRequirements != null)
            allRequirements.AddRange(_additionalSkillRequirements);

        return allRequirements.ToArray();
    }
}