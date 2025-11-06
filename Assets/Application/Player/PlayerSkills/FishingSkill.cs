using UnityEngine;

public class FishingSkill : PlayerSkill
{
    private SkillType _skillType;

    [SerializeField]
    private float _currentXP = 0;

    public int CurrentLevel { private set; get; } = 1;

    [SerializeField]
    private float _requiredXPForLevelUp = 85;   

    [SerializeField]
    private float _chanceToCatchFishPerLevel = 0.30f;

    [SerializeField]
    private float _increaseChanceToCatchPerLevel = 0.5f;

    private void Start()
    {
        _skillType = SkillType.Fishing;
    }

    public void IncreaseFishingXP(float amount)
    {
        _currentXP = this.IncreaseXP(amount, _currentXP);

        if (this.IsNewLevelReached(_currentXP, _requiredXPForLevelUp))
            UpdatePlayerSkills(true);
        else
            UpdatePlayerSkills(false);

        this.ShowFloatingXP(_skillType, amount);
    }

    public float GetChanceToCatchFishPerLevel()
        => _chanceToCatchFishPerLevel;

    private void UpdatePlayerSkills(bool isLevelUp)
    {
        if (isLevelUp)
        {
            CurrentLevel++;
            _requiredXPForLevelUp = GetNewRequiredXPForSkill(_requiredXPForLevelUp);
            _chanceToCatchFishPerLevel += _increaseChanceToCatchPerLevel;
        }

        this.UpdateUI(_skillType, currentXP: _currentXP, requiredXPForLevelUp: _requiredXPForLevelUp);
    }
}