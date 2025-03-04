using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Components
    public Rigidbody2D rb { get; set; }
    public Animator anim { get; private set; }

    #endregion


    #region SO
    [SerializeField] private EnemyIdleSOBase enemyIdleBase;
    [SerializeField] private EnemyChaseSOBase enemyChaseBase;
    [SerializeField] private EnemyAttackSOBase enemyAttackBase;

    public EnemyIdleSOBase enemyIdleBaseInstance {  get; set; }
    public EnemyChaseSOBase enemyChaseBaseInstance { get; set; }
    public EnemyAttackSOBase enemyAttackBaseInstance { get; set; }
    #endregion

    #region 状态机变量
    public EnemyStateMachine stateMachine { get; set; }
    public EnemyIdleState idleState { get; set; }
    public EnemyChaseState chaseState { get; set; }
    public EnemyAttackState attackState { get; set; }

    #endregion

    public bool isAttacking;

    [Header("Collision Info")]//碰撞参数
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    //面向
    public int facingDir { get; private set; } = 1;
    public bool facingRight = true;


    protected virtual void Awake()
    {
        enemyIdleBaseInstance = Instantiate(enemyIdleBase);
        enemyChaseBaseInstance = Instantiate(enemyChaseBase);
        enemyAttackBaseInstance = Instantiate(enemyAttackBase);

        stateMachine = new EnemyStateMachine();
        idleState = new EnemyIdleState(this,stateMachine);
        chaseState = new EnemyChaseState(this,stateMachine);
        attackState = new EnemyAttackState(this,stateMachine);
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        enemyIdleBaseInstance.Initialize(gameObject, this);
        enemyChaseBaseInstance.Initialize(gameObject, this);
        enemyAttackBaseInstance.Initialize(gameObject, this);

        stateMachine.Initialize(idleState);
    }

    protected virtual void Update()
    {
        //Debug.Log(stateMachine.currentEnemyState);
        stateMachine.currentEnemyState.UpdateState();
    }

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
    EnemyDamaged,
    PlayFootstepSound
}
#endregion
