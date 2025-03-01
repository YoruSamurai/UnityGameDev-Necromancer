using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStats : MonoBehaviour
{

    [SerializeField] private List<PoisonEffect> poisonEffects = new List<PoisonEffect>();
    [SerializeField] private List<BurnEffect> burnEffects = new List<BurnEffect>();
    [SerializeField] private List<BleedEffect> bleedEffects = new List<BleedEffect>();

    private void Update()
    {
        UpdatePoisonEffects();
    }

    

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

    }

    public void TakeDirectDamage(int dmg)
    {
        yoruUtils.JumpNumber(dmg, this.gameObject);
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
