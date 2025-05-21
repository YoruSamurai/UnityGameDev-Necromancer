using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Drawing;

public class ShadowCasterGenerator : MonoBehaviour
{
    public GameObject shadowCasterPrefab; // 包含 ShadowCaster2D 的预制体

    private List<ShadowCasterData> shadowCasterList;

    public Transform player;
    public float activeRange = 30f;

    private class ShadowCasterData
    {
        public Vector2 center;
        public Vector2 size;
        public GameObject instance; // 生成出的 GameObject，如果没有生成就是 null
    }
    private void Awake()
    {
        shadowCasterList = new List<ShadowCasterData>();
    }

    private void FixedUpdate()
    {
        Vector2 playerPos = player.position;

        foreach (var data in shadowCasterList)
        {
            float distance = Vector2.Distance(playerPos, data.center);
            bool inRange = distance <= activeRange;

            if (inRange && data.instance == null)
            {
                // 不在池中 → 加入池
                data.instance = GetFromPool(data.center, data.size);
            }
            else if (!inRange && data.instance != null)
            {
                // 在池中 → 移出池
                RemoveToPool(data.instance);
                data.instance = null;
            }
        }


    }

    private GameObject GetFromPool(Vector2 center,Vector2 size)
    {
        GameObject obj = ObjectPoolManager.SpawnObject(shadowCasterPrefab, center, Quaternion.identity, ObjectPoolManager.PoolType.ShadowCasters);
        obj.transform.position = center;

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

        var shadowCaster = obj.AddComponent<ShadowCaster2D>();
        shadowCaster.useRendererSilhouette = false;
        shadowCaster.selfShadows = true;
        

        return obj;

    }

    private void RemoveToPool(GameObject shadowCaster)
    {
        ObjectPoolManager.ReturnObjectToPool(shadowCaster, ObjectPoolManager.PoolType.ShadowCasters);
    }


    public void GenerateShadowCasters(Tilemap tilemap, Vector2 offset)
    {
        BoundsInt bounds = tilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;

        bool[,] visited = new bool[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int tileX = bounds.xMin + x;
                int tileY = bounds.yMin + y;
                Vector3Int tilePos = new Vector3Int(tileX, tileY, 0);

                if (visited[x, y] || !tilemap.HasTile(tilePos))
                    continue;

                // 从当前 tile 开始向右扩展横向的连续 tile
                List<Vector3Int> rowRegion = new List<Vector3Int>();
                int currentX = x;

                while (currentX < width)
                {
                    Vector3Int checkPos = new Vector3Int(bounds.xMin + currentX, tileY, 0);
                    if (tilemap.HasTile(checkPos) && !visited[currentX, y])
                    {
                        rowRegion.Add(checkPos);
                        visited[currentX, y] = true;
                        currentX++;
                    }
                    else
                    {
                        break;
                    }
                }

                // 创建 Shadow Caster（横向一条）
                if (rowRegion.Count > 0)
                    CreateShadowCaster(rowRegion, offset);
            }
        }
    }

    private void CreateShadowCaster(List<Vector3Int> region, Vector2 offset)
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

        shadowCasterList.Add(new ShadowCasterData
        {
            center = center + offset,
            size = size,
            instance = null
        });

        /*GameObject obj = Instantiate(shadowCasterPrefab, center + offset, Quaternion.identity);
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
        shadowCaster.selfShadows = true;*/
    }

    


}
