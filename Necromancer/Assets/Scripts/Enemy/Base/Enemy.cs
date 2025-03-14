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

    [Header("Collision Info")]//碰撞参数
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
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


    #region 切换状态
    public void ChangeToState(EnemyState state)
    {
        stateMachine.ChangeState(state);
    }


    #endregion



    #region 动画触发器
    public void AnimationTrigger(AnimationTriggerType type)
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
    }
    public void Die()
    {
        GameObject pixelEffect = Instantiate(dieFX1, this.transform.position, Quaternion.identity);
        GameObject bloodEffect = Instantiate(bloodSprite, this.transform.position, Quaternion.identity);
        Destroy(pixelEffect,3f);
        // 将血浆效果设为墙体的子物体（使其跟随墙体）
        Destroy(gameObject);
    }
    #endregion

    #region Collision

    //通过射线检测能不能射到地面，
    public bool IsGroundDetected()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
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

    private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        //fill in when statemachine was created
        stateMachine.currentEnemyState.AnimationTriggerEvent(triggerType);
    }

    
}

#region 敌人枚举（意义还不明确）
public enum AnimationTriggerType
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
}
#endregion
