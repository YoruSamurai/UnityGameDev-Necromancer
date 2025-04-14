using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Enemy/Components/Chase/条件转换/计时器攻击")]
public class T追逐_计时转换 : EnemyBehaviorComponent
{
    [SerializeField] private float timeThreshold = 2f; // 追逐状态持续时间阈值
    private float _timer;
    [Header("这个condition暂时没用")]
    [SerializeField] private EnterCondition conditionType; // 之前的条件类型
    [SerializeField] private EnemyStateType targetState;  // 条件满足时要切换的目标状态

    public override void OnEnter()
    {
        _timer = 0f;
    }

    public override void OnUpdate()
    {
        _timer += Time.deltaTime;
        if (_timer > timeThreshold && enemy.currentAttackCooldown <= 0f)
        {
            switch (targetState)
            {
                case EnemyStateType.Attack:
                    enemy.stateMachine.ChangeState(enemy.attackState);
                    break;
                case EnemyStateType.Chase:
                    enemy.stateMachine.ChangeState(enemy.chaseState);
                    break;
                case EnemyStateType.Idle:
                    enemy.stateMachine.ChangeState(enemy.idleState);
                    break;
            }
        }
    }
}
