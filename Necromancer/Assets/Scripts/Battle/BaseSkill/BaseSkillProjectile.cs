using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSkillProjectile : MonoBehaviour
{
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float projectileMaxDistance;
    [SerializeField] protected float projectileMaxTimer;
    [SerializeField] protected float projectileGravity;
    [SerializeField] protected float projectileAngle;


    [SerializeField] protected Vector2 startPosition;
    [SerializeField] protected bool hasHit;
    [SerializeField] protected bool isFacingRight;
    [SerializeField] protected float destroyDelay = .03f;
    [SerializeField] protected Collider2D boxCollider;
    [SerializeField] protected Rigidbody2D rb;

    private float lifetime;

    public void Initialize(float _projectileSpeed,
                            float _projectileMaxDistance,
                            float _projectileMaxTimer,
                            float _projectileGravity,
                            float _projectileAngle,
                            bool _isFacingRight)
    {
        // 初始化参数
        projectileSpeed = _projectileSpeed;
        projectileMaxDistance = _projectileMaxDistance;
        projectileGravity = _projectileGravity;
        projectileAngle = _projectileAngle;
        projectileMaxTimer = _projectileMaxTimer;
        isFacingRight = _isFacingRight;

        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError("没有找到 Rigidbody2D！");

        // 设置方向
        float direction = isFacingRight ? 1f : -1f;
        float angleInRadians = _projectileAngle * Mathf.Deg2Rad;
        Vector2 velocity = new Vector2(
            direction * projectileSpeed * Mathf.Cos(angleInRadians),
            projectileSpeed * Mathf.Sin(angleInRadians)
        );

        rb.velocity = velocity;
        rb.gravityScale = projectileGravity;

        // 处理翻转
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.flipY = !isFacingRight;
        }

        startPosition = transform.position;
        lifetime = 0f;
    }
        
    private void Update()
    {
        // 检测超时或超距离
        lifetime += Time.deltaTime;
        if (lifetime >= projectileMaxTimer || Vector2.Distance(startPosition, transform.position) >= projectileMaxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        /*if (!hasHit)
        {
            hasHit = true;
            Debug.Log("命中敌人：" + hit.name);
            Destroy(gameObject, destroyDelay);
        }*/
    }
}
