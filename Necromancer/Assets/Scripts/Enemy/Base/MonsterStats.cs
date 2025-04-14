using LDtkUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStats : MonoBehaviour
{

    #region Components
    public Enemy enemy { get; private set; }

    #endregion

    [Header("生命值")]
    public int maxHealth;
    public int currentHealth;
    [Header("基础伤害值")]
    public int baseDamage;
    [Header("眩晕抵抗值")]
    public int stunResistance;
    public int currentStunResistance;
    [Header("冻结抵抗值")]
    public int freezeResistance;
    public int currentFreezeResistance;
    [Header("眩晕时长")]
    public float stunDuration;
    public float currentStunTimer;
    [Header("冻结时长")]
    public float freezeDuration;
    public float currentFreezeTimer;


    #region 怪物当前状态
    public bool isDead {  get; private set; }
    [SerializeField] public bool isStunned;// { get; private set; }
    #endregion

    #region 怪物计时器

    private float stunTickInterval = 1f;
    #endregion


    [SerializeField] private List<PoisonEffect> poisonEffects = new List<PoisonEffect>();
    [SerializeField] private List<BurnEffect> burnEffects = new List<BurnEffect>();
    [SerializeField] private List<BleedEffect> bleedEffects = new List<BleedEffect>();

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        isDead = false;
        isStunned = false;
        StartStunResistanceDecay();
        /*LDtkComponentEntity entity = GetComponent<LDtkComponentEntity>();
        if (entity != null)
        {
            Debug.Log(entity.FieldInstances.GetInt("Unnamed"));

        }*/

    }

    private void Update()
    {
        UpdatePoisonEffects();
        CheckStun();
    }

    public void SetupMonsterStats(EnemyProfileSO profileSO)
    {
        maxHealth = profileSO.maxHealth;
        currentHealth = profileSO.maxHealth;
        baseDamage = profileSO.baseDamage;
        stunResistance = profileSO.stunResistance;
        currentStunResistance = 0;
        freezeResistance = profileSO.freezeResistance;
        currentFreezeResistance = 0;
        stunDuration = profileSO.stunDuration;
        currentStunTimer = 0;
        freezeDuration = profileSO.freezeDuration;
        currentFreezeTimer = 0;
    }

    #region 伤害判定检测 后续慢慢重构和优化
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                Debug.Log("玩家即将受到伤害");
                Debug.Log(enemy.currentDamageMultiplier * baseDamage);
                if (PlayerStats.Instance.isParrying)
                {
                    Debug.Log("我被招架了");
                    currentStunTimer = 2f;
                    enemy.ChangeToState(enemy.stunState);
                }
                if (PlayerStats.Instance.isDefensing)
                {
                    Debug.Log("我被防御了");
                    currentStunTimer = 1f;
                    enemy.ChangeToState(enemy.stunState);
                }
            }
        }
    }
    #endregion


    #region 怪物进行攻击 怪物命中 怪物被命中（无防御） 怪物被命中（防御中） 怪物死亡了 此时触发这些函数
    public void OnAttack()
    {
        
    }

    public void OnHit(BaseEquipment _baseEquipment)
    {
        
    }

    public void OnHitted()
    {

    }

    public void OnDefense()
    {

    }

    public void OnDeath()
    {
        isDead = true;
        gameObject.layer = LayerMask.NameToLayer("Dead");
        StopStunResistanceDecay();
        enemy.ChangeToState(enemy.dieState);
    }

    public void TakeDirectDamage(int dmg)
    {
        if(currentHealth > 0)
        {
            yoruUtils.JumpNumber(dmg, this.gameObject);
            enemy.anim.SetTrigger("Hit");
            enemy.OnHitted();
            currentHealth -= dmg;

            // 每次受到伤害调用 ProcessStunOnHit 来增加眩晕值
            ProcessStunOnHit();
        }
        if(currentHealth <=  0 && isDead == false)
        {
            OnDeath();
        }

    }
    #endregion

    #region 获取怪物各种状态
    public bool isInPoison()
    {
        if (poisonEffects.Count > 0)
        {
            return true;
        }
        return false;
    }
    public bool isInBurn()
    {
        if (burnEffects.Count > 0)
        {
            return true;
        }
        return false;
    }
    public bool isInBleed()
    {
        if(bleedEffects.Count > 0)
        {
            return true;
        }
        return false;
    }


    #endregion


    #region 眩晕属性
    private void CheckStun()
    {
        // 检查是否进入晕眩
        if (currentStunResistance > stunResistance && !isStunned)
        {
            EnterStun();
        }

        if (isStunned && currentStunTimer > 0)
        {
            currentStunTimer -= Time.deltaTime;
        }
        if (isStunned && currentStunTimer <= 0)
        {
            ExitStun();
        }

    }

    // 开启每秒减少3点耐性
    private void StartStunResistanceDecay()
    {
        if (!IsInvoking(nameof(DecreaseStunResistance)))
        {
            InvokeRepeating(nameof(DecreaseStunResistance), 0f, stunTickInterval);
        }
    }

    // 停止耐性衰减
    private void StopStunResistanceDecay()
    {
        if (IsInvoking(nameof(DecreaseStunResistance)))
        {
            CancelInvoke(nameof(DecreaseStunResistance));
        }
    }

    // 每秒减少3点耐性
    private void DecreaseStunResistance()
    {
        if (!isStunned && currentStunResistance > 0)
        {
            currentStunResistance -= 3;
            if (currentStunResistance < 0)
                currentStunResistance = 0;
        }
    }

    public void ProcessStunOnHit()
    {
        // 如果敌人已经处于眩晕状态，则不再叠加眩晕
        if (isStunned)
            return;

        // 每次受到伤害时增加5点眩晕值
        currentStunResistance += 10;
        Debug.Log("当前眩晕值增加到：" + currentStunResistance);
    }

    private void EnterStun()
    {
        isStunned = true;
        StopStunResistanceDecay(); // 眩晕时停止耐性衰减
        currentStunTimer = (currentStunResistance / (float)stunResistance) * stunDuration;
        // 进入眩晕状态时，可以调用 Enemy 的状态切换，例如：
        enemy.ChangeToState(enemy.stunState);
    }

    private void ExitStun()
    {
        isStunned = false;
        currentStunResistance = 0;
        currentStunTimer = 0;
        StartStunResistanceDecay(); // 恢复耐性衰减
        Debug.Log("眩晕结束");
    }
    #endregion

    #region 毒属性
    private void UpdatePoisonEffects()
    {
        if (poisonEffects.Count == 0) return;

        for (int i = poisonEffects.Count - 1; i >= 0; i--)
        {
            PoisonEffect poison = poisonEffects[i];
            poison.timer += Time.deltaTime;
            poison.elapsedTime += Time.deltaTime;

            // 毒层生效，每 1 秒触发伤害
            if (poison.timer >= poison.interval)
            {
                ApplyPoisonDamage(poison.damage);
                poison.timer = 0f;  // 重新计时
            }

            // 毒层持续时间到了，移除该毒层
            if (poison.elapsedTime >= poison.totalDuration)
            {
                poisonEffects.RemoveAt(i);
                Debug.Log("毒层数-1");
            }
        }
    }

    private void ApplyPoisonDamage(int dmg)
    {
        Debug.Log($"毒伤害触发，造成 {dmg} 伤害");
    }

    public void ApplyPoison(int dmg, float duration, float interval)
    {
        poisonEffects.Add(new PoisonEffect(dmg, duration,interval));
        Debug.Log($"新毒层添加：伤害 {dmg}, 持续 {duration} 秒");
    }
    #endregion

    #region 灼烧属性
    private void UpdateBurnEffects()
    {
        if (burnEffects.Count == 0) return;

        for (int i = burnEffects.Count - 1; i >= 0; i--)
        {
            BurnEffect burn = burnEffects[i];
            burn.timer += Time.deltaTime;
            burn.elapsedTime += Time.deltaTime;

            if (burn.timer >= burn.interval)
            {
                ApplyBurnDamage(burn.damage);
                burn.timer = 0f;  // 重新计时
            }
            if (burn.elapsedTime >= burn.totalDuration)
            {
                burnEffects.RemoveAt(i);
                Debug.Log("灼烧层数-1");
            }
        }
    }

    private void ApplyBurnDamage(int dmg)
    {
        Debug.Log($"灼烧伤害触发，造成 {dmg} 伤害");
    }

    public void ApplyBurn(int dmg, float duration, float interval)
    {
        burnEffects.Add(new BurnEffect(dmg, duration, interval));
        Debug.Log($"新毒层添加：伤害 {dmg}, 持续 {duration} 秒");
    }
    #endregion

    #region 流血属性
    private void UpdateBleedEffects()
    {
        if (bleedEffects.Count == 0) return;

        for (int i = bleedEffects.Count - 1; i >= 0; i--)
        {
            BleedEffect bleed = bleedEffects[i];
            bleed.timer += Time.deltaTime;
            bleed.elapsedTime += Time.deltaTime;


            if (bleed.timer >= bleed.interval)
            {
                ApplyBleedDamage(bleed.damage);
                bleed.timer = 0f;  // 重新计时
            }
            if (bleed.elapsedTime >= bleed.totalDuration)
            {
                bleedEffects.RemoveAt(i);
                Debug.Log("流血层数-1");
            }
        }
    }

    private void ApplyBleedDamage(int dmg)
    {
        Debug.Log($"流血伤害触发，造成 {dmg} 伤害");
    }

    public void ApplyBleed(int dmg, float duration, float interval)
    {
        bleedEffects.Add(new BleedEffect(dmg, duration, interval));
        Debug.Log($"新流血添加：伤害 {dmg}, 持续 {duration} 秒");
    }
    #endregion


}
