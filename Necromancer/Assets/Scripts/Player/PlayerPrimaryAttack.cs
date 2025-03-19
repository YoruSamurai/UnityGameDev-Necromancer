using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttack : PlayerState
{

    private EquipmentSO currentWeapon;


    public PlayerPrimaryAttack(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationTriggerEvent(PlayerAnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        switch(triggerType)
        {
            case PlayerAnimationTriggerType.PlayerAttackEnd:
                {
                    if (PlayerStats.Instance.isAttacking)
                        PlayerStats.Instance.isAttacking = false;
                    else if (PlayerStats.Instance.isParrying)
                        PlayerStats.Instance.isParrying = false;
                    PlayerStats.Instance.SetCurrentEquipmentIndex(0);
                    stateMachine.ChangeState(player.idleState);
                    break;
                }
            case PlayerAnimationTriggerType.PlayerHitDetermineStart:
                {
                    if (PlayerStats.Instance.currentEquipmentIndex == 1)
                    {
                        PlayerStats.Instance.baseEquipment1.TriggerHitCheckStart();
                    }
                    else if (PlayerStats.Instance.currentEquipmentIndex == 2)
                    {
                        PlayerStats.Instance.baseEquipment2.TriggerHitCheckStart();
                    }
                    break;
                }
            case PlayerAnimationTriggerType.PlayerHitDetermineEnd:
                {
                    if (PlayerStats.Instance.currentEquipmentIndex == 1)
                    {
                        PlayerStats.Instance.baseEquipment1.TriggerHitCheckEnd();
                    }
                    else if (PlayerStats.Instance.currentEquipmentIndex == 2)
                    {
                        PlayerStats.Instance.baseEquipment2.TriggerHitCheckEnd();
                    }
                    break;
                }
        }
    }

    public override void Enter()
    {
        base.Enter();
        // 获取当前装备SO
        currentWeapon = PlayerStats.Instance.baseEquipment1.equipmentSO;
        if(PlayerStats.Instance.currentEquipmentIndex == 1)
            currentWeapon = PlayerStats.Instance.baseEquipment1.equipmentSO;
        else if(PlayerStats.Instance.currentEquipmentIndex == 2)
            currentWeapon = PlayerStats.Instance.baseEquipment2.equipmentSO;


        // 应用武器动画
        player.ApplyWeaponAnimator(currentWeapon.attackAnimator);
        SetComboAnimation();
        
    }

    private void SetComboAnimation()
    {
        Debug.Log(currentWeapon.comboAnimations.Length);
        if (currentWeapon.comboAnimations.Length > 0)
        {
            int currentCombo = 0;
            if (PlayerStats.Instance.currentEquipmentIndex == 1)
            {
                currentCombo = PlayerStats.Instance.baseEquipment1.currentCombo;
            }
            else if (PlayerStats.Instance.currentEquipmentIndex == 2)
            {
                currentCombo = PlayerStats.Instance.baseEquipment2.currentCombo;
            }

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

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if(stateTimer < 0 )
        {
            player.SetVelocity(0, rb.velocity.y);
        }
    }

}
