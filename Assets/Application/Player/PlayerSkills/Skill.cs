using UnityEngine;

public abstract class Skill
{
    protected float IncreaseXP(float amount, float currentXP)
        => currentXP + amount;

    protected bool IsNewLevelReached(float currentXP, float requiredXP)
        => currentXP >= requiredXP;

}