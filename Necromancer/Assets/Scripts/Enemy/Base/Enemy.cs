using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable,IEnemyMoveable,ITriggerCheckable
{
    [field: SerializeField] public int MaxHealth { get; set; } = 100;
    public int CurrentHealth { get; set; }
    public Rigidbody2D rb { get; set; }
    public Animator anim { get; private set; }
    public bool IsFacingRight { get; set; } = true;

    [field: SerializeField] public bool isAggroed { get; set; }
    [field: SerializeField] public bool isWithinStrikingDistance { get; set; }

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
        CurrentHealth = MaxHealth;
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

    #region 伤害
    public void Damage(int damageAmount)
    {
        CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("DIE");
    }
    #endregion

    #region 移动
    public void MoveEnemy(Vector2 velocity)
    {
        rb.velocity = velocity;
        CheckFacing(velocity);
    }

    public void CheckFacing(Vector2 velocity)
    {
        if(IsFacingRight && velocity.x < 0f)
        {
            Vector3 rotator = new Vector3(transform.rotation.x,180f,transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight; 
        }
        else if (!IsFacingRight && velocity.x > 0f)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
        }
    }
    #endregion

    private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        //fill in when statemachine was created
        stateMachine.currentEnemyState.AnimationTriggerEvent(triggerType);
    }

    #region 距离
    public void SetAggroStatus(bool isAggroed)
    {
        this.isAggroed = isAggroed;
    }

    public void SetStrikingDistanceBool(bool isWithinStrikingDistance)
    {
        this.isWithinStrikingDistance = isWithinStrikingDistance;
    }
    #endregion
}

#region 敌人枚举（意义还不明确）
public enum AnimationTriggerType
{
    EnemyDamaged,
    PlayFootstepSound
}
#endregion
