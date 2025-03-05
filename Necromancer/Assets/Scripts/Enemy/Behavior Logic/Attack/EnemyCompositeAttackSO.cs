using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Attack/CompositeAttack")]
public class EnemyCompositeAttackSO : EnemyAttackSOBase
{
    [SerializeField] private List<AttackChoice> attackBehaviors; // 预设的攻击行为列表
    private EnemyAttackSOBase currentAttackComponents;
     

    public override void DoEnterLogic()
    {
        // 选择一种攻击方式（例如随机选择）
        currentAttackComponents = ChooseAttackBehavior();
        if (currentAttackComponents != null)
        {
            // 注意：这里重新初始化当前选择的攻击行为
            currentAttackComponents.Initialize(gameObject, enemy);
            currentAttackComponents.DoEnterLogic();
        }
    }

    public override void DoUpdateLogic()
    {
        currentAttackComponents?.DoUpdateLogic();
    }

    public override void DoExitLogic()
    {
        currentAttackComponents?.DoExitLogic();
    }

    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType)
    {
        /*if (triggerType == AnimationTriggerType.EnemyAttackEnd)
        {
            // 将当前攻击行为的结束事件传递出去（让它自己做一些清理工作）
            currentAttackComponents?.DoAnimationTriggerEventLogic(triggerType);
            // 重新选择一个新的攻击行为
            currentAttackComponents = ChooseAttackBehavior();
            if (currentAttackComponents != null)
            {
                // 重新初始化新的攻击行为
                currentAttackComponents.Initialize(gameObject, enemy);
                currentAttackComponents.DoEnterLogic();
            }
        }
        else
        {*/
            currentAttackComponents?.DoAnimationTriggerEventLogic(triggerType);
        //}
    }

    /// <summary>
    /// 重置当前选择的攻击行为，下次进入攻击状态时重新选择
    /// </summary>
    public void ResetAttack()
    {
        currentAttackComponents = null;
    }

    private EnemyAttackSOBase ChooseAttackBehavior()
    {
        Vector2 enemyPos = enemy.transform.position;
        Vector2 playerPos = playerTransform.position;
        float distance = Vector2.Distance(enemyPos, playerPos);
        List<int> indexs = new List<int>();
        for (int i = 0; i < attackBehaviors.Count; i++)
        {
            AttackChoice choice = attackBehaviors[i];
            if (choice.condition == EnterCondition.DistancePlus && distance > choice.distance)
            {
                indexs.Add(i);
            }
            else if(choice.condition == EnterCondition.DistanceMinus && distance <= choice.distance)
            {
                indexs.Add(i);
            }
        }
        return Instantiate(attackBehaviors[indexs[Random.Range(0, indexs.Count)]].attackBehavior);

        /*if (attackBehaviors == null || attackBehaviors.Count == 0)
            return null;
        int index = Random.Range(0, attackBehaviors.Count);
        // 克隆出新的实例，保证每个敌人使用独立实例
        return Instantiate(attackBehaviors[index]);*/
    }
}
