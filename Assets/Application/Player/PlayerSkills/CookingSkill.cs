using UnityEngine;

public class CookingSkill : PlayerSkill
{
    private SkillType _skillType;

    [SerializeField]
    private float _currentXP = 0;

    public int CurrentLevel { private set; get; } = 1;

    [SerializeField]
    private float _requiredXPForLevelUp = 85;   

    [SerializeField]
    private float _chanceToCookPerLevel = 0.30f;

    [SerializeField]
    private float _increaseChanceToCookPerLevel = 0.5f;

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
            _requiredXPForLevelUp = GetNewRequiredXPForSkill(_requiredXPForLevelUp);
            _chanceToCookPerLevel += _increaseChanceToCookPerLevel;
        }

        this.UpdateUI(_skillType, currentXP: _currentXP, requiredXPForLevelUp: _requiredXPForLevelUp);
    }

    public float GetChanceToCookPerLevel()
        => _chanceToCookPerLevel;
}