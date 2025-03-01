using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttack : PlayerState
{

    private int comboCounter;
    private EquipmentSO currentWeapon;

    private float lastTimeAttacked;
    private float comboWindow = 2; 

    public PlayerPrimaryAttack(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // 获取当前装备SO
        currentWeapon = PlayerStats.Instance.baseEquipment1.equipmentSO;
;
        // 应用武器动画
        player.ApplyWeaponAnimator(currentWeapon.attackAnimator);
        SetComboAnimation();
        
    }

    private void SetComboAnimation()
    {
        if (currentWeapon.comboAnimations.Length > 0)
        {
            int currentCombo = PlayerStats.Instance.baseEquipment1.currentCombo;
            AnimationClip clip = currentWeapon.comboAnimations[currentCombo];
            currentWeapon.attackAnimator["playerAttack1"] = clip;
        }
        //设置速度应该写在下面 通过获取武器的属性来设置
        player.SetVelocity(1 * player.facingDir, rb.velocity.y);
        stateTimer = .1f;
    }

    public override void Exit()
    {
        base.Exit();
        player.ResetToDefaultAnimator(); // 恢复默认动画
        player.StartCoroutine("BusyFor", .02f);
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer < 0 )
        {
            player.SetZeroVelocity();
        }

        if(triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
