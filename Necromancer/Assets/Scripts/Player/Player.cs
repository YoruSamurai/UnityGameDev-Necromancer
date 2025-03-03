using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{

    //攻击时候的僵直状态
    public bool isBusy {  get; private set; }

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

    

    //玩家面向
    public int facingDir { get; private set; } = 1;
    private bool facingRight = true;

    #region Components
    public Animator anim {  get; private set; }
    public AnimatorOverrideController defaultAnimator {  get; private set; }
    public Rigidbody2D rb { get; private set; }

    #endregion


    #region States

    public PlayerStateMachine stateMachine {  get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerFallState fallState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerPrimaryAttack primaryAttack { get; private set; }

    #endregion


    protected override void Awake()
    {
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine,"Idle");
        moveState = new PlayerMoveState(this, stateMachine,"Move");
        jumpState = new PlayerJumpState(this, stateMachine,"Jump");
        fallState = new PlayerFallState(this, stateMachine,"Jump");
        dashState = new PlayerDashState(this, stateMachine,"Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine,"WallSlide");
        primaryAttack = new PlayerPrimaryAttack(this, stateMachine,"Attack");
    }

    protected override void Start()
    {
        anim = GetComponentInChildren<Animator>();
        defaultAnimator = new AnimatorOverrideController(anim.runtimeAnimatorController);
        rb = GetComponent<Rigidbody2D>();

        stateMachine.Initialize(idleState);//初始化idle状态
    }

    protected override void Update()
    {
        stateMachine.currentState.Update();//在每一帧只对当前的状态进行update

        CheckForDashInput();
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

    public void AnimationTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }


    //获取冲刺输入
    private void CheckForDashInput()
    {
        dashUsageTimer -= Time.deltaTime;

        //按下shift 设置timer为设置好的cd 并获取冲刺方向 进入冲刺状态
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashUsageTimer < 0)
        {
            dashUsageTimer = dashCooldown;
            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
                dashDir = facingDir;

            stateMachine.ChangeState(dashState);
        }
    }

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

    //设置跳跃计数器
    public void SetJumpCounter(int _jumpCounter)
    {
        jumpCounter = _jumpCounter;
    }

    //通过射线检测能不能射到地面，
    public bool IsGroundDetected()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    //通过射线检测能不能射到墙上
    public bool IsWallDetected()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    //通过debug射线绘制一些可视化
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }

    //翻转
    public void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    //根据X是否翻转
    public void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if(_x < 0 && facingRight)
            Flip();
    }

}
