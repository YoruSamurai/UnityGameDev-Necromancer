public interface ISkillAction
{
    /// <summary>
    /// 执行技能动作
    /// </summary>
    /// <param name="player">施放技能的玩家</param>
    /// <param name="skillData">技能数据（可以传入 ScriptableObject 数据）</param>
    void Execute(Player player, PlayerStats playerStats,SkillSO skillData);
}