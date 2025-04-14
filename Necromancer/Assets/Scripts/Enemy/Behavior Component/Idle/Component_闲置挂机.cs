using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Components/Idle/闲置挂机")]
public class Component_闲置挂机 : EnemyBehaviorComponent
{
    [SerializeField] private bool isShowedExclamation;
    [SerializeField] private GameObject exclamationPrefab;
    private GameObject exclamationInstance; // 实例化的感叹号

    public override void OnEnter()
    {
        isShowedExclamation = false;
    }

    public override void OnExit()
    {
        base.OnExit();
        enemy.canMove = true;
        // 销毁感叹号实例
        if (exclamationInstance != null)
        {
            Destroy(exclamationInstance);
        }
    }

    public override void OnFixedUpdate()
    {


    }

    //一般情况下 就Idle 一张图片挂机 
    //发现敌人 就是canMove == false 这里（感觉表达方式有点差）
    //进入Wake 然后就不用管了 待会就进入攻击状态了
    //重新进入的时候 我们就鹅 进入一个Sleep？ 动画触发一个事件 进入Idle
    public override void OnUpdate()
    {
        if (enemy.canMove == false)
        {
            if (!isShowedExclamation)
            {
                isShowedExclamation = true;
                // 实例化感叹号
                exclamationInstance = Instantiate(exclamationPrefab, enemy.transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
            }
            enemy.anim.SetBool("Idle", true);
            enemy.anim.SetBool("Move", false);
        }

        // 检测如果碰到墙或者即将掉落，进入延迟状态
        else if (enemy.IsGroundDetected())
        {
            enemy.anim.SetBool("Idle", true);
            enemy.anim.SetBool("Move", false);
            return;
        }

    }
}
