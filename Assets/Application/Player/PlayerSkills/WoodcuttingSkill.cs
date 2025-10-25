using System;
using UnityEngine;

public class WoodcuttingSkill : Skill
{
    private PlayerSkills _playerSkills;

    private float _currentXP = 0;
    private int _currentLevel = 1;

    private float _requiredXPForLevelUp = 85;
    private readonly int _requiredXPIncreasePerLevel = 10; //In percantage 

    private readonly float _damageMultiplerPerLevel = 0.5f;


    public WoodcuttingSkill(PlayerSkills skill)
    {
        _playerSkills = skill;

        _playerSkills.WoodcuttingLevel = _currentLevel;
        _playerSkills.WoodcuttingCurrentEP = _currentLevel;
        _playerSkills.WoodcuttingBonusDamage = _currentLevel * _damageMultiplerPerLevel;
        _playerSkills.WoodcuttingNeededEP = _requiredXPForLevelUp;
    }

    public void IncreaseWoodcuttingXP(float amount)
    {
        _currentXP = this.IncreaseXP(amount, _currentXP);

        if (this.IsNewLevelReached(_currentXP, _requiredXPForLevelUp))
            UpdatePlayerSkills(true);
        else
            UpdatePlayerSkills(false);

        ShowFloatingXP(amount);
    }

    private void UpdatePlayerSkills(bool isLevelUp)
    {
        _playerSkills.WoodcuttingCurrentEP = _currentXP;

        if (isLevelUp)
        {
            _requiredXPForLevelUp += (_requiredXPForLevelUp / 100) * _requiredXPIncreasePerLevel;
            _currentLevel++;

            _playerSkills.WoodcuttingLevel = _currentLevel;
            _playerSkills.WoodcuttingCurrentEP = _currentXP;
            _playerSkills.WoodcuttingBonusDamage = _currentLevel * _damageMultiplerPerLevel;
            _playerSkills.WoodcuttingNeededEP = _requiredXPForLevelUp;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        _playerSkills.XPBar.DisplayXPBar(SkillType.Woodcutting,
                                        _currentXP.ToString(),
                                        _requiredXPForLevelUp.ToString());
    }

    private void ShowFloatingXP(float xpAmount)
    {
        if (FloatingXPController.Instance != null)
        {
            // Zeige schwebenden XP-Text �ber dem Spieler
            FloatingXPController.Instance.ShowXPGain(
                SkillType.Woodcutting,
                xpAmount,
                _playerSkills.transform
            );
        }
    }
}