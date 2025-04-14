using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 苦难雕像子弹 : EnemyBaseProjectile
{
    public float randomPhaseDuration = 0.3f;
    public float randomYVariance = 1f;

    // 添加变量来表示是否正在过渡中
    private bool isInterpolating = false;
    private float interpolationTime = 0.2f; // 插值持续时间
    private float interpolationTimer = 0f;

    private float creationTime;
    private Vector2 lockedDirection;
    private bool hasLockedDirection = false;

    private Vector2 initialRandomVelocity;
    private Transform playerTransform;

    private bool isTurning = false;
    private float turnSpeed = 360f; // 每秒最大旋转角度（可调）
    private float angleThreshold = 1f; // 小于这个角度就开始直线冲刺

    protected void Start()
    {
        creationTime = Time.time;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // 随机初始方向：水平+随机垂直
        float randomYOffset = Random.Range(-randomYVariance, randomYVariance);
        initialRandomVelocity = new Vector2(0, 15f);
        rb.velocity = initialRandomVelocity;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (movingType == ProjectileMovingType.A)
        {
            if (hasHit) return;

            float elapsed = Time.time - creationTime;

            // 第一阶段：随机方向飞行
            if (elapsed < randomPhaseDuration)
            {
                rb.velocity = initialRandomVelocity;
            }
            // 第二阶段：锁定玩家方向后飞行
            else
            {
                if (!hasLockedDirection)
                {
                    if (playerTransform != null)
                    {
                        Vector2 toPlayer = (playerTransform.position - transform.position).normalized;
                        lockedDirection = toPlayer;
                        isTurning = true;
                    }
                    else
                    {
                        lockedDirection = rb.velocity.normalized;
                        isTurning = false;
                    }

                    hasLockedDirection = true;
                }

                if (isTurning)
                {
                    Vector2 currentDir = rb.velocity.normalized;
                    float currentAngle = Mathf.Atan2(currentDir.y, currentDir.x) * Mathf.Rad2Deg;
                    float targetAngle = Mathf.Atan2(lockedDirection.y, lockedDirection.x) * Mathf.Rad2Deg;

                    float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, turnSpeed * Time.fixedDeltaTime);
                    Vector2 newDir = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                    rb.velocity = newDir.normalized * projectileSpeed;

                    // 检查是否对准目标方向
                    float angleDiff = Vector2.Angle(newDir, lockedDirection);
                    if (angleDiff <= angleThreshold)
                    {
                        isTurning = false; // 进入直线阶段
                    }
                }
                else
                {
                    rb.velocity = lockedDirection * projectileSpeed;
                }
            }

            // 最大飞行距离判定
            if (Vector2.Distance(startPosition, transform.position) >= projectileMaxDistance)
            {
                Destroy(gameObject);
            }

            // 朝向速度方向
            if (rb.velocity != Vector2.zero)
            {
                float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }
}
