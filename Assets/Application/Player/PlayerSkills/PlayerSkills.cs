using System;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    [Header("UI")]
    public XPBar XPBar;

    [Header("Woodcutting Skill")]
    private WoodcuttingSkill _woodcuttingSkill;
    public int WoodcuttingLevel;
    public float WoodcuttingCurrentEP;
    public float WoodcuttingBonusDamage;
    public float WoodcuttingNeededEP;

    private void Awake()
    {
        _woodcuttingSkill = new(this);
    }

    public WoodcuttingSkill GetWoodcuttingSkill()
        => _woodcuttingSkill;

    public void IncreaseWoodcuttingXP(float amount)
    {
        _woodcuttingSkill.IncreaseWoodcuttingXP(amount);
    }
}