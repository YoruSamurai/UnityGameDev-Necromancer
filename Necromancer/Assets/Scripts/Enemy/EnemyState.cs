using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class EnemyState
{
    //状态机
    protected EnemyStateMachine stateMachine;
    //玩家脚本
    protected Enemy enemy;

    protected bool triggerCalled;
    private string animBoolName;

    //状态计时器
    protected float stateTimer;

    //构造函数 用于在Enemy中创建状态
    public EnemyState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName)
    {
        this.enemy = _enemy;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }


    public virtual void Enter()
    {
        //进入一个新状态的时候 就可以重新触发trigger了
        triggerCalled = false;
        enemy.anim.SetBool(animBoolName, true);

    }

    //减一减计时器 
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    //离开状态时停止该状态动画
    public virtual void Exit()
    {
        enemy.anim.SetBool(animBoolName, false);
    }


    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }

}
