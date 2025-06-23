using System.Collections.Generic;
using UnityEngine;

public class ZoneEvaluator
{
    private int zoneColliderLayer;
    private float checkBoxSize = 0.8f;

    public ZoneEvaluator(float checkBoxSize = 0.8f)
    {
        this.zoneColliderLayer = LayerMask.GetMask("Ground", "OneWayPlatform"); ;
        this.checkBoxSize = checkBoxSize;
    }

    public List<Vector2Int> GetGenerateZonePosList(Vector2 currentPos, int zoneLength, int maxHeightTolerance, bool isRightDirection, bool middleExpand)
    {
        List<Vector2Int> results = new List<Vector2Int>();
        lastAddedPos = null;
        Vector2Int basePos = Vector2Int.FloorToInt(currentPos);
        basePos += new Vector2Int(0, -1); // 向下偏移一格
        Debug.Log(basePos);

        if (middleExpand)
        {
            int halfLength = zoneLength / 2;
            bool hasMiddle = zoneLength % 2 == 1;
            if(!hasMiddle)
            {
                halfLength -= 1;
                hasMiddle = true;
            }
            Vector2Int startPos = new Vector2Int();
            // 中间点失败就直接返回空列表
            if (hasMiddle)
            {
                if (!TryAddGeneratePos(basePos, results, maxHeightTolerance))
                    return results;
                else
                {
                    startPos = results[0];
                }
            }

            // 右边
            for (int i = 1; i <= halfLength; i++)
            {
                Vector2Int rightPos = basePos + new Vector2Int(i, 0);
                if (!TryAddGeneratePos(rightPos, results, maxHeightTolerance))
                    break; // 一旦失败就停止右扩展
            }

            lastAddedPos = startPos;

            // 左边
            for (int i = 1; i <= halfLength; i++)
            {
                Vector2Int leftPos = basePos + new Vector2Int(-i, 0);
                if (!TryAddGeneratePos(leftPos, results, maxHeightTolerance))
                    break; // 一旦失败就停止左扩展
            }
            results = ReorderZigZag(startPos, results);
        }
        else
        {
            // 单方向遍历（左或右）
            int dir = isRightDirection ? 1 : -1;
            for (int i = 0; i < zoneLength; i++)
            {
                Vector2Int checkPos = basePos + new Vector2Int(i * dir, 0);
                if (!TryAddGeneratePos(checkPos, results, maxHeightTolerance))
                {
                    break;
                }

            }
        }

        Debug.Log("可生成区域：" + string.Join(", ", results));
        return results;
    }

    private List<Vector2Int> ReorderZigZag(Vector2Int center, List<Vector2Int> input)
    {
        List<Vector2Int> right = new List<Vector2Int>();
        List<Vector2Int> left = new List<Vector2Int>();
        List<Vector2Int> ordered = new List<Vector2Int>();

        ordered.Add(center);

        foreach (var pos in input)
        {
            if (pos == center) continue;
            if (pos.x > center.x)
                right.Add(pos);
            else
                left.Add(pos);
        }

        // 右边从近到远排序
        right.Sort((a, b) => a.x.CompareTo(b.x));
        // 左边从近到远排序（x大的排前面）
        left.Sort((a, b) => b.x.CompareTo(a.x));

        for (int i = 0; i < Mathf.Max(left.Count, right.Count); i++)
        {
            if (i < right.Count) ordered.Add(right[i]);
            if (i < left.Count) ordered.Add(left[i]);
        }

        return ordered;
    }


    private Vector2Int? lastAddedPos = null;

    // 封装检测逻辑
    private bool TryAddGeneratePos(Vector2Int checkPos, List<Vector2Int> results, int maxHeightTolerance)
    {
        Vector2Int? validPos = null;

        for (int offsetY = 0; offsetY <= maxHeightTolerance; offsetY++)
        {
            // 检测当前层（offsetY = 0）、向上、向下
            foreach (int dy in new int[] { offsetY, -offsetY })
            {
                Vector2Int testPos = checkPos + new Vector2Int(0, dy);
                Vector2 worldCheckPos = (Vector2)testPos + new Vector2(0.5f, 0.5f);
                Vector2 worldBelowPos = (Vector2)testPos + new Vector2(0.5f, -0.5f);

                bool isBlocked = CheckHasGround(worldCheckPos);
                bool isBelowSolid = CheckHasGround(worldBelowPos);

                if (!isBlocked && isBelowSolid)
                {
                    validPos = testPos;
                    break; // 找到有效位置就停止
                }
            }

            if (validPos.HasValue)
                break;
        }

        if (validPos.HasValue)
        {
            // 检查前一个位置高度，防止突变
            if (lastAddedPos.HasValue && Mathf.Abs(validPos.Value.y - lastAddedPos.Value.y) > 1)
            {
                Debug.LogWarning($"跳过断层：{validPos.Value}, 与上一个点高差过大：{lastAddedPos.Value}");
                return false;
            }

            results.Add(validPos.Value);
            lastAddedPos = validPos;
            return true;
        }
        return false;
        
    }

    // 原有碰撞检测方法 + 可视化
    private bool CheckHasGround(Vector2 worldPos)
    {
        Vector2 size = new Vector2(.8f, .8f);
        //Collider2D hit = Physics2D.OverlapBox(worldPos, size, 0f, zoneColliderLayer);
        Collider2D hit = Physics2D.OverlapBox(worldPos, size, 0f, PlayerStats.Instance.player.combinedGroundLayers);

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