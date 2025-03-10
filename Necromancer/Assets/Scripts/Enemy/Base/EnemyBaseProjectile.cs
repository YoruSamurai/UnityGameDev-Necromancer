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
        // 如果碰撞到玩家
        Debug.Log(hit.tag);
        if (hit.CompareTag("Player"))
        {
            hasHit = true;
            Player player = hit.GetComponent<Player>();
            if (player != null && !player.isInvincible)
            {
                int finalDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);
                Debug.Log($"射中了玩家，造成 {finalDamage} 点伤害");
                // 可根据需求，子弹命中玩家后立即销毁
                Destroy(gameObject, destroyDelay);
            }
        }
        // 如果碰撞到其他物体
        else
        {
            // 只处理第一次碰撞，避免重复嵌入
            if (!hasHit)
            {
                hasHit = true;
                Debug.Log("子弹嵌在了目标上");
                // 停止运动
                rb.velocity = Vector2.zero;
                // 禁用碰撞器，防止再次检测（或将其设为非触发模式）
                if (boxCollider != null)
                {
                    boxCollider.enabled = false;
                }
                // 可选：将子弹父对象设为碰撞物，使其随目标移动
                // transform.SetParent(hit.transform);
                // 延时1秒后销毁子弹
                Destroy(gameObject, .3f);
            }
        }
    }

}
