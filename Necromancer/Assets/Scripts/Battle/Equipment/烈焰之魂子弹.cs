using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 烈焰之魂子弹 : MonoBehaviour
{
    [SerializeField] private 烈焰之魂 equipment;
    [SerializeField] private GameObject explosionPrefab; // 爆炸特效预制体

    [SerializeField] private float attackMag;
    [SerializeField] private float attackStun;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileMaxDistance;
    [SerializeField] private float projectileMaxTimer;
    [SerializeField] private float projectileGravity;
    [SerializeField] private float projectileRadius;


    [SerializeField] private Vector2 startPosition;
    [SerializeField] private bool hasHit;
    [SerializeField] private bool isFacingRight;
    [SerializeField] private float destroyDelay = .03f;
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private Rigidbody2D rb;


    public void Initialize(烈焰之魂 _equipment, StaffAttackStruct _staffAttackStruct,bool _isFacingRight)
    {
        equipment = _equipment;
        attackMag = _staffAttackStruct.attackMag;
        attackStun = _staffAttackStruct.attackStun;
        projectileSpeed = _staffAttackStruct.projectileSpeed;
        projectileMaxDistance = _staffAttackStruct.projectileMaxDistance;
        projectileMaxTimer = _staffAttackStruct.projectileMaxTimer;
        projectileGravity = _staffAttackStruct.projectileGravity;
        projectileRadius = _staffAttackStruct.projectileRadius;

        isFacingRight = _isFacingRight;

        circleCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = projectileGravity;
        circleCollider.radius = projectileRadius;

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


    private void Update()
    {
        if (hasHit) return;

        // 确保子弹始终有水平速度
        rb.velocity = new Vector2((isFacingRight ? 1f : -1f) * projectileSpeed, rb.velocity.y);

        // 检查是否达到最大飞行距离
        if (Vector2.Distance(startPosition, transform.position) >= projectileMaxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hasHit) return;
        if (((1 << hit.gameObject.layer) & PlayerStats.Instance.whatIsEnemy) != 0)
        {
            hasHit = true;
            MonsterStats monsterStats = hit.GetComponent<MonsterStats>();
            equipment.HandleProjectileHit(monsterStats);
            // 生成爆炸特效
            if (explosionPrefab != null)
            {
                GameObject explosion = Instantiate(explosionPrefab, hit.transform.position, Quaternion.identity);
                ParticleSystem ps = explosion.GetComponentInChildren<ParticleSystem>();

                if (ps != null)
                {
                    var shape = ps.shape;
                    shape.shapeType = ParticleSystemShapeType.Rectangle; // 形状改为矩形
                    //shape.scale = new Vector3(10f, 10f, 1f); // 形状缩放 10x10

                    var emission = ps.emission;
                    //emission.rateOverTimeMultiplier *= 10f; // 发射率放大 10 倍
                }

                Destroy(explosion, 1f); // 1秒后销毁爆炸特效
            }
            Destroy(gameObject, .03f);
        }
    }

}
