using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;
using DG.Tweening.Core.Easing;

public class Player : MonoBehaviour
{

    //攻击时候的僵直状态
    public bool isBusy {  get; private set; }

    [Header("当前接触梯子")]
    [SerializeField] public Ladder currentLadder = new Ladder();

    [Header("玩家拖尾效果")]
    [SerializeField] public GameObject dashTrail;

    [Header("无敌，真是一个好东西")]
    [SerializeField] private bool _isInvincible = false;  // 改用字段
    public bool isInvincible { get => _isInvincible; set => _isInvincible = value; }

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

    [Header("当按下S + 空格的时候，会进入0.05f的时间，在这个时间内碰撞的单向平台会被取消碰撞")]
    public float dropTimer;

    [Header("进入攀爬状态的时候，会进入.1f的时间，在这个时间内不能进行下冲和跳跃，相当于僵直")]
    public float climbTimer;

    [Header("每当离开攀爬 要经过亿点点时间才能再进入攀爬 就像人生一样")]
    public float climbCooldownTimer;

    [Header("当前碰撞梯子位置 顶部1 底部-1 其他0")]
    [SerializeField]public int currentLadderPosition;


    //可以爬梯子 和正在爬梯子 不一样
    [Header("Climb Info " +
        "isOnLadder表示玩家是否和梯子物体有碰撞" +
        "isClimbing表示玩家是否在攀爬状态")]
    [SerializeField] private bool _isOnLadder = false;  // 改用字段
    [SerializeField] private bool _isClimbing = false;  // 改用字段

    // 提供公共访问方式（可选）
    public bool isOnLadder { get => _isOnLadder; set => _isOnLadder = value; }
    public bool isClimbing { get => _isClimbing; set => _isClimbing = value; }

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

    [Header("FX预制体")]
    [SerializeField] private GameObject fxPrefab;

    //冲刺方向
    public float dashDir { get; private set; }

    // 创建一个包含多个Layer的LayerMask
    [Header("组合碰撞Layer，包含了Ground和单向平台图层")]
    [SerializeField] public int combinedGroundLayers;

    //面向
    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    [Header("Collision Info")]//碰撞参数
    [SerializeField] public Transform groundCheck;
    [SerializeField] public float groundCheckDistance;
    [SerializeField] public Transform wallCheckBody;
    [SerializeField] public Transform wallCheckHead;
    [SerializeField] public Transform wallCheckFoot;
    [SerializeField] public float wallCheckDistance;
    [SerializeField] public LayerMask whatIsGround;
    [SerializeField] public LayerMask whatIsOneWayPlatform;


    #region 单向平台处理

    //此处存储了当前需要忽略碰撞的所有单向平台
    [Header("Platform Management")]
    public List<Collider2D> ignoredPlatforms = new List<Collider2D>();
    //对列表进行增减和清空LIST 很好理解
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
    #endregion

    

    #region Components
    public Animator anim { get; private set; }
    public AnimatorOverrideController defaultAnimator { get; private set; }
    public Rigidbody2D rb { get; private set; }

    #endregion



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
    public PlayerCrouchingState crouchingState { get; private set; }

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
        crouchingState = new PlayerCrouchingState(this, stateMachine, "Crouching");
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
        CheckForDashInput();
        CheckForDownDash();
        CheckForClimbInput();
        stateMachine.currentState.Update();//在每一帧只对当前的状态进行update

        
        
    }

    protected void FixedUpdate()
    {
        stateMachine.currentState.FixedUpdate();
    }

    #region Trigger处理

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            Debug.Log("梯子！！");
            isOnLadder = true;
            // 1. 获取第一个碰撞接触点（世界坐标）
            Vector2 contactPoint = collision.ClosestPoint(transform.position);
            currentLadder = currentLadder.GetLadderInWorld(contactPoint, collision);
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            if(currentLadder.IsLadderExist())
            {
                if (transform.position.y > currentLadder.ladderTopY)
                    currentLadderPosition = 1;
                else if (transform.position.y - 3f < currentLadder.ladderBottomY)
                    currentLadderPosition = -1;
                else
                    currentLadderPosition = 0;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = false;
            currentLadderPosition = 0;
            currentLadder.ClearLadder();
            //isClimbing = false;
            // 如果玩家正处于攀爬状态，则退出
            if (stateMachine.currentState is PlayerClimbState)
            {
                stateMachine.ChangeState(idleState);
            }
        }
        
    }

    

    #endregion

    public void InitialFxPrefab(AnimationClip slashClip, Vector3 offset)
    {
        // 实例化 fxPrefab，假设 fxPrefab 定义在 Player 中并在 Inspector 设置
        GameObject fxInstance = Instantiate(fxPrefab, transform.position, Quaternion.identity,transform);

        // 获取 fxInstance 上的 FXSlash 组件并调用 Initialize
        FxController fxSlash = fxInstance.GetComponent<FxController>();
        if (fxSlash != null)
        {
            fxSlash.Initialize(slashClip, offset,facingRight);
        }
        else
        {
            Debug.LogError("FXSlash 脚本未挂载在 fxPrefab 上！");
            Destroy(fxInstance);
        }
    }

    #region 使用武器时候的动画器设置

    public void ApplyWeaponAnimator(AnimatorOverrideController weaponAnimator)
    {
        anim.runtimeAnimatorController = weaponAnimator;
    }

    public void ResetToDefaultAnimator()
    {
        anim.runtimeAnimatorController = defaultAnimator;
    }

    #endregion

    #region 判断冲突状态或条件
    //对PlayerState进行判断 都通过才返回false
    public bool IsInForbiddenState(params PlayerState[] excludedStates)
    {
        // 基础禁止条件
        bool isForbidden = false;
            

        // 检查当前状态是否在排除列表中
        if (!isForbidden && excludedStates != null)
        {
            foreach (var stateType in excludedStates)
            {
                if (stateMachine.currentState == stateType)
                {
                    isForbidden = true;
                    break;
                }
            }
        }

        return isForbidden;
    }

    //相同就返回true
    public bool IsInState(PlayerState state)
    {
        if(stateMachine.currentState == state)
            return true;
        return false;
    }

    public bool IsUsingEquipment()
    {
        if(PlayerStats.Instance.isAttacking || PlayerStats.Instance.isParrying || PlayerStats.Instance.isDefensing)
            return true;
        return false;
    }

    public bool IsCanInterrupt()
    {
        if (!PlayerStats.Instance.canInterrupt || PlayerStats.Instance.isParrying || PlayerStats.Instance.isDefensing)
            return false;
        return true;
    }

    #endregion


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

    #region 在每个Update中的输入检查 攀爬 翻滚 下冲

    private void CheckForClimbInput()
    {
        if (climbCooldownTimer > 0)
        {
            climbCooldownTimer -= Time.deltaTime;
        }
        if (climbTimer > 0)
        {
            climbTimer -= Time.deltaTime;
        }
        //进行判断
        {

            //不在梯子上或者还不能爬 就不用看了 
            if (!isOnLadder  || climbCooldownTimer > 0)
            {
                return;
            }
            PlayerState[] forbiddenStates = { dashState, downDashState };
            if (IsInForbiddenState(forbiddenStates))
            {
                return;
            }
            if (IsUsingEquipment())
            {
                return;
            }
            if (isClimbing)
            {
                return;
            }
            if (IsInState(oneWayState))
            {
                return;
            }
        }
        // 不在爬的时候 接收到垂直输入 此时不在爬单向平台 则开始爬
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            if(Input.GetAxisRaw("Vertical") > 0)//上爬
            {
                if(currentLadderPosition != 1)//判断能不能上爬
                {
                    isClimbing = true;
                    stateMachine.ChangeState(climbState);
                }
            }
            else//往下爬
            {
                dropTimer = .05f;
                if (!IsGroundDetected() && currentLadderPosition != -1)//判断能不能下爬
                {
                    isClimbing = true;
                    stateMachine.ChangeState(climbState);
                }
            }
        }
    }


    //获取冲刺输入
    private void CheckForDashInput()
    {
        dashUsageTimer -= Time.deltaTime;
        if (!IsCanInterrupt())
        {
            return ;
        }
        if (IsInState(downDashState))
        {
            return;
        }
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

    private void CheckForDownDash()
    {
        if (dropTimer > 0)
        {
            dropTimer -= Time.deltaTime;
        }
        if (IsUsingEquipment())
        {
            return;
        }
        if (IsGroundDetected())
        {
            return;
        }
        if(climbTimer > 0)
        {
            return;
        }
        // 确保玩家在空中且按下S+空格
        if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.Space))
        {
            //向下5f检测 没有地面就下冲
            float checkDistance = 5f;
            Vector2 origin = transform.position;
            Vector2 direction = Vector2.down;
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, checkDistance, whatIsGround);
            if (hit.collider == null)
            {
                stateMachine.ChangeState(downDashState);
            }
        }
    }

    #endregion

    //设置跳跃计数器
    public void SetJumpCounter(int _jumpCounter)
    {
        jumpCounter = _jumpCounter;
    }

    #region 残影设置

    public void CreateAfterImage(float interval)
    {
        StartCoroutine(CreateAfterImage(this, interval));
    }

    private IEnumerator CreateAfterImage(Player player,float interval)
    {
        SpriteRenderer sprite = player.GetComponentInChildren<SpriteRenderer>();
        float afterImageDuration = 0.5f; // 残影存活时间

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

    #endregion

    #region 拖尾效果

    public void StopTrail(GameObject trailObj, float duration)
    {
        StartCoroutine(FadeTrailRenderer(trailObj, duration));
    }

    private IEnumerator FadeTrailRenderer(GameObject trail, float duration)
    {
        float timer = 0f;
        ParticleSystem trailRenderer = trail.GetComponent<ParticleSystem>();
        // 停止发射新粒子
        trailRenderer.Stop();

        while (timer < duration)
        {
            float t = timer / duration;
            float alpha = Mathf.Lerp(1f, 0f, t);

            timer += Time.deltaTime;
            yield return null;
        }

        // 完全隐藏
        Destroy(trail);
    }

    #endregion


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



