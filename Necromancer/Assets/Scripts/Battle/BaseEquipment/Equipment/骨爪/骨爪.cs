using System.Collections.Generic;
using UnityEngine;

public class 骨爪 : MeleeEquipment
{
    //
    protected override void Start()
    {
        base.Start();
        Debug.Log("骨爪，启动！");
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.gameObject.layer == 6 && !isHitInAttack)
        {
            Debug.Log("是敌人！");
            isHitInAttack = true;
        }

        MonsterStats monsterStats = collision.GetComponent<MonsterStats>();
        PlayerStats.Instance.OnPlayerHit(this, monsterStats);

    }

    public override void UseEquipment()
    {
        if (!GetCanUseEquipment())
        {
            Debug.Log("攻击还在冷却中！");
            return;
        }
        base.UseEquipment();
    }



    public override void DoDamage(float _cMag, MonsterStats monsterStats)
    {
        base.DoDamage(_cMag, monsterStats);
        int dmg = (int)(currentDmg * _cMag);
        Debug.Log(dmg);
        monsterStats.TakeDirectDamage(dmg);

    }

    public override void TriggerHitCheckStart()
    {
        base.TriggerHitCheckStart();
        List<Vector2Int> zonePosList = GetGenerateZonePosList(transform.position, 5, 1,player.GetFacingRight(),false);
        ProjectileManager.Instance.GenerateZone(zonePosList, 5f, 0.2f, 0.5f, false, this);

    }

    public List<Vector2Int> GetGenerateZonePosList(Vector2 currentPos, int zoneLength, int maxHeightTolerance, bool isRightDirection, bool middleExpand)
    {
        List<Vector2Int> results = new List<Vector2Int>();

        Vector2Int basePos = Vector2Int.FloorToInt(currentPos);
        basePos += new Vector2Int(0, -1); // 向下偏移一格
        Debug.Log(basePos);

        if (middleExpand)
        {
            // 从中间向两边扩散
            for (int offset = 0; offset < zoneLength; offset++)
            {
                // 交替向左和向右扩展
                int dx = (offset % 2 == 0) ? (offset / 2) : -(offset / 2 + 1);
                Vector2Int checkPos = basePos + new Vector2Int(dx, 0);

                TryAddGeneratePos(checkPos, results);
            }
        }
        else
        {
            // 单方向遍历（左或右）
            int dir = isRightDirection ? 1 : -1;
            for (int i = 0; i < zoneLength; i++)
            {
                Vector2Int checkPos = basePos + new Vector2Int(i * dir, 0);
                TryAddGeneratePos(checkPos, results);
            }
        }

        Debug.Log("可生成区域：" + string.Join(", ", results));
        return results;
    }

    // 封装检测逻辑
    private void TryAddGeneratePos(Vector2Int checkPos, List<Vector2Int> results)
    {
        Vector2 worldCheckPos = (Vector2)checkPos + new Vector2(0.5f, 0.5f);
        Vector2 worldBelowPos = (Vector2)checkPos + new Vector2(0.5f, -0.5f);

        bool isBlocked = CheckHasGround(worldCheckPos);
        bool isBelowSolid = CheckHasGround(worldBelowPos);
        Debug.Log($"检测 {checkPos}: isBlocked={isBlocked}, isBelowSolid={isBelowSolid}");

        if (!isBlocked && isBelowSolid)
        {
            results.Add(checkPos); // 添加“上方平台点”
        }
    }

    // 原有碰撞检测方法 + 可视化
    private bool CheckHasGround(Vector2 worldPos)
    {
        Vector2 size = new Vector2(.8f, .8f);
        Collider2D hit = Physics2D.OverlapBox(worldPos, size, 0f, player.combinedGroundLayers);

        // 可视化：画出检测方框
        Vector2 halfSize = size * 0.5f;
        Vector2 topLeft = worldPos + new Vector2(-halfSize.x, halfSize.y);
        Vector2 topRight = worldPos + new Vector2(halfSize.x, halfSize.y);
        Vector2 bottomLeft = worldPos + new Vector2(-halfSize.x, -halfSize.y);
        Vector2 bottomRight = worldPos + new Vector2(halfSize.x, -halfSize.y);
        Debug.DrawLine(topLeft, topRight, Color.red, 1f);
        Debug.DrawLine(topRight, bottomRight, Color.red, 1f);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red, 1f);
        Debug.DrawLine(bottomLeft, topLeft, Color.red, 1f);

        return hit != null;
    }


}