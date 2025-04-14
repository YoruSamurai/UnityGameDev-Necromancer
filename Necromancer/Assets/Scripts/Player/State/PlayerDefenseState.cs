using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefenseState : PlayerState
{
    private ShieldEquipmentSO currentWeapon;


    public PlayerDefenseState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if (PlayerStats.Instance.currentEquipmentIndex == 1)
            currentWeapon = PlayerStats.Instance.baseEquipment1.equipmentSO as ShieldEquipmentSO;
        else if (PlayerStats.Instance.currentEquipmentIndex == 2)
            currentWeapon = PlayerStats.Instance.baseEquipment2.equipmentSO as ShieldEquipmentSO;


        // 应用武器动画
        player.ApplyWeaponAnimator(currentWeapon.attackAnimator);
        SetComboAnimation();

    }

    private void SetComboAnimation()
    {
        Debug.Log(currentWeapon.comboAnimations.Length);
        if (currentWeapon.comboAnimations.Length > 0)
        {
            AnimationClip clip = currentWeapon.defenseAnimations[0];
            currentWeapon.attackAnimator["DplayerDefense"] = clip;
        }
        //设置速度应该写在下面 通过获取武器的属性来设置
        player.SetVelocity(1 * player.facingDir, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        PlayerStats.Instance.isDefensing = false;
        PlayerStats.Instance.SetCurrentEquipmentIndex(0);
        player.ResetToDefaultAnimator(); // 恢复默认动画
        player.StartCoroutine("BusyFor", .02f);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        player.SetVelocity(0, rb.velocity.y);
    }

    public override void AnimationTriggerEvent(PlayerAnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        if(triggerType == PlayerAnimationTriggerType.PlayerAnimationEndTrigger)
        {
            stateMachine.ChangeState(player.idleState);
        }
        if (triggerType == PlayerAnimationTriggerType.PlayerOnDefense)
        {
            PlayerStats.Instance.isDefensing = true;
        }
    }
}
