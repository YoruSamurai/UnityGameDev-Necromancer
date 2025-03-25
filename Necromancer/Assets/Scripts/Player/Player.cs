using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{

    //攻击时候的僵直状态
    public bool isBusy {  get; private set; }

    //无敌状态
    public bool isInvincible { get; private set; } = false;

    //检测S+空格的计时器
    public float dropTimer;

    //当前梯子是顶部还是顶部
    [SerializeField]public int currentLadderPosition;


    //可以爬梯子 和正在爬梯子 不一样
    [SerializeField] public bool isOnLadder;
    //public bool isOnLadder { get; set; } = false;
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

    [Header("Platform Management")]
    public List<Collider2D> ignoredPlatforms = new List<Collider2D>();

    public void AddIgnoredPlatform(Collider2D platform)
    {
        if (!ignoredPlatforms.Contains(platform))
        {
            ignoredPlatforms.Add(platform);
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), platform, true);
            platform.gameObject.layer = LayerMask.NameToLayer("OneWayPlatform");
        }
    }

    public void RemoveIgnoredPlatform(Collider2D platform)
    {
        if (ignoredPlatforms.Contains(platform))
        {
            ignoredPlatforms.Remove(platform);
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), platform, false);
            platform.gameObject.layer = LayerMask.NameToLayer("Ground");
        }
    }

    public void ClearIgnoredPlatforms()
    {
        foreach (var platform in ignoredPlatforms.ToArray()) // 使用ToArray避免修改集合
        {
            RemoveIgnoredPlatform(platform);
        }
    }

    //冲刺方向
    public float dashDir {  get; private set; }

    // 创建一个包含多个Layer的LayerMask
    public int combinedGroundLayers;


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
    public PlayerDownDashState downDashState { get; private set; }
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
        combinedGroundLayers = LayerMask.GetMask("Ground", "OneWayPlatform");
        idleState = new PlayerIdleState(this, stateMachine,"Idle");
        moveState = new PlayerMoveState(this, stateMachine,"Move");
        jumpState = new PlayerJumpState(this, stateMachine,"Jump");
        fallState = new PlayerFallState(this, stateMachine,"Jump");
        downDashState = new PlayerDownDashState(this, stateMachine,"DownDash");
        dashState = new PlayerDashState(this, stateMachine,"Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine,"WallSlide");
        climbState = new PlayerClimbState(this, stateMachine, "Climb");
        ledgeUpState = new PlayerLedgeUpState(this, stateMachine, "LedgeUp");
        oneWayState = new PlayerOneWayState(this, stateMachine, "OneWay");
        primaryAttack = new PlayerPrimaryAttack(this, stateMachine,"Attack");
        parryState = new PlayerParryState(this, stateMachine,"Parry");
        defenseState = new PlayerDefenseState(this, stateMachine,"Defense");
        currentLadderPosition = 0;
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
        CheckForDownDash();
        if(dropTimer > 0)
        {
            dropTimer -= Time.deltaTime;
        }
    }

    protected void FixedUpdate()
    {
        stateMachine.currentState.FixedUpdate();
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            float ladderBottomY;
            float ladderTopY;
            // 获取梯子的底部和顶部 y 坐标
            ladderBottomY = collision.bounds.min.y; // 底部 y 坐标
            ladderTopY = collision.bounds.max.y;    // 顶部 y 坐标
            // 判断玩家相对于梯子的位置
            if (transform.position.y > ladderTopY)
                currentLadderPosition = 1;
            else if(transform.position.y - 3f < ladderBottomY)
                currentLadderPosition = -1;
            else
                currentLadderPosition = 0;
            
            //Debug.Log($"Ladder Bottom Y: {ladderBottomY}, Ladder Top Y: {ladderTopY}");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = false;
            currentLadderPosition = 0;
            //isClimbing = false;
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

    private void CheckForClimbInput()
    {
        if (!isOnLadder || stateMachine.currentState is PlayerDownDashState || stateMachine.currentState is PlayerDashState)
            return;
        // 如果玩家有意按下垂直方向键，可以切换状态
        if (Input.GetAxisRaw("Vertical") != 0 && !isClimbing && !(stateMachine.currentState is PlayerOneWayState))
        {
            if(Input.GetAxisRaw("Vertical") > 0)//上爬
            {
                if(currentLadderPosition != 1)
                {
                    isClimbing = true;
                    stateMachine.ChangeState(climbState); // 或者专门的攀爬状态1
                }
            }
            else//往下爬
            {
                dropTimer = .05f;
                if (!IsGroundDetected() && currentLadderPosition != -1)
                {
                    isClimbing = true;
                    stateMachine.ChangeState(climbState); // 或者专门的攀爬状态
                }
            }
        }
    }


    //获取冲刺输入
    private void CheckForDashInput()
    {
        dashUsageTimer -= Time.deltaTime;

        //按下shift 设置timer为设置好的cd 并获取冲刺方向 进入冲刺状态
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashUsageTimer < 0
            && !PlayerStats.Instance.isAttacking && !PlayerStats.Instance.isParrying && !PlayerStats.Instance.isDefensing)
        {
            Debug.Log("冲刺冲刺");
            dashUsageTimer = dashCooldown;
            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
                dashDir = facingDir;

            stateMachine.ChangeState(dashState);
        }
    }

    private void CheckForDownDash()
    {
        // 确保玩家在空中且按下S+空格
        if (!IsGroundDetected() && Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("你为何不下冲");
            // 发射一条5f长的向下射线
            float checkDistance = 5f;
            Vector2 origin = transform.position;
            Vector2 direction = Vector2.down;
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, checkDistance, whatIsGround);

            // 可视化射线
            Debug.DrawRay(origin, direction * checkDistance, Color.red);

            // 如果没有检测到地面，就切换到下冲状态
            if (hit.collider == null)
            {
                stateMachine.ChangeState(downDashState);
            }
        }
    }

    //设置跳跃计数器
    public void SetJumpCounter(int _jumpCounter)
    {
        jumpCounter = _jumpCounter;
    }

    public void CreateAfterImage()
    {
        StartCoroutine(CreateAfterImage(this));
    }

    private IEnumerator CreateAfterImage(Player player)
    {
        Debug.Log("生成残影吗");
        SpriteRenderer sprite = player.GetComponentInChildren<SpriteRenderer>();
        float afterImageDuration = 0.5f; // 残影存活时间
        float interval = 0.03f; // 残影生成间隔

        Color afterImageColor = new Color(1f, 1f, 1f, 0.5f); // 白色半透明残影
        List<GameObject> afterImages = new List<GameObject>();

        float timer = 0f;
        while (!player.IsGroundDetected())
        {
            // 创建残影对象
            GameObject afterImage = new GameObject("AfterImage");
            SpriteRenderer afterImageRenderer = afterImage.AddComponent<SpriteRenderer>();

            // 复制玩家的Sprite
            afterImageRenderer.sprite = sprite.sprite;
            afterImageRenderer.sortingLayerID = sprite.sortingLayerID;
            afterImageRenderer.sortingOrder = sprite.sortingOrder - 1; // 残影在玩家身后
            afterImageRenderer.color = afterImageColor;

            // 设置残影位置
            afterImage.transform.position = player.transform.position;
            afterImage.transform.rotation = player.transform.rotation;

            // 残影淡出效果
            afterImageRenderer.DOFade(0, afterImageDuration).OnComplete(() => GameObject.Destroy(afterImage));

            afterImages.Add(afterImage);

            yield return new WaitForSeconds(interval);
            timer += interval;
        }
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
    public bool IsCollisionDetected()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround) 
            || Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsOneWayPlatform);
    }

    public bool IsGroundDetected()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    }
    public bool IsOneWayDetected()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsOneWayPlatform);
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
