using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Drawing;

public class ShadowCasterGenerator : MonoBehaviour
{
    public GameObject shadowCasterPrefab; // 包含 ShadowCaster2D 的预制体

    [SerializeField] private List<List<GameObject>> shadowCasterRoomList;
    [SerializeField] private List<int> currentRoomList;



    private void Awake()
    {
        shadowCasterRoomList = new List<List<GameObject>>();
        currentRoomList = new List<int>(); 
    }

    private float updateInterval = 1f; // 每秒更新一次
    private float updateTimer = 0f;

    private void FixedUpdate()
    {
        updateTimer += Time.fixedDeltaTime;

        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;

            List<int> roomList = LevelManager.Instance.GetAdjacentRooms();

            // 启用新进入的房间
            foreach (int room in roomList)
            {
                if (!currentRoomList.Contains(room))
                {
                    EnableShadowInRoom(room-1);
                    LevelManager.Instance.EnableLightInRoom(room-1);
                }
            }

            // 禁用不再相邻的房间
            foreach (int room in currentRoomList)
            {
                if (!roomList.Contains(room))
                {
                    DisableShadowInRoom(room-1);
                    LevelManager.Instance.DisableLightInRoom(room - 1);
                }
            }

            // 更新当前房间列表
            currentRoomList = new List<int>(roomList);
        }
    }


    private void EnableShadowInRoom(int roomIndex)
    {
        foreach (var caster in shadowCasterRoomList[roomIndex])
        {
            caster.SetActive(true);
        }
    }

    private void DisableShadowInRoom(int roomIndex)
    {
        foreach (var caster in shadowCasterRoomList[roomIndex])
        {
            caster.SetActive(false);
        }
    }


    public void GenerateShadowCasters(Tilemap tilemap, Vector2 offset)
    {
        BoundsInt bounds = tilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;

        bool[,] visited = new bool[width, height];
        List<GameObject> shadowCasterData = new List<GameObject>();


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int tileX = bounds.xMin + x;
                int tileY = bounds.yMin + y;
                Vector3Int tilePos = new Vector3Int(tileX, tileY, 0);

                if (visited[x, y] || !tilemap.HasTile(tilePos))
                    continue;

                // 寻找可以合并的最大矩形（从当前位置向右下扩展）
                int maxWidth = 0;
                while (x + maxWidth < width && tilemap.HasTile(new Vector3Int(bounds.xMin + x + maxWidth, tileY, 0)) && !visited[x + maxWidth, y])
                {
                    maxWidth++;
                }

                int maxHeight = 1;
                bool canExpand = true;
                while (y + maxHeight < height && canExpand)
                {
                    for (int i = 0; i < maxWidth; i++)
                    {
                        if (!tilemap.HasTile(new Vector3Int(bounds.xMin + x + i, bounds.yMin + y + maxHeight, 0)) || visited[x + i, y + maxHeight])
                        {
                            canExpand = false;
                            break;
                        }
                    }
                    if (canExpand)
                        maxHeight++;
                }

                // 标记区域已访问
                for (int dy = 0; dy < maxHeight; dy++)
                {
                    for (int dx = 0; dx < maxWidth; dx++)
                    {
                        visited[x + dx, y + dy] = true;
                    }
                }

                // 构造 region 并生成 ShadowCaster
                List<Vector3Int> region = new List<Vector3Int>();
                for (int dy = 0; dy < maxHeight; dy++)
                {
                    for (int dx = 0; dx < maxWidth; dx++)
                    {
                        region.Add(new Vector3Int(bounds.xMin + x + dx, bounds.yMin + y + dy, 0));
                    }
                }

                shadowCasterData.Add(CreateShadowCaster(region, offset));
            }
        }
        shadowCasterRoomList.Add(shadowCasterData);
    }

    private GameObject CreateShadowCaster(List<Vector3Int> region, Vector2 offset)
    {
        // 获取区域边界
        int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
        foreach (var pos in region)
        {
            if (pos.x < minX) minX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y > maxY) maxY = pos.y;
        }

        Vector2 center = new Vector2((minX + maxX + 1) / 2f, (minY + maxY + 1) / 2f);
        Vector2 size = new Vector2((maxX - minX + 1), (maxY - minY + 1));



        GameObject obj = Instantiate(shadowCasterPrefab, center + offset, Quaternion.identity);
        obj.transform.SetParent(this.gameObject.transform);

        var poly = obj.GetComponent<PolygonCollider2D>();
        if (poly != null)
        {
            poly.pathCount = 1;
            poly.SetPath(0, new Vector2[]
            {
                new Vector2(-size.x / 2f, -size.y / 2f),
                new Vector2(-size.x / 2f, size.y / 2f),
                new Vector2(size.x / 2f, size.y / 2f),
                new Vector2(size.x / 2f, -size.y / 2f)
            });
        }
        // 添加 ShadowCaster2D
        var shadowCaster = obj.AddComponent<ShadowCaster2D>();
        shadowCaster.useRendererSilhouette = false;
        shadowCaster.selfShadows = true;
        obj.SetActive(false);
        return obj;

    }

    


}
