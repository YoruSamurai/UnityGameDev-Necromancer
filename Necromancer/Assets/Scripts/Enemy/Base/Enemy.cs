using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Components
    public Rigidbody2D rb { get; set; }
    public Animator anim { get; private set; }

    public MonsterStats monsterStats { get; private set; }

    #endregion

    #region 属性
    public float currentDamageMultiplier;

    #endregion

    #region 特效
    public GameObject dieFX1;
    public GameObject hitBloodSprite;
    public GameObject bloodSprite;
    public GameObject bloodDirSprite;
    #endregion


    #region SO
    [SerializeField] private EnemyConfigSO configSO;

    public EnemyIdleSOBase enemyIdleBaseInstance {  get; set; }
    public EnemyChaseSOBase enemyChaseBaseInstance { get; set; }
    public EnemyAttackSOBase enemyAttackBaseInstance { get; set; }
    #endregion

    #region 状态机变量
    public EnemyStateMachine stateMachine { get; set; }
    public EnemyIdleState idleState { get; set; }
    public EnemyChaseState chaseState { get; set; }
    public EnemyAttackState attackState { get; set; }
    public EnemyStunState stunState { get; set; }
    public EnemyDieState dieState { get; set; }

    #endregion

    #region 攻击冷却
    public bool isAttacking;
    [SerializeField] public float attackCooldown; // 每次攻击后的冷却时间
    public float currentAttackCooldown = 0f;
    #endregion

    #region 移动属性
    public bool canMove;
    #endregion

    #region 击退状态
    public bool isKnockBack { get; private set; } = false;

    public IEnumerator KnockbackLock(Vector2 direction, float force, float duration)
    {
        if (rb != null)
        {
            // 添加击退力
            rb.AddForce(direction * force, ForceMode2D.Impulse);
        }

        // 进入击退锁定状态
        isKnockBack = true;

        // 等待指定时间
        yield return new WaitForSeconds(duration);

        // 恢复行动
        isKnockBack = false;
    }


    #endregion

    [Header("Collision Info")]//碰撞参数
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsOneWayPlatform;
    [SerializeField] public Transform shootPosition;

    //面向
    public int facingDir { get; private set; } = 1;
    public bool facingRight = true;


    protected virtual void Awake()
    {
        monsterStats = GetComponent<MonsterStats>();
        var configCopy = configSO.DeepCopy();
        enemyIdleBaseInstance = configCopy.IdleBehavior;
        enemyChaseBaseInstance = configCopy.ChaseBehavior;
        enemyAttackBaseInstance = configCopy.AttackBehavior;

        monsterStats.SetupMonsterStats(configCopy.EnemyProfile);

        stateMachine = new EnemyStateMachine();
        idleState = new EnemyIdleState(this,stateMachine, monsterStats);
        chaseState = new EnemyChaseState(this,stateMachine, monsterStats);
        attackState = new EnemyAttackState(this,stateMachine, monsterStats);
        stunState = new EnemyStunState(this,stateMachine, monsterStats);
        dieState = new EnemyDieState(this,stateMachine, monsterStats);
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        
        enemyIdleBaseInstance.Initialize(gameObject, this,monsterStats);
        enemyChaseBaseInstance.Initialize(gameObject, this, monsterStats);
        enemyAttackBaseInstance.Initialize(gameObject, this, monsterStats);
        canMove = true;
        stateMachine.Initialize(idleState);
    }

    protected virtual void Update()
    {
        // 更新冷却计时器
        if (currentAttackCooldown > 0f)
        {
            currentAttackCooldown -= Time.deltaTime;
            if (currentAttackCooldown < 0f) currentAttackCooldown = 0f;
        }


        stateMachine.currentEnemyState.UpdateState();
    }

    protected virtual void FixedUpdate()
    {
        stateMachine.currentEnemyState.FixedUpdateState();
    }


    #region 切换状态
    public void ChangeToState(EnemyState state)
    {
        stateMachine.ChangeState(state);
    }


    #endregion



    #region 动画触发器
    public void AnimationTrigger(EnemyAnimationTriggerType type)
    {
        stateMachine.currentEnemyState.AnimationTriggerEvent(type);
    }
    #endregion

    #region 移动

    public void SetZeroVelocity()
    {
        rb.velocity = new Vector2(0, 0);
    }
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }

    #endregion

    #region Flip
    //翻转
    public virtual void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    //根据X是否翻转
    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();
    }
    #endregion

    #region 死亡
    public void OnHitted()
    {
        GameObject bloodEffect = Instantiate(hitBloodSprite, this.transform.position, Quaternion.identity);
        Vector2 hitPoint = transform.position;
        Vector2 attackerPos = PlayerStats.Instance.transform.position;

        PlayFxManager.Instance.PlayBloodLine(hitPoint, attackerPos);
    }
    public void Die()
    {
        // 生成基本死亡特效
        GameObject pixelEffect = Instantiate(dieFX1, this.transform.position, Quaternion.identity);
        GameObject bloodEffect = Instantiate(bloodSprite, this.transform.position, Quaternion.identity);

        // 获取玩家位置
        Vector3 playerPos = PlayerStats.Instance.transform.position;

        // 判断玩家在敌人的左边还是右边
        float direction = playerPos.x < transform.position.x ? 1f : -1f;

        // 设置血迹生成位置和方向
        Vector3 bloodDirPosition = transform.position + new Vector3(direction * 4f, 0, 0);
        GameObject bloodDirEffect = Instantiate(bloodDirSprite, bloodDirPosition, Quaternion.identity);

        // 如果玩家在右边，翻转血迹效果
        if (direction == -1f)
        {
            bloodDirEffect.transform.localScale = new Vector3(-.4f, .4f,1f);
        }

        // 清理特效
        Destroy(pixelEffect, 3f);

        // 销毁敌人本体
        Destroy(gameObject);
    }
    #endregion

    #region Collision

    //通过射线检测能不能射到地面，
    public bool IsGroundDetected()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround)
            || Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsOneWayPlatform);
    }

    //通过射线检测能不能射到墙上
    public virtual bool IsWallDetected()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    //通过debug射线绘制一些可视化
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }
    #endregion

    private void AnimationTriggerEvent(EnemyAnimationTriggerType triggerType)
    {
        //fill in when statemachine was created
        stateMachine.currentEnemyState.AnimationTriggerEvent(triggerType);
    }

    
}

#region 敌人枚举（意义还不明确）
public enum EnemyAnimationTriggerType
{
    EnemyAttackEnd,
    PlayFootstepSound,
    EnemyRollEnd,
    EnemyFlashMid,
    EnemyFlashEnd,
    EnemyHitDetermineStart,//伤害判定开始判定
    EnemyHitDetermineEnd,//伤害判定结束
    EnemyOnShoot,//敌人射箭
    EnemyDied,//敌人死亡
    EnemyOnSummon,
}
#endregion
