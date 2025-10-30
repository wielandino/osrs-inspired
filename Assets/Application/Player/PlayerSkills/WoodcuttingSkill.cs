using System;
using UnityEngine;

public class WoodcuttingSkill : PlayerSkill
{
    [SerializeField]
    private float _currentXP = 0;

    [SerializeField]
    public int CurrentLevel { private set; get; } = 1;

    [SerializeField]
    private float _requiredXPForLevelUp = 85;

    [SerializeField]
    private readonly int _requiredXPIncreasePerLevel = 10; //In percantage 

    public float BonusDamage { private set; get; } = 0.5f;

    public readonly float DamageMultiplerPerLevel = 0.5f;

    public void IncreaseWoodcuttingXP(float amount)
    {
        _currentXP = this.IncreaseXP(amount, _currentXP);

        if (this.IsNewLevelReached(_currentXP, _requiredXPForLevelUp))
            UpdatePlayerSkills(true);
        else
            UpdatePlayerSkills(false);

        this.ShowFloatingXP(SkillType.Woodcutting, amount);
    }

    private void UpdatePlayerSkills(bool isLevelUp)
    {
        if (isLevelUp)
        {
            CurrentLevel++;
            _requiredXPForLevelUp += _requiredXPForLevelUp / 100 * _requiredXPIncreasePerLevel;

            BonusDamage = CurrentLevel * DamageMultiplerPerLevel;
        }

        this.UpdateUI(SkillType.Woodcutting, currentXP: _currentXP, requiredXPForLevelUp: _requiredXPForLevelUp);
    }
}