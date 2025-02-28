using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 烈焰之魂子弹 : MonoBehaviour
{
    [SerializeField] private 烈焰之魂 equipment;


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

        rb.gravityScale = projectileGravity;
        circleCollider.radius = projectileRadius;
    }


    private void Update()
    {
        if (hasHit) return;

        // 根据 isFacingRight 设置子弹的速度方向
        Vector2 velocity = isFacingRight ? Vector2.right : Vector2.left;
        rb.velocity = velocity * projectileSpeed;

        // 反转子弹的 sprite 方向
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !isFacingRight;
        }

        // 检查是否达到最大距离
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
            Destroy(gameObject, .03f);
        }
    }

}
