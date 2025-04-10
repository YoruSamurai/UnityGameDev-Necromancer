using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Components/Idle/左右闲置")]
public class Component_左右闲置 : EnemyBehaviorComponent
{
    [SerializeField] private float MovementSpeed;

    // 延迟状态相关变量
    private bool isDelaying = false;
    private float delayTimer = 0f;
    [SerializeField] private float delayDuration; // 延迟转身时间

    [SerializeField] private bool isShowedExclamation;
    [SerializeField] private GameObject exclamationPrefab;
    private GameObject exclamationInstance; // 实例化的感叹号

    public override void OnEnter()
    {
        isDelaying = false;
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
        // 如果正在被击退，则跳过正常移动逻辑
        if (enemy.isKnockBack)
        {
            return;
        }
        else if(enemy.canMove == false)
        {
            enemy.SetVelocity(0, enemy.rb.velocity.y);
        }
        else if (isDelaying || (!enemy.IsGroundDetected() || enemy.IsWallDetected()))
        {
            enemy.SetVelocity(0, enemy.rb.velocity.y);
        }
        else
        {
            // 正常情况下按照左右移动
            enemy.SetVelocity(enemy.facingDir * MovementSpeed, enemy.rb.velocity.y);
        }

    }

    public override void OnUpdate()
    {
        if(enemy.canMove == false)
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
        // 如果处于延迟状态，则更新计时器，保持静止，并设置动画参数
        if (isDelaying)
        {
            delayTimer += Time.deltaTime;
            //enemy.SetVelocity(0, enemy.rb.velocity.y);
            if (delayTimer >= delayDuration)
            {
                // 延迟结束后执行翻转，并恢复运动
                //Debug.Log("我转" + delayDuration);
                enemy.Flip();
                enemy.anim.SetBool("Idle", false);
                enemy.anim.SetBool("Move", true);
                isDelaying = false;
                delayTimer = 0f;
            }
            return;
        }

        // 检测如果碰到墙或者即将掉落，进入延迟状态
        else if (!enemy.IsGroundDetected() || enemy.IsWallDetected())
        {
            isDelaying = true;
            delayTimer = 0f;
            //enemy.SetVelocity(0,enemy.rb.velocity.y);
            enemy.anim.SetBool("Idle", true);
            enemy.anim.SetBool("Move", false);
            return;
        }

    }
}
