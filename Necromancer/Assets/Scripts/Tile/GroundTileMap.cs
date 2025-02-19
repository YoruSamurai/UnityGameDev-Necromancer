using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundTileMap : MonoBehaviour
{
    public Tilemap ground_tilemap;
    public Tilemap environ_tilemap;
    public TileBase groundTile;
    public TileBase environTile;

    [Header("地图参数")]
    public Vector2Int mapSize = new Vector2Int(100, 20); // 地图尺寸 (x: 长度, y: 高度)
    public Vector2Int roomSizeRange = new Vector2Int(5, 15); // 房间尺寸范围
    public int minRooms = 3; // 最少生成房间数量
    public float corridorWidth = 3f; // 走廊宽度
    public float environmentDensity = 0.3f; // 环境装饰密度

    // 清除并重新生成地图
    public void GenerateMap()
    {
        ClearMap();
        GenerateRoomsAndCorridors();
        GenerateEnvironment();
    }

    public void ClearMap()
    {
        ground_tilemap.ClearAllTiles();
        environ_tilemap.ClearAllTiles();
    }

    // 生成房间和走廊
    private void GenerateRoomsAndCorridors()
    {
        // 生成初始房间
        Vector2Int previousRoomCenter = Vector2Int.zero;
        for (int i = 0; i < minRooms; i++)
        {
            Vector2Int roomSize = new Vector2Int(
                Random.Range(roomSizeRange.x, roomSizeRange.y),
                Random.Range(roomSizeRange.x, roomSizeRange.y)
            );

            Vector2Int roomPosition = previousRoomCenter + new Vector2Int(
                Random.Range(5, 10),
                Random.Range(-2, 2)
            );

            // 生成房间
            GenerateRoom(roomPosition, roomSize);

            // 生成走廊（连接上一个房间）
            if (i > 0)
            {
                //GenerateCorridor(previousRoomCenter, roomPosition);
            }

            previousRoomCenter = roomPosition;
        }
    }

    // 生成一个矩形房间
    private void GenerateRoom(Vector2Int center, Vector2Int size)
    {
        BoundsInt bounds = new BoundsInt(
            center.x - size.x / 2,
            center.y - size.y / 2,
            0,
            size.x,
            size.y,
            1
        );

        ground_tilemap.SetTilesBlock(bounds, GetTilesArray(groundTile, bounds.size.x * bounds.size.y));
    }

    // 生成走廊（横向 + 纵向）
    private void GenerateCorridor(Vector2Int from, Vector2Int to)
    {
        // 横向走廊
        int startX = Mathf.Min(from.x, to.x);
        int endX = Mathf.Max(from.x, to.x);
        int y = from.y;

        BoundsInt horizontalCorridor = new BoundsInt(
            startX,
            y - (int)(corridorWidth / 2),
            0,
            endX - startX,
            (int)corridorWidth,
            1
        );

        ground_tilemap.SetTilesBlock(horizontalCorridor, GetTilesArray(groundTile, horizontalCorridor.size.x * horizontalCorridor.size.y));

        // 纵向走廊
        int startY = Mathf.Min(from.y, to.y);
        int endY = Mathf.Max(from.y, to.y);
        int x = to.x;

        BoundsInt verticalCorridor = new BoundsInt(
            x - (int)(corridorWidth / 2),
            startY,
            0,
            (int)corridorWidth,
            endY - startY,
            1
        );

        ground_tilemap.SetTilesBlock(verticalCorridor, GetTilesArray(groundTile, verticalCorridor.size.x * verticalCorridor.size.y));
    }

    // 生成环境装饰
    private void GenerateEnvironment()
    {
        foreach (var pos in ground_tilemap.cellBounds.allPositionsWithin)
        {
            if (ground_tilemap.GetTile(pos) != null &&
                environ_tilemap.GetTile(pos) == null &&
                Random.value < environmentDensity)
            {
                Vector3Int upPos = new Vector3Int(pos.x, pos.y + 1, pos.z);
                if (ground_tilemap.GetTile(upPos) == null)
                {
                    environ_tilemap.SetTile(upPos, environTile);
                }
            }
        }
    }

    // 辅助方法：生成 TileBase 数组
    private TileBase[] GetTilesArray(TileBase tile, int length)
    {
        TileBase[] tiles = new TileBase[length];
        for (int i = 0; i < length; i++) tiles[i] = tile;
        return tiles;
    }
}