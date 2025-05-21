using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectileGenerator : MonoBehaviour
{
    [SerializeField] private ProjectileSO projectileSO;

    [SerializeField] private float damageMultiplier;

    [SerializeField] private List<Transform> hasHittedTargetList;

    private Player player;
    private Enemy enemy;
    private BaseEquipment equipment;
    public bool facingRight { get; private set; }
    private int combo;

    [SerializeField] public int maxHitNum;
    [SerializeField] public int currentHitNum;

    [SerializeField] private ProjectileTargetType projectileTargetType;
    [SerializeField] private ProjectileParentType projectileParentType;

    [SerializeField] public ProjectileDestroyType projectileDestroyType;
    [SerializeField] public ProjectileRecoverType projectileRecoverType;

    [SerializeField] public ProjectileEffectGenerateType projectileEffectGenerateType;

    [SerializeField] public bool canStickInGround;
    [SerializeField] public float stickTime;

    [SerializeField] private GameObject Prefab;
    [SerializeField] private GameObject TrailPrefab;
    [SerializeField] private GameObject HitEffectPrefab;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject projectileTrailPrefab;


    private ProjectileController projectileController;
    private ProjectileDamager projectileDamager;
    private ProjectileTrigger projectileTrigger;
    private ProjectileTargetFinder projectileTargetFinder;

    private ProjectileEffectHandler projectileEffectHandler;

    private ProjectileDestructionHandler projectileDestructionHandler;

    /// <summary>
    /// 初始化 子弹生成器 先初始化去获得一些生成子弹的信息 然后开始生成子弹及其组件
    /// </summary>
    /// <param name="_projectileSO"></param>
    /// <param name="_baseEquipment"></param>
    /// <param name="facingRight"></param>
    /// <param name="_enemy"></param>
    /// <param name="combo"></param>
    public void Initialize(ProjectileSO _projectileSO, BaseEquipment _baseEquipment, bool facingRight, Enemy _enemy, int combo)
    {

        InitializeGenerator(_projectileSO, _baseEquipment, facingRight, _enemy, combo);
        InitializeProjectile();

    }


    private void InitializeGenerator(ProjectileSO _projectileSO, BaseEquipment _baseEquipment, bool _facingRight, Enemy _enemy, int _combo)
    {
        projectileDestructionHandler = GetComponent<ProjectileDestructionHandler>();
        projectileDestructionHandler.Initialize(this);

        projectileSO = _projectileSO;


        damageMultiplier = projectileSO.damageMultiplier;

        hasHittedTargetList = new List<Transform>();


        canStickInGround = projectileSO.canStickInGround;
        stickTime = projectileSO.stickTime;


        maxHitNum = projectileSO.maxHitNum;
        currentHitNum = 0;


        projectileTargetType = projectileSO.projectileTargetType;
        projectileParentType = projectileSO.projectileParentType;
        projectileDestroyType = projectileSO.projectileDestroyType;
        projectileRecoverType = projectileSO.projectileRecoverType;

        projectileEffectGenerateType = projectileSO.projectileEffectGenerateType;


        facingRight = _facingRight;
        player = PlayerStats.Instance.player;

        if (projectileTargetType == ProjectileTargetType.Enemy)
        {
            enemy = _enemy;
        }
        else if (projectileTargetType == ProjectileTargetType.Player)
        {
            combo = _combo;
            facingRight = _facingRight;
            equipment = _baseEquipment;
        }

    }

    /// <summary>
    /// 生成子弹 及其组件
    /// </summary>
    private void InitializeProjectile()
    {
        Vector3 offset = new Vector3(facingRight? projectileSO.projectileOffset.x : - projectileSO.projectileOffset.x, projectileSO.projectileOffset.y, 0);
        projectilePrefab = Instantiate(
            Prefab,
            projectileTargetType == ProjectileTargetType.Player ? 
            enemy.transform.position + offset : player.transform.position + offset,
            Quaternion.identity
            );

        projectileController = projectilePrefab.GetComponent<ProjectileController>();
        projectileDamager = projectilePrefab.GetComponent<ProjectileDamager>();
        projectileTrigger = projectilePrefab.GetComponent<ProjectileTrigger>();
        projectileTargetFinder = projectilePrefab.GetComponent<ProjectileTargetFinder>();

        projectileController.Initialize(this, projectileSO, this.transform);
        projectileDamager.Initialize(this);
        projectileTrigger.Initialize(this, projectileSO);
        projectileTargetFinder.Initialize(this,projectileSO);

    }

    /// <summary>
    /// 在命中之后该干什么？ 
    /// 1:造成伤害 2:添加命中数 如果超过最大值就销毁一切
    /// 3:如果是索敌型 找到新的目标 如果找到了 就更改 PC的运动模式
    /// </summary>
    /// <param name="targets"></param>
    public void OnHit(List<Transform> targets)
    {
        if (targets.Count == 0) return;
        foreach (Transform target in targets)
        {
            projectileDamager.ApplyDamage(target);
            
            currentHitNum += 1;
            AddHasHittedTarget(target);
            if (maxHitNum != -1 &&  currentHitNum >= maxHitNum)
            {
                DestroyProjectile();
            }

            Transform newTarget = projectileTargetFinder.TryGetNewTransform(target);
            if(newTarget != null)
            {
                projectileController.ChangeMovingTypeToTrack(ProjectileMovingType.Track,newTarget);
            }

        }
    }


    public bool CanStillHit()
    {
        return maxHitNum == -1 || currentHitNum < maxHitNum;
    }

    /// <summary>
    /// 生成特效
    /// </summary>
    /// <param name="targets"></param>
    public void InitializeEffect(List<Transform> targets,Vector3 projectilePosition)
    {

        if(HitEffectPrefab != null)
        {
            if(projectileEffectGenerateType == ProjectileEffectGenerateType.Target)
            {
                Debug.Log("生成特效！");
                foreach (Transform target in targets)
                {
                    GameObject obj = Instantiate(
                        HitEffectPrefab,
                        target.position,
                        Quaternion.identity,
                        target
                        );
                    projectileEffectHandler = obj.GetComponent<ProjectileEffectHandler>();

                    projectileEffectHandler.Initialize(this);
                }
            }
            else if(projectileEffectGenerateType == ProjectileEffectGenerateType.HitPoint)
            {

            }

        }

    }

    public void Stick(Transform projectile,Transform newParent,Vector2 lastGroundHitPoint)
    {
        if (canStickInGround)
        {
            //projectile.transform.parent = newParent;
            //projectile.transform.position = lastGroundHitPoint;
            projectileController.Stick(lastGroundHitPoint);
            StartCoroutine(WaitForDestroy(stickTime));
        }

    }

    private IEnumerator WaitForDestroy(float stickTime)
    {
        yield return new WaitForSeconds(stickTime);
        DestroyProjectile();
    }

    /// <summary>
    /// 在最后发现要摧毁子弹的时候 停止协程并摧毁一切
    /// </summary>
    public void DestroyProjectile()
    {
        StopAllCoroutines();
        projectileDestructionHandler.DestructionHandle(projectilePrefab,projectileTrailPrefab);
    }

    #region GetSetAdd方法

    public ProjectileTargetType GetProjectileTargetType()
    {
        return projectileTargetType; 
    }

    public void AddHasHittedTarget(Transform target)
    {
        if (target == null)
            return;
        if(!hasHittedTargetList.Contains(target))
            hasHittedTargetList.Add(target);
    }

    public bool IsTargetInHittedList(Transform target)
    {
        if(target == null)
            return false;
        if(hasHittedTargetList.Contains(target))
            return true;
        return false;
    }

    public bool GetIsStick()
    {
        return projectileController.isStick;
    }

    public ProjectileTargetFinderType GetProjectileTargetFinderType()
    {
        return projectileTargetFinder.projectileTargetFinderType;
    }


    #endregion

}
