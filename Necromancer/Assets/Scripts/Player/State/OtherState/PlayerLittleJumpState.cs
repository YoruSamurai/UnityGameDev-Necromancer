using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLittleJumpState : PlayerState
{
    public PlayerLittleJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationTriggerEvent(PlayerAnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        if (triggerType == PlayerAnimationTriggerType.PlayerAnimationEndTrigger)
        {
            stateMachine.ChangeState(player.idleState);
        }
        
    }


    public override void Enter()
    {
        base.Enter();
        player.rb.isKinematic = true;
        AdjustPosition();
    }

    private void AdjustPosition()
    {
        Vector2 newPosition = player.transform.position;
        //newPosition.y += .1f;
        if(player.cornerNormal.x > 0)
        {
            newPosition.x -= 0.4f;
        }
        else
        {
            newPosition.x += 0.4f;
        }
        player.transform.position = newPosition;
    }

    public override void Exit()
    {
        base.Exit();
        player.rb.isKinematic = false;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        player.SetZeroVelocity();
    }
}
