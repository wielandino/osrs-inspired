public interface ISkillTool
{
    public SkillType RequiredSkill { get; }
    public int RequiredLevel { get; }
    public float EfficiencyBonus { get; }
    public bool CanPlayerUseForSkill(SkillType skill, int playerLevel);
}