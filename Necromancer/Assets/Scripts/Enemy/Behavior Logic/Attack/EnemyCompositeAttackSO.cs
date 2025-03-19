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
            // 重新初始化当前选择的攻击行为
            currentAttackComponents.Initialize(gameObject, enemy, monsterStats);
            currentAttackComponents.DoEnterLogic();
        }
        else
        {
            // 没有找到有效攻击行为时，切换回 idle 状态
            enemy.stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void DoUpdateLogic()
    {
        currentAttackComponents?.DoUpdateLogic();
    }

    public override void DoFixedUpdateLogic()
    {
        currentAttackComponents?.DoFixedUpdateLogic();
    }

    public override void DoExitLogic()
    {
        currentAttackComponents?.DoExitLogic();
    }

    public override void DoAnimationTriggerEventLogic(EnemyAnimationTriggerType triggerType)
    {
        currentAttackComponents?.DoAnimationTriggerEventLogic(triggerType);
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
            if (choice.condition == EnterCondition.DistancePlus && distance > choice.value)
            {
                indexs.Add(i);
            }
            else if (choice.condition == EnterCondition.DistanceMinus && distance <= choice.value)
            {
                indexs.Add(i);
            }
        }
        if (indexs.Count == 0)
        {
            return null;
        }
        return Instantiate(attackBehaviors[indexs[Random.Range(0, indexs.Count)]].attackBehavior);

    }
}
