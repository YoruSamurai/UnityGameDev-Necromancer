using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    // 可选：保存技能数据
    public SkillSO skillData;

    // 存放所有技能组件（继承自 BaseSkillComponent）的引用
    [SerializeField] private BaseSkillComponent[] skillComponents;

    private void Awake()
    {
        // 获取所有挂在这个技能Prefab上的技能组件
        skillComponents = GetComponents<BaseSkillComponent>();
    }

    /// <summary>
    /// 触发技能执行。各个组件会根据各自的逻辑执行。
    /// </summary>
    public void CastSkill(Player player, PlayerStats playerStats)
    {
        foreach (BaseSkillComponent comp in skillComponents)
        {
            // 这里可以根据需求判断是否需要检查CanExecute
            if (comp.CanExecute(player, playerStats, skillData))
            {
                comp.Execute(player, playerStats, skillData);
            }
        }
        // 也可以统一在这里做技能结束的处理，比如启动冷却计时
    }
}
