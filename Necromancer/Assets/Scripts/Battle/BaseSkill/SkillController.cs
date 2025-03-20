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
        if (AllComponentsCanExecute(player, playerStats))
        {
            foreach (BaseSkillComponent comp in skillComponents)
            {
                comp.Execute(player, playerStats, skillData);
            }
        }
        else
        {
            Debug.Log("⚠️ 技能条件不满足，无法施放");
        }
        // 也可以统一在这里做技能结束的处理，比如启动冷却计时
    }

    /// <summary>
    /// 判断所有组件是否都能执行
    /// </summary>
    private bool AllComponentsCanExecute(Player player, PlayerStats playerStats)
    {
        foreach (BaseSkillComponent comp in skillComponents)
        {
            if (!comp.CanExecute(player, playerStats, skillData))
            {
                return false; // 只要有一个不满足，就不执行
            }
        }
        return true;
    }
}
