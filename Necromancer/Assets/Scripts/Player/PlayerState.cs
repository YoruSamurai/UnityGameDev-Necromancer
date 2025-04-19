using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;


//作为所有玩家子状态的基类 所有子类都有父类的属性如rb xyInput
public class PlayerState
{

    //状态机
    protected PlayerStateMachine stateMachine;
    //玩家脚本
    protected Player player;

    //刚体
    protected Rigidbody2D rb;

    //wsad输入
    public float xInput;
    public float yInput;

    //切换状态时设置动画bool值切换动画
    private string animBoolName;

    //状态计时器
    protected float stateTimer;

    
    
    //构造函数 用于在Player中创建状态
    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    //父类 进入状态时开始新状态动画
    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName,true);
        rb = player.rb;
        //Debug.Log("我在" + stateMachine.currentState);
    }

    //减一减计时器 获取wsad输入 把y速度设置给动画器
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;   

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        player.anim.SetFloat("yVelocity",rb.velocity.y);
    }

    public virtual void FixedUpdate()
    {

    }

    //离开状态时停止该状态动画
    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);
    }

    public virtual void AnimationTriggerEvent(PlayerAnimationTriggerType triggerType)
    {

    }

    

}
