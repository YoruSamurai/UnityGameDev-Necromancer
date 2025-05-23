using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttack : PlayerState
{

    private EquipmentSO currentWeapon;
    private BaseEquipment baseEquipment;

    private float previousNormalizedTime = 0; // 用于存储上一个动画帧

    public PlayerPrimaryAttack(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationTriggerEvent(PlayerAnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        switch (triggerType)
        {
            case PlayerAnimationTriggerType.PlayerCanNotInterrupt:
                {
                    //设置速度应该写在下面 通过获取武器的属性来设置
                    player.SetVelocity(3 * player.facingDir, rb.velocity.y);
                    stateTimer = .15f;
                    PlayerStats.Instance.canInterrupt = false;
                    int combo = baseEquipment.currentCombo > 0 ? baseEquipment.currentCombo - 1 : currentWeapon.comboAnimations.Length - 1;
                    // 生成刀光效果
                    if (currentWeapon != null &&
                        currentWeapon.slashAnimations != null && currentWeapon.slashAnimations.Length > 0 &&
                        currentWeapon.slashOffsets != null && currentWeapon.slashOffsets.Length > 0)
                    {
                        AnimationClip slashClip = currentWeapon.slashAnimations[combo];
                        if (slashClip != null)
                        {
                            Vector2 offset = currentWeapon.slashOffsets[combo];
                            Debug.Log(slashClip.name + "slashshh");
                            player.InitialFxPrefab(slashClip, offset);
                        }
                        else
                        {
                            Debug.LogWarning(currentWeapon.equipmentName + "没有攻击特效");
                        }
                        
                    }
                    // 近战音效
                    MeleeEquipment meleeEquipment = baseEquipment as MeleeEquipment;
                    if(meleeEquipment != null)
                    {
                        if (meleeEquipment.meleeAttacks[combo].attackSfx != null)
                        {
                            SoundData soundData = meleeEquipment.meleeAttacks[combo].attackSfx.GetSoundData();
                            SoundManager.Instance.CreateSound()
                            .WithSoundData(soundData)
                            .WithPosition(player.transform.position)
                            .Play();
                        }
                        else
                        {
                            Debug.LogWarning(meleeEquipment.equipmentName + "没有对应的攻击音效诶");
                        }
                    }
                    break;
                }
            case PlayerAnimationTriggerType.PlayerChargeCheckLeft:
                {

                    AnimatorStateInfo stateInfo = player.anim.GetCurrentAnimatorStateInfo(0);
                    previousNormalizedTime = stateInfo.normalizedTime;
                    Debug.Log("当前动画帧: " + previousNormalizedTime);
                    break;
                }
            case PlayerAnimationTriggerType.PlayerChargeCheckRight:
                {
                    // 进行检查
                    MeleeEquipment meleeEquipment = baseEquipment as MeleeEquipment;
                    if (meleeEquipment != null)
                    {
                        if (meleeEquipment.isProcessingInput)
                        {
                            // 如果检查不通过，返回到之前的动画帧
                            player.anim.Play(player.anim.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, previousNormalizedTime);
                        }

                    }
                    break;
                }
            case PlayerAnimationTriggerType.PlayerOnShoot:
                {
                    StaffEquipment staffEquipment = baseEquipment as StaffEquipment;
                    if(staffEquipment != null)
                    {
                        staffEquipment.FireProjectile();
                    }
                    break;
                }
            case PlayerAnimationTriggerType.PlayerAttackEnd:
                {
                    if (PlayerStats.Instance.isAttacking)
                        PlayerStats.Instance.isAttacking = false;
                    else if (PlayerStats.Instance.isParrying)
                        PlayerStats.Instance.isParrying = false;
                    PlayerStats.Instance.SetCurrentEquipmentIndex(0);
                    player.SetVelocity(0, rb.velocity.y);
                    
                    stateMachine.ChangeState(player.idleState);
                    break;
                }
            case PlayerAnimationTriggerType.PlayerHitDetermineStart:
                {
                    Debug.Log("你要努力！");
                    baseEquipment.TriggerHitCheckStart();
                    break;
                }
            case PlayerAnimationTriggerType.PlayerHitDetermineEnd:
                {
                    baseEquipment.TriggerHitCheckEnd();
                    break;
                }
        }
    }

    public override void Enter()
    {
        base.Enter();
        // 获取当前装备SO
        if(PlayerStats.Instance.currentEquipmentIndex == 1)
        {
            currentWeapon = PlayerStats.Instance.baseEquipment1.equipmentSO;
            baseEquipment = PlayerStats.Instance.baseEquipment1;
        }
        else if(PlayerStats.Instance.currentEquipmentIndex == 2)
        {
            currentWeapon = PlayerStats.Instance.baseEquipment2.equipmentSO;
            baseEquipment = PlayerStats.Instance.baseEquipment2;
        }

        SetComboAnimation();
        
    }

    private void SetComboAnimation()
    {
        Debug.Log(currentWeapon.comboAnimations.Length);
        if (currentWeapon.comboAnimations.Length > 0)
        {
            AnimationClip clip = currentWeapon.comboAnimations[baseEquipment.currentCombo];
            currentWeapon.attackAnimator["DplayerAttack1"] = clip;
            player.ApplyWeaponAnimator(currentWeapon.attackAnimator);
        }
        
    }

    public override void Exit()
    {
        base.Exit();
        PlayerStats.Instance.isAttacking = false;
        PlayerStats.Instance.canInterrupt = true;
        //player.BusyFor(.025f);
        MeleeEquipment meleeEquipment = baseEquipment as MeleeEquipment;
        if (meleeEquipment != null)
        {
            meleeEquipment.isCharged = false;
        }


        baseEquipment.TriggerHitCheckEnd();
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
        if(stateTimer < 0 )
        {
            player.SetVelocity(0, rb.velocity.y);
        }
    }

}
