using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [SerializeField]
    private XPBar _xpBarUI;

    protected void UpdateUI(SkillType skillType, float currentXP, float requiredXPForLevelUp)
    {
        _xpBarUI.DisplayXPBar(skillType,
                                  currentXP.ToString(),
                                  requiredXPForLevelUp.ToString());
    }

    protected void ShowFloatingXP(SkillType skillType, float xpAmount)
    {
        if (FloatingXPController.Instance == null)
            return; 
        
        FloatingXPController.Instance.ShowXPGain(skillType, xpAmount, transform);
    }

    protected float IncreaseXP(float amount, float currentXP)
        => currentXP + amount;

    protected bool IsNewLevelReached(float currentXP, float requiredXP)
        => currentXP >= requiredXP;

    public WoodcuttingSkill GetWoodcuttingSkill()
        => gameObject.GetComponent<WoodcuttingSkill>();
}