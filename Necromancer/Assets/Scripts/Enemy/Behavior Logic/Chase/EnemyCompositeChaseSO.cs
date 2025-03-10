using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Enemy/Chase/CompositeChase")]
public class EnemyCompositeChaseSO : EnemyChaseSOBase
{
    [SerializeField] private List<ChaseChoice> chaseBehaviors; // 预设的攻击行为列表
    private EnemyChaseSOBase currentChaseComponents;


    public override void DoEnterLogic()
    {
        // 选择一种攻击方式（例如随机选择）
        currentChaseComponents = ChooseAttackBehavior();
        if (currentChaseComponents != null)
        {
            // 注意：这里重新初始化当前选择的攻击行为
            currentChaseComponents.Initialize(gameObject, enemy, monsterStats);
            currentChaseComponents.DoEnterLogic();
        }
        else
        {
            // 没有找到有效攻击行为时，切换回 idle 状态
            Debug.Log("我回去了");
            enemy.stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void DoUpdateLogic()
    {
        currentChaseComponents?.DoUpdateLogic();
    }

    public override void DoExitLogic()
    {
        currentChaseComponents?.DoExitLogic();
    }

    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType)
    {
        currentChaseComponents?.DoAnimationTriggerEventLogic(triggerType);
    }

    /// <summary>
    /// 重置当前选择的攻击行为，下次进入攻击状态时重新选择
    /// </summary>
    public void ResetAttack()
    {
        currentChaseComponents = null;
    }

    private EnemyChaseSOBase ChooseAttackBehavior()
    {
        Vector2 enemyPos = enemy.transform.position;
        Vector2 playerPos = playerTransform.position;
        float distance = Vector2.Distance(enemyPos, playerPos);
        List<int> indexs = new List<int>();
        for (int i = 0; i < chaseBehaviors.Count; i++)
        {
            ChaseChoice choice = chaseBehaviors[i];
            if (choice.condition == EnterCondition.DistancePlus && distance > choice.value)
            {
                indexs.Add(i);
            }
            else if (choice.condition == EnterCondition.DistanceMinus && distance <= choice.value)
            {
                indexs.Add(i);
            }
            else if (choice.condition == EnterCondition.BackGroundCheck && distance < 15f)
            {
                // 判断敌人在玩家的左侧还是右侧
                float direction = (enemyPos.x > playerPos.x) ? 1f : -1f;

                // 计算射线起点，沿水平方向偏移
                Vector2 rayOrigin = enemyPos + Vector2.right * direction * choice.value;

                // 从该点向下射出射线（射线长度可以根据需求调整，这里设为2f）
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 2f);
                Debug.DrawRay(rayOrigin, Vector2.down * 1f, Color.green, 2f);

                // 如果检测到地面，则条件满足
                if (hit.collider != null)
                {
                    indexs.Add(i);
                }
            }
        }
        if (indexs.Count == 0)
        {
            return null;
        }
        return Instantiate(chaseBehaviors[indexs[Random.Range(0, indexs.Count)]].attackBehavior);


    }
}
