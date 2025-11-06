using UnityEngine;

public class WoodcuttingSkill : PlayerSkill
{
    [SerializeField]
    private float _currentXP = 0;

    public int CurrentLevel { private set; get; } = 1;

    [SerializeField]
    private float _requiredXPForLevelUp = 85;

    public float BonusDamage { private set; get; } = 0.5f;

    public readonly float DamageMultiplerPerLevel = 0.5f;

    private SkillType _skillType;

    private void Start()
    {
        _skillType = SkillType.Woodcutting;
    }

    public void IncreaseWoodcuttingXP(float amount)
    {
        _currentXP = this.IncreaseXP(amount, _currentXP);

        if (this.IsNewLevelReached(_currentXP, _requiredXPForLevelUp))
            UpdatePlayerSkills(true);
        else
            UpdatePlayerSkills(false);

        this.ShowFloatingXP(_skillType, amount);
    }

    private void UpdatePlayerSkills(bool isLevelUp)
    {
        if (isLevelUp)
        {
            CurrentLevel++;
            _requiredXPForLevelUp = GetNewRequiredXPForSkill(_requiredXPForLevelUp);
            BonusDamage = CurrentLevel * DamageMultiplerPerLevel;
        }

        this.UpdateUI(_skillType, currentXP: _currentXP, requiredXPForLevelUp: _requiredXPForLevelUp);
    }
}