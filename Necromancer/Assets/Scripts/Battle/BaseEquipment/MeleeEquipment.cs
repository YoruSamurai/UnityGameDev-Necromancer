using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEquipment : BaseEquipment
{

    //近战武器要什么属性？   一次combo的总次数 每次攻击的：时长time 伤害倍率mag 晕值
    //伤害box又是另一个麻烦的故事了 比如说 形状是一个枚举 以玩家为中心还是以玩家面前1/2box为中心又是一个枚举？ 半径？长？宽？
    [SerializeField] public int fullComboAttackTimes;//一次combo的总攻击次数
    [SerializeField] public List<MeleeAttackStruct> meleeAttacks;

    [SerializeField] public bool isHitInAttack;//单次攻击是否有命中

    [SerializeField] public bool canCharge;//武器是否可以蓄力
    [SerializeField] private float holdThreshold; // 长按阈值
    [SerializeField] private float holdTimer = 0f;
    public bool isCharged = false;
    public bool isProcessingInput = false; // 标记是否正在处理盾牌输入
    private int currentWeaponIndex;


    [Header("Attack Effect Settings")]
    [SerializeField] protected GameObject rectangleEffectPrefab; // 蓝色闪烁特效预制件
    [SerializeField] protected GameObject circleEffectPrefab; // 蓝色闪烁特效预制件

    [SerializeField] public BoxCollider2D attackCheckBox;
    [SerializeField] public CircleCollider2D attackCheckCircle;
    

    protected override void Start()
    {
        base.Start();
        // 使用 as 关键字进行类型转换
        MeleeEquipmentSO meleeEquipmentSO = equipmentSO as MeleeEquipmentSO;
        if (meleeEquipmentSO != null)
        {
            fullComboAttackTimes = meleeEquipmentSO.fullComboAttackTimes;
            meleeAttacks = meleeEquipmentSO.meleeAttacks;
            if(equipmentType == EquipmentType.heavyMelee)
                canCharge = true;
            else if(equipmentType == EquipmentType.lightMelee)
                canCharge = false;
        }
        else
        {
            Debug.LogWarning("The equipmentSO is not a MeleeEquipmentSO");
        }

        attackCheckBox = GetComponent<BoxCollider2D>();
        attackCheckCircle = GetComponent<CircleCollider2D>();
        attackCheckBox.excludeLayers = ~enemyLayerMask;  // 只检测敌人
        attackCheckCircle.excludeLayers = ~enemyLayerMask;
        attackCheckBox.enabled = false;
        attackCheckCircle.enabled = false;
        isHitInAttack = false;

    }

    protected override void Update()
    {
        base.Update();
        if (isProcessingInput)
        {
            // 持续按住时累加计时 松开不再累计
            if (currentWeaponIndex == 1 ?
                Input.GetKey(KeyCode.Mouse0) : Input.GetKey(KeyCode.Mouse1))
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdThreshold && !isCharged)
                {
                    isCharged = true;
                }
            }
            if (currentWeaponIndex == 1 ?
                Input.GetKeyUp(KeyCode.Mouse0) : Input.GetKeyUp(KeyCode.Mouse1))
            {
                isProcessingInput = false;
            }
        }
        if (comboBreakTimer > 0)
        {
            comboBreakTimer -= Time.deltaTime;
        }
        else
        {
            if (currentCombo == 0)
                return;
            SetCombo(0);
        }

    }
    public override void UseEquipment()
    {
        base.UseEquipment();
        // 检查是否处于攻击CD中
        if (!GetCanUseEquipment())
        {
            Debug.Log("BASE攻击还在冷却1中！");
            return;
        }
        else
        {
            PlayerStats.Instance.ChangeToAttackState();
            // 如果已经开始处理输入，就不重复初始化
            if (equipmentType == EquipmentType.heavyMelee && !isProcessingInput)
            {
                isProcessingInput = true;
                holdThreshold = meleeAttacks[currentCombo].chargeThreshold;
                holdTimer = 0f;
                isCharged = false;
                currentWeaponIndex = PlayerStats.Instance.currentEquipmentIndex;
            }
            TriggerAttackEffect();//可视化攻击范围 未启用
            Attack();
            // 攻击后重置combo和冷却时间
            ResetCombo();
        }
    }
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
    }

    public override void TriggerHitCheckStart()
    {
        EnableHitDetermine();
    }

    public override void TriggerHitCheckEnd()
    {
        DisableHitDetermine();
    }


    public void EnableHitDetermine()
    {
        // 如果连击数据无效，直接返回
        if (currentCombo >= meleeAttacks.Count)
            return;

        // 选择当前攻击数据（这里参考你已有的 GetHitEnemy() 逻辑）
        MeleeAttackStruct currentAttack = meleeAttacks[currentCombo == 0 ? meleeAttacks.Count - 1 : currentCombo - 1];
        Debug.Log("我要努力！");
        // 获取玩家面向方向
        bool isFacingRight = GetFacingDirection();
        Vector2 faceDirection = isFacingRight ? Vector2.right : Vector2.left;

        // 计算攻击中心位置
        Vector2 attackCenter = currentAttack.attackCenter switch
        {
            MeleeAttackCenterEnum.player => (Vector2)transform.position,
            MeleeAttackCenterEnum.front => currentAttack.attackShape == MeleeAttackShapeEnum.circle ?
                (Vector2)transform.position + faceDirection * currentAttack.attackRadius :
                (Vector2)transform.position + faceDirection * (currentAttack.attackLength / 2),
            _ => (Vector2)transform.position
        };

        // 根据攻击形状设置碰撞器
        if (currentAttack.attackShape == MeleeAttackShapeEnum.circle)
        {
            // 禁用BoxCollider2D
            if (attackCheckBox != null)
                attackCheckBox.enabled = false;
            // 启用CircleCollider2D，并设置其参数
            if (attackCheckCircle != null)
            {
                attackCheckCircle.enabled = true;
                // 假设碰撞器的父物体就是本对象，
                // 设定 offset 为攻击中心与本对象位置之差
                attackCheckCircle.offset = faceDirection * (attackCenter - (Vector2)player.transform.position);
                attackCheckCircle.radius = currentAttack.attackRadius;
            }
        }
        else // 如果是矩形攻击
        {
            // 禁用CircleCollider2D
            if (attackCheckCircle != null)
                attackCheckCircle.enabled = false;
            // 启用BoxCollider2D，并设置其参数
            if (attackCheckBox != null)
            {
                attackCheckBox.enabled = true;
                // BoxCollider2D 的 offset 为攻击中心与本对象位置之差
                attackCheckBox.offset = faceDirection * (attackCenter - (Vector2)player.transform.position);
                // 设置BoxCollider2D的大小
                attackCheckBox.size = new Vector2(currentAttack.attackLength, currentAttack.attackWidth);
            }
        }
    }

    public void DisableHitDetermine()
    {
        attackCheckCircle.enabled = false;
        attackCheckBox.enabled = false;
        isHitInAttack = false;
    }

    /*public Collider2D[] GetHitEnemy()
    {
        if (currentCombo >= meleeAttacks.Count) return null;
        Collider2D[] hits;
        MeleeAttackStruct currentAttack = meleeAttacks[currentCombo == 0 ? meleeAttacks.Count - 1: currentCombo - 1];

        bool isFacingRight = GetFacingDirection();
        Vector2 faceDirection = isFacingRight ? Vector2.right : Vector2.left;

        Vector2 attackCenter = currentAttack.attackCenter switch
        {
            MeleeAttackCenterEnum.player => transform.position,
            MeleeAttackCenterEnum.front => currentAttack.attackShape == MeleeAttackShapeEnum.circle ?
                (Vector2)transform.position + faceDirection * currentAttack.attackRadius : (Vector2)transform.position + faceDirection * currentAttack.attackLength / 2,
            _ => transform.position
        };

        
        if (currentAttack.attackShape == MeleeAttackShapeEnum.circle)
        {
            hits = Physics2D.OverlapCircleAll(attackCenter, currentAttack.attackRadius, PlayerStats.Instance.whatIsEnemy);
        }
        else
        {
            Vector2 size = new Vector2(currentAttack.attackLength, currentAttack.attackWidth);
            hits = Physics2D.OverlapBoxAll(
                attackCenter,
                size,
                isFacingRight ? 0 : 180,
                PlayerStats.Instance.whatIsEnemy
            );
        }
        return hits;
    }*/

    protected virtual void TriggerAttackEffect()
    {
        /*if (meleeAttacks == null || currentCombo >= meleeAttacks.Count) return;

        MeleeAttackStruct currentAttack = meleeAttacks[currentCombo];

        // 获取玩家朝向（假设通过transform.localScale判断）
        bool isFacingRight = GetFacingDirection();
        Vector2 faceDirection = isFacingRight ? Vector2.right : Vector2.left;

        // 计算攻击中心位置
        Vector2 attackCenter = currentAttack.attackCenter switch
        {
            MeleeAttackCenterEnum.player => transform.position,
            MeleeAttackCenterEnum.front => currentAttack.attackShape==MeleeAttackShapeEnum.circle? 
                (Vector2)transform.position + faceDirection * currentAttack.attackRadius: (Vector2)transform.position + faceDirection * currentAttack.attackLength/2,
            _ => transform.position
        };

        // 选择预制体并实例化
        GameObject effectPrefab = currentAttack.attackShape switch
        {
            MeleeAttackShapeEnum.circle => circleEffectPrefab,
            MeleeAttackShapeEnum.rectangle => rectangleEffectPrefab,
            _ => null
        };

        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, attackCenter, Quaternion.identity, transform);

            // 设置特效尺寸
            if (currentAttack.attackShape == MeleeAttackShapeEnum.circle)
            {
                effect.transform.localScale = Vector3.one * (currentAttack.attackRadius * 2);
            }
            else
            {
                Vector3 effectScale = new Vector3(
                    currentAttack.attackLength,
                    currentAttack.attackWidth,
                    1
                );

                // 根据朝向调整方向
                if (!isFacingRight)
                {
                    effectScale.x *= -1;
                }

                effect.transform.localScale = effectScale;
            }
            Color color = Color.red;
            color.a = .3f;
            effect.GetComponent<SpriteRenderer>().color = color;
            Destroy(effect, 0.2f);
        }*/
    }

    private void Attack()
    {
        PlayerStats.Instance.OnAttack();
    }


    public override void DoDamage(float _cMag, MonsterStats monsterStats)
    {
        base.DoDamage(_cMag, monsterStats);

        //在这里进行击退
        // 获取当前攻击数据，用于计算击退力度
        MeleeAttackStruct currentAttack = meleeAttacks[currentCombo == 0 ? meleeAttacks.Count - 1 : currentCombo - 1];

        // 计算击退方向
        Vector2 direction = monsterStats.enemy.transform.position - player.transform.position;
        direction = new Vector2(direction.x > 0 ? 1 : -1, 0.1f); // 控制X和Y方向

        // 直接调用重构后的 KnockbackLock
        monsterStats.enemy.StartCoroutine(
            monsterStats.enemy.KnockbackLock(direction, currentAttack.knockbackForce, 0.04f)
        );
    }


    #region 连击和冷却时间相关函数

    public void SetComboBreakTime(float _breakTime)
    {
        comboBreakTimer = _breakTime;
    }

    public void SetCombo(int _combo)
    {
        currentCombo = _combo;
    }

    //在使用装备攻击的时候 设置combo数和冷却时间
    protected void ResetCombo()
    {
        Debug.Log("当前combo为" + currentCombo);
        //在这里不但要给武器设置 还要给PlayerStat（管理人物攻击的单例）设置一个锁
        
        SetAttackCooldown(meleeAttacks[currentCombo].attackTime);

        // 只有一段连击的情况
        if (meleeAttacks.Count == 1)
        {
            Debug.Log("只有一段连击，重置combo");
            SetCombo(0);
            SetComboBreakTime(0);
            return;
        }
        //重置combo
        if (comboBreakTimer > 0)
        {
            
            Debug.Log(meleeAttacks.Count);
            if(currentCombo + 1 == meleeAttacks.Count)
            {
                Debug.Log("连击结束");
                SetCombo(0);
                SetComboBreakTime(0);
            }
            else
            {
                Debug.Log("进入下一段连击");
                SetCombo(currentCombo + 1);
                SetComboBreakTime(meleeAttacks[currentCombo - 1].comboBreakTime);
            }
        }
        else
        {
            Debug.Log("开始连击");
            SetCombo(currentCombo + 1);
            SetComboBreakTime(meleeAttacks[currentCombo - 1].comboBreakTime);
        }
        
    }
    #endregion
}
