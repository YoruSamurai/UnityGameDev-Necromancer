using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Rendering.DebugUI;

public class Ladder
{
    public float ladderBottomY;
    public float ladderTopY;
    public float ladderTopX;
    public float ladderBottomX;

    public Ladder GetLadderInWorld(Vector2 hitPoint, Collider2D platformCollider)
    {
        Ladder ladder = new Ladder();
        float fractional = hitPoint.x % 1f;
        if(fractional > .6f)
        {
            hitPoint.x += .5f;
        }
        else
        {
            hitPoint.x -= .5f;
        }
        Tilemap tilemap = platformCollider.GetComponentInParent<Tilemap>();
        if (tilemap != null)
        {
            Debug.Log($"碰撞发生位置（世界坐标）: {hitPoint}");

            Vector3Int originCellPos = tilemap.WorldToCell(hitPoint);
            Debug.Log("cellpos: " + originCellPos);

            List<Vector3Int> ladderCells = new List<Vector3Int>();

            // 向上查找
            Vector3Int currentPos = originCellPos;
            while (tilemap.GetTile(currentPos) != null)
            {
                ladderCells.Add(currentPos);
                currentPos += Vector3Int.up;
            }

            // 向下查找
            currentPos = originCellPos + Vector3Int.down;
            while (tilemap.GetTile(currentPos) != null)
            {
                ladderCells.Add(currentPos);
                currentPos += Vector3Int.down;
            }
            Debug.Log(ladderCells.Count);
            if (ladderCells.Count > 0)
            {
                int minY = ladderCells.Min(pos => pos.y) ;
                int maxY = ladderCells.Max(pos => pos.y);
                int minX = ladderCells.Min(pos => pos.x);
                int maxX = ladderCells.Max(pos => pos.x);

                Vector3 bottomLeft = tilemap.GetCellCenterWorld(new Vector3Int(minX, minY, 0));
                Vector3 topRight = tilemap.GetCellCenterWorld(new Vector3Int(maxX, maxY, 0));

                ladder.ladderBottomX = bottomLeft.x;
                ladder.ladderBottomY = bottomLeft.y;
                ladder.ladderTopX = topRight.x;
                ladder.ladderTopY = topRight.y;

                Debug.Log($"Ladder范围 Bottom: {ladder.ladderBottomY}, Top: {ladder.ladderTopY} {ladder.ladderBottomX} {ladder.ladderTopX}");
            }
        }
        return ladder;
    }

    public bool IsLadderExist()
    {
        if(ladderBottomX == 0 && ladderTopX == 0 && ladderBottomY == 0 && ladderTopY == 0)
        {
            return false;
        }
        return true;
    }

    public void ClearLadder()
    {
        ladderTopY = 0;
        ladderTopX = 0;
        ladderBottomX = 0;
        ladderBottomY = 0;
    }
}
