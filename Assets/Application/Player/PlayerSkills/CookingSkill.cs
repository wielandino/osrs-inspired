using UnityEngine;

public class CookingSkill : PlayerSkill
{
    private SkillType _skillType;

    [SerializeField]
    private float _currentXP = 0;

    public int CurrentLevel { private set; get; } = 1;

    [SerializeField]
    private float _requiredXPForLevelUp = 85;

    private readonly int _requiredXPIncreasePerLevel = 10; //In percantage 

    private void Start()
    {
        _skillType = SkillType.Cooking;
    }
    
    public void IncreaseCookingXP(float amount)
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
            _requiredXPForLevelUp += _requiredXPForLevelUp / 100 * _requiredXPIncreasePerLevel;
        }

        this.UpdateUI(_skillType, currentXP: _currentXP, requiredXPForLevelUp: _requiredXPForLevelUp);
    }
}