using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    public static PlayerSkill Instance;

    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnDisable()
    {
        if (Instance != null)
            Instance = null;
    }

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

    protected float GetNewRequiredXPForSkill(float currentRequiredXP)
        => Mathf.Ceil((currentRequiredXP / 4 + 300 * 2) / 7 + currentRequiredXP);
    

    protected float IncreaseXP(float amount, float currentXP)
        => currentXP + amount;

    protected bool IsNewLevelReached(float currentXP, float requiredXP)
        => currentXP >= requiredXP;

    public WoodcuttingSkill GetWoodcuttingSkill()
        => gameObject.GetComponent<WoodcuttingSkill>();

    public FiremakingSkill GetFiremakingSkill()
        => gameObject.GetComponent<FiremakingSkill>();

    public FishingSkill GetFishingSkill()
        => gameObject.GetComponent<FishingSkill>();

    public CookingSkill GetCookingSkill()
        => gameObject.GetComponent<CookingSkill>();

    public CraftingSkill GetCraftingSkill()
        => gameObject.GetComponent<CraftingSkill>();
}