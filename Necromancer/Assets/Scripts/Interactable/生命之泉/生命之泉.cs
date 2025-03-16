using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 生命之泉 : MonoBehaviour
{
    [Header("Upgrade Settings")]
    [Tooltip("str提升百分比（例如 0.1 表示 10%）")]
    [SerializeField] private float strBoostPercent = 0.1f;
    [Tooltip("str提升百分比（例如 0.1 表示 10%）")]
    [SerializeField] private float agileBoostPercent = 0.1f;
    [Tooltip("法术强度提升百分比")]
    [SerializeField] private float magicBoostPercent = 0.1f;
    [Tooltip("生命值提升百分比")]
    [SerializeField] private float healthBoostPercent = 0.2f;

    [Header("Visual Settings")]
    [Tooltip("正常状态颜色")]
    [SerializeField] private Color normalColor = Color.gray;
    [Tooltip("高亮状态颜色")]
    [SerializeField] private Color highlightColor = Color.cyan;

    private SpriteRenderer sr;
    private bool isPlayerNearby = false;
    private bool isUsed = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("LifeFountain: 没有找到 SpriteRenderer 组件！");
        }
        sr.color = normalColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isUsed) return;
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            sr.color = highlightColor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isUsed) return;
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            sr.color = normalColor;
        }
    }

    private void Update()
    {
        if (isUsed || !isPlayerNearby) return;

        // 检测数字键 1 和 2
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ApplyUpgrade(DamageType.Str);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ApplyUpgrade(DamageType.Agile);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ApplyUpgrade(DamageType.Magic);
        }
    }

    private void ApplyUpgrade(DamageType dmgType)
    {
        // 获取玩家的 PlayerStats 单例
        PlayerStats ps = PlayerStats.Instance;
        if (ps != null)
        {
            if (dmgType == DamageType.Str)
            {
                ps.UpgradeStr(1 + strBoostPercent, 1 + healthBoostPercent);
            }
            else if(dmgType == DamageType.Agile)
            {
                ps.UpgradeAgile(1 + agileBoostPercent, 1 + healthBoostPercent);
            }
            else if(dmgType == DamageType.Magic) 
            {
                ps.UpgradeMagic(1 + magicBoostPercent, 1 + healthBoostPercent);
            }
        }

        // 升级完成后将血泉设为已使用状态，颜色恢复正常，并禁用碰撞
        isUsed = true;
        sr.color = normalColor;
        GetComponent<Collider2D>().enabled = false;
    }
}



