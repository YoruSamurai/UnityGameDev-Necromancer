using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //攻击时候的僵直状态
    public bool isBusy {  get; private set; }

    //无敌状态
    public bool isInvincible { get; private set; } = false;

    //可以爬梯子 和正在爬梯子 不一样
    public bool isOnLadder { get; set; } = false;
    public bool isClimbing { get; set; } = false;

    [Header("Move Info")]//移动参数
    public float moveSpeed = 12f;
    public float jumpForce;

    [Header("Dash Info")]//冲刺参数
    [SerializeField] private float dashCooldown;
    private float dashUsageTimer;
    public float dashSpeed;
    public float dashDuration;

    //二段跳
    [Header("Jump Info")]//跳跃参数
    [SerializeField] private int jumpTimes;
    [SerializeField] public int jumpCounter;

    //冲刺方向
    public float dashDir {  get; private set; }




    [Header("Collision Info")]//碰撞参数
    [SerializeField] public Transform groundCheck;
    [SerializeField] public float groundCheckDistance;
    [SerializeField] public Transform wallCheckBody;
    [SerializeField] public Transform wallCheckHead;
    [SerializeField] public Transform wallCheckFoot;
    [SerializeField] public float wallCheckDistance;
    [SerializeField] public LayerMask whatIsGround;
    [SerializeField] public LayerMask whatIsOneWayPlatform;

    #region Components
    public Animator anim { get; private set; }
    public AnimatorOverrideController defaultAnimator { get; private set; }
    public Rigidbody2D rb { get; private set; }

    #endregion

    //面向
    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;


    #region States

    public PlayerStateMachine stateMachine {  get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerFallState fallState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerClimbState climbState { get; private set; }
    public PlayerLedgeUpState ledgeUpState { get; private set; }
    public PlayerOneWayState oneWayState { get; private set; }
    public PlayerPrimaryAttack primaryAttack { get; private set; }
    public PlayerParryState parryState { get; private set; }
    public PlayerDefenseState defenseState { get; private set; }

    #endregion


    protected void Awake()
    {
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine,"Idle");
        moveState = new PlayerMoveState(this, stateMachine,"Move");
        jumpState = new PlayerJumpState(this, stateMachine,"Jump");
        fallState = new PlayerFallState(this, stateMachine,"Jump");
        dashState = new PlayerDashState(this, stateMachine,"Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine,"WallSlide");
        climbState = new PlayerClimbState(this, stateMachine, "Climb");
        ledgeUpState = new PlayerLedgeUpState(this, stateMachine, "LedgeUp");
        oneWayState = new PlayerOneWayState(this, stateMachine, "OneWay");
        primaryAttack = new PlayerPrimaryAttack(this, stateMachine,"Attack");
        parryState = new PlayerParryState(this, stateMachine,"Parry");
        defenseState = new PlayerDefenseState(this, stateMachine,"Defense");
    }

    protected void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        defaultAnimator = new AnimatorOverrideController(anim.runtimeAnimatorController);
        stateMachine.Initialize(idleState);//初始化idle状态
    }

    protected void Update()
    {
        stateMachine.currentState.Update();//在每一帧只对当前的状态进行update

        CheckForDashInput();
        CheckForClimbInput();
    }

    protected void FixedUpdate()
    {
        stateMachine.currentState.FixedUpdate();
    }

    private void CheckForClimbInput()
    {
        if (!isOnLadder)
            return;
        // 如果玩家有意按下垂直方向键，可以切换状态
        if (Input.GetAxisRaw("Vertical") != 0 && !isClimbing)
        {
            isClimbing = true;
            stateMachine.ChangeState(climbState); // 或者专门的攀爬状态
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = false;
            isClimbing = false;
            // 如果玩家正处于攀爬状态，则退出
            if (stateMachine.currentState is PlayerClimbState)
            {
                stateMachine.ChangeState(idleState);
            }
        }
    }


    public void ApplyWeaponAnimator(AnimatorOverrideController weaponAnimator)
    {
        anim.runtimeAnimatorController = weaponAnimator;
    }

    public void ResetToDefaultAnimator()
    {
        anim.runtimeAnimatorController = defaultAnimator;
    }

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(_seconds);

        isBusy = false;
    }

    public void SetInvincible(bool value)
    {
        isInvincible = value;
        // 你可以在这里触发视觉反馈（例如改变材质、闪烁效果等）
    }

    public void ChangeStateByPlayerStats(PlayerState _playerState)
    {
        if (_playerState == stateMachine.currentState || isBusy)
        {
            Debug.Log("进不去啊啊" + isBusy);
            return;
        }
        else
        {
            stateMachine.ChangeState(_playerState);
        }
        
    }

    public void AnimationTrigger(PlayerAnimationTriggerType triggerType)
    {
        stateMachine.currentState.AnimationTriggerEvent(triggerType);
    }


    //获取冲刺输入
    private void CheckForDashInput()
    {
        dashUsageTimer -= Time.deltaTime;

        //按下shift 设置timer为设置好的cd 并获取冲刺方向 进入冲刺状态
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashUsageTimer < 0
            && !PlayerStats.Instance.isAttacking && !PlayerStats.Instance.isParrying && !PlayerStats.Instance.isDefensing)
        {
            dashUsageTimer = dashCooldown;
            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
                dashDir = facingDir;

            stateMachine.ChangeState(dashState);
        }
    }

    

    //设置跳跃计数器
    public void SetJumpCounter(int _jumpCounter)
    {
        jumpCounter = _jumpCounter;
    }


    #region  Velocity
    public void SetZeroVelocity()
    {
        rb.velocity = new Vector2(0, 0);
    }

    //设置人物速度 并根据面向进行sprite的翻转
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
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
    public virtual bool IsWallBodyDetected()
    {
        return Physics2D.Raycast(wallCheckBody.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    public virtual bool IsWallHeadDetected()
    {
        return Physics2D.Raycast(wallCheckHead.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    public virtual bool IsWallFootDetected()
    {
        return Physics2D.Raycast(wallCheckFoot.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    //通过debug射线绘制一些可视化
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheckBody.position, new Vector3(wallCheckBody.position.x + wallCheckDistance, wallCheckBody.position.y));
        Gizmos.DrawLine(wallCheckHead.position, new Vector3(wallCheckHead.position.x + wallCheckDistance, wallCheckHead.position.y));
        Gizmos.DrawLine(wallCheckFoot.position, new Vector3(wallCheckFoot.position.x + wallCheckDistance, wallCheckFoot.position.y));
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


}
