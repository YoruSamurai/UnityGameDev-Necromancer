using UnityEngine;

public abstract class BaseSkillComponent : MonoBehaviour, ISkillAction
{
    /// <summary>
    /// 技能前置检查（比如消耗、冷却检测等）
    /// </summary>
    public virtual bool CanExecute(Player player, PlayerStats playerStats, SkillSO skillData)
    {
        return true; // 默认允许施放
    }

    /// <summary>
    /// 执行技能核心逻辑（让子类去具体实现）
    /// </summary>
    public abstract void Execute(Player player, PlayerStats playerStats, SkillSO skillData);

    /// <summary>
    /// 技能结束后的处理（比如进入冷却、打断状态等）
    /// </summary>
    public virtual void OnSkillEnd(Player player, SkillSO skillData)
    {
        Debug.Log($"{skillData.skillName} 结束");
    }

    /// <summary>
    /// 通用技能启动时的效果（可以被子类覆盖）
    /// </summary>
    public virtual void OnSkillStart(Player player, SkillSO skillData)
    {
        Debug.Log($"{skillData.skillName} 启动");
    }
}