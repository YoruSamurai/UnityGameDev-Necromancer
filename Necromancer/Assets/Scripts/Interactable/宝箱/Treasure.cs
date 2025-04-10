using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    [Header("升级参数")]


    [Header("视觉设置")]
    [SerializeField] private Color normalColor = Color.gray;
    [SerializeField] private Color highlightColor = Color.yellow;

    private SpriteRenderer sr;
    private bool isPlayerNearby = false;
    private bool isUsed = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("WeaponUpgradeFountain: 未找到 SpriteRenderer 组件！");
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

        // 按下数字键 1 爆武器
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GenerateRandomWeapon();
        }
        
    }

    private void GenerateRandomWeapon()
    {
        BattleManagerTest.Instance.DropPickableEquipment(this.gameObject.transform.position);
        //MarkAsUsed();

    }

    

    private void MarkAsUsed()
    {
        isUsed = true;
        sr.color = normalColor;
        // 禁用碰撞器，防止重复使用
        GetComponent<Collider2D>().enabled = false;
    }
}
