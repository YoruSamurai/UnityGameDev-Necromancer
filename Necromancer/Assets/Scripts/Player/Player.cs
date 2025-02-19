using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

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
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;

    //玩家面向
    public int facingDir { get; private set; } = 1;
    private bool facingRight = true;

    #region Components
    public Animator anim {  get; private set; }
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

    #endregion


    private void Awake()
    {
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine,"Idle");
        moveState = new PlayerMoveState(this, stateMachine,"Move");
        jumpState = new PlayerJumpState(this, stateMachine,"Jump");
        fallState = new PlayerFallState(this, stateMachine,"Jump");
        dashState = new PlayerDashState(this, stateMachine,"Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine,"WallSlide");
    }

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine.Initialize(idleState);//初始化idle状态
    }

    private void Update()
    {
        stateMachine.currentState.Update();//在每一帧只对当前的状态进行update

        //用于测试碰撞 后续删掉
        {
            Debug.Log(IsGroundDetected());
            // 射线起点为当前物体的位置
            Vector2 origin = transform.position;
            // 射线方向为向下
            Vector2 direction = Vector2.down;
            // 射线长度
            float distance = 10f;

            // 发射射线，获取命中的所有物体
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance);

            // 遍历所有命中的物体
            foreach (RaycastHit2D hit in hits)
            {
                Debug.Log("Hit object: " + hit.collider.name);
            }

            // 在 Scene 视图中绘制射线，方便调试
            Debug.DrawRay(origin, direction * distance, Color.red);
        }
        



        CheckForDashInput();
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
