using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseProjectile : MonoBehaviour
{
    private Enemy enemy;
    [SerializeField] protected float baseDamage;  // 攻击伤害
    [SerializeField] protected float damageMultiplier;  // 攻击伤害倍率
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float projectileMaxDistance;
    [SerializeField] protected float projectileMaxTimer;
    [SerializeField] protected float projectileGravity;
    [SerializeField] protected float projectileAngle;


    [SerializeField] protected Vector2 startPosition;
    [SerializeField] protected bool hasHit;
    [SerializeField] protected bool isFacingRight;
    [SerializeField] protected float destroyDelay = .03f;
    [SerializeField] protected BoxCollider2D boxCollider;
    [SerializeField] protected Rigidbody2D rb;

    public void Initialize(Enemy enemy,float _baseDamage, float _damageMultiplier, float _projectileSpeed,
        float _projectileMaxDistance, float _projectileMaxTimer, float _projectileGravity,
        float _projectileAngle,bool _isFacingRight)
    {
        baseDamage = _baseDamage;
        damageMultiplier = _damageMultiplier;
        projectileSpeed = _projectileSpeed;
        projectileMaxDistance = _projectileMaxDistance;
        projectileGravity = _projectileGravity;
        projectileAngle = _projectileAngle;
        projectileMaxTimer = _projectileMaxTimer;
        isFacingRight = _isFacingRight;

        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = projectileGravity;

        // 初始速度（加上水平速度，重力会由 Rigidbody2D 自动处理）
        float xDirection = isFacingRight ? 1f : -1f;
        rb.velocity = new Vector2(xDirection * projectileSpeed, rb.velocity.y);

        // 设置 Sprite 方向
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !isFacingRight;
        }

        // 记录初始位置
        startPosition = transform.position;
    }

    public virtual void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        Debug.Log("射中了！！");
    }

}
