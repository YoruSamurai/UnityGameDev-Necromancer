using UnityEngine;

public class ProjectileController : MonoBehaviour
{

    private BaseProjectileGenerator generator;

    private float speed;
    private float lifetime;
    private float maxDistance;
    private float gravity;
    private float angle;
    private float radius;

    private ProjectileMovingType movingType;
    private Vector2 startPosition;
    private Vector2 direction;

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    public bool isStick { get; private set; }

    private Transform trackTarget;

    private float timeAlive;

    public void Initialize(BaseProjectileGenerator _generator,ProjectileSO _projectileSO,Transform _projectileParentTransform)
    {

        generator = _generator;

        isStick = false;

        speed = generator.facingRight? _projectileSO.projectileSpeed : -_projectileSO.projectileSpeed;
        lifetime = _projectileSO.projectileLifetime;
        maxDistance = _projectileSO.projectileMaxDistance;
        gravity = _projectileSO.projectileGravity;
        angle = _projectileSO.projectileAngle;
        movingType = _projectileSO.projectileMovingType;
        radius = _projectileSO.projectileRadius;

        startPosition = transform.position;

        circleCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = gravity;
        circleCollider.radius = radius;
        spriteRenderer.sprite = _projectileSO.projectileSprite;

        // 根据角度和发射者方向计算初始方向
        Vector2 baseDirection = _projectileParentTransform.right;
        Quaternion rot = Quaternion.Euler(0, 0, angle);
        direction = rot * baseDirection;

        ///如果后续有那种一生成就索敌的 可能需要修改这个
        trackTarget = null;


    }



    public void ChangeMovingTypeToTrack(ProjectileMovingType newMovingType,Transform newTarget)
    {
        movingType = newMovingType;
        trackTarget = newTarget;
    }


    private void FixedUpdate()
    {
        timeAlive += Time.fixedDeltaTime;

        // 生命周期结束 去XX地方调用其他逻辑 等待下一步处理
        if (timeAlive > lifetime || Vector2.Distance(startPosition, transform.position) > maxDistance)
        {
            generator.DestroyProjectile();
            return;
        }
        if (!isStick)
        {
            // 移动逻辑
            switch (movingType)
            {
                case ProjectileMovingType.Straight:
                    MoveStraight();
                    break;
                case ProjectileMovingType.A:
                    // 抛物线使用刚体，不在这里处理
                    break;
                case ProjectileMovingType.Track:
                    MoveTrack();
                    break;
            }
        }
    }

    public void Stick(Vector2 lastGroundHitPoint)
    {
        isStick = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.position = lastGroundHitPoint;
        Debug.Log(transform.position +"asdadad");
    }

    /// <summary>
    /// 以特定X速度 受自身设置的重力影响
    /// </summary>
    private void MoveStraight()
    {
        // 确保子弹始终有水平速度
        rb.velocity = new Vector2(speed, rb.velocity.y);
        RotateAngle();
    }

    private void MoveTrack()
    {
        if (trackTarget == null) Debug.LogWarning("没有可以追踪的目标");


        Vector2 directionToTarget = ((Vector2)trackTarget.position - (Vector2)transform.position).normalized;
        rb.velocity = directionToTarget * speed;
        RotateAngle();
    }

    private void RotateAngle()
    {
        Vector2 dir = rb.velocity.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }

}
