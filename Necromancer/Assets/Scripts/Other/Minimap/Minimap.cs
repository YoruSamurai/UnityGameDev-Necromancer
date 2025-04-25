using LDtkUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;
using static RoomGraphGenerator;

public class Minimap : MonoBehaviour
{

    [SerializeField] private float scale;


    public RectTransform minimapRectTransform; // RawImage 的 RectTransform
    public RectTransform playerIconImage;      // 小红点（图标）

    [SerializeField] private ShadowCasterGenerator shadowCasterGenerator;


    public RawImage minimapImage;
    public Transform playerTransform;
    public Player player;
    public int mapWidth;
    public int mapHeight;
    public int pixelPerUnit = 1;
    public int revealRadius = 10;

    int minX = int.MaxValue, maxX = int.MinValue;
    int minY = int.MaxValue, maxY = int.MinValue;

    private RoomGraphGenerator roomGraphGenerator;

    public int mapOffset = 20;

    public Vector2Int startPos;

    public Color borderColor = Color.white;
    public Color defaultRoomColor = Color.green;
    public Color blackTileColor = new Color(1f, 1f, 1f,0f);

    private Texture2D minimapTexture;
    private HashSet<Vector2Int> revealedTiles = new HashSet<Vector2Int>();

    public Dictionary<Vector2Int, Color> tileColorMap = new Dictionary<Vector2Int, Color>();

    void Start()
    {
        player = playerTransform.GetComponent<Player>();
        InitialMinimap();
        InitialMapTile();
    }

    private void InitialMapTile()
    {
        List<ActualRoomData> roomDatas = roomGraphGenerator.roomDatas;
        foreach(var roomData in roomDatas)
        {
            foreach (LDtkComponentLayer layer in roomData.room.levelData.LayerInstances)
            {
                if(layer == null) continue;
                if (layer.name == "Ladder")
                {
                    Tilemap map = layer.GetComponentInChildren<Tilemap>();
                    if (map == null) continue;

                    // 获取tilemap的边界
                    BoundsInt bounds = map.cellBounds;

                    for (int x = bounds.xMin; x < bounds.xMax; x++)
                    {
                        for (int y = bounds.yMin; y < bounds.yMax; y++)
                        {
                            Vector3Int tilePos = new Vector3Int(x, y, 0);
                            if (map.HasTile(tilePos))
                            {
                                // 这是该Tilemap下的本地tile位置
                                Debug.Log($"Tile at local position:{roomData.room.levelData.name} {tilePos}");
                                //设置和取值不需要动 要动的只有最后的SetPixel。
                                RegisterTile(new Vector2Int(tilePos.x + (int)roomData.startPosition.x, tilePos.y + (int)roomData.startPosition.y), Color.white);
                            }
                        }
                    }
                }
                if (layer.name == "OneWayPlatform")
                {
                    Tilemap map = layer.GetComponentInChildren<Tilemap>();
                    if (map == null) continue;

                    // 获取tilemap的边界
                    BoundsInt bounds = map.cellBounds;

                    for (int x = bounds.xMin; x < bounds.xMax; x++)
                    {
                        for (int y = bounds.yMin; y < bounds.yMax; y++)
                        {
                            Vector3Int tilePos = new Vector3Int(x, y, 0);
                            if (map.HasTile(tilePos))
                            {
                                // 这是该Tilemap下的本地tile位置
                                Debug.Log($"Tile at local position:{roomData.room.levelData.name} {tilePos}");
                                //设置和取值不需要动 要动的只有最后的SetPixel。
                                RegisterTile(new Vector2Int(tilePos.x + (int)roomData.startPosition.x, tilePos.y + (int)roomData.startPosition.y), Color.blue);
                            }
                        }
                    }
                }
                if (layer.name == "Ground")
                {
                    Tilemap map = layer.GetComponentInChildren<Tilemap>();
                    if (map == null) continue;
                    shadowCasterGenerator.GenerateShadowCasters(map, roomData.startPosition);
                    BoundsInt bounds = map.cellBounds;

                    for (int x = bounds.xMin; x < bounds.xMax; x++)
                    {
                        for (int y = bounds.yMin; y < bounds.yMax; y++)
                        {
                            Vector3Int tilePos = new Vector3Int(x, y, 0);
                            if (!map.HasTile(tilePos)) continue;

                            bool isEdge = false;
                            Vector3Int[] directions = new Vector3Int[]
                            {
                                new Vector3Int(1, 0, 0),
                                new Vector3Int(-1, 0, 0),
                                new Vector3Int(0, 1, 0),
                                new Vector3Int(0, -1, 0),
                                new Vector3Int(1, 1, 0),
                                new Vector3Int(-1, 1, 0),
                                new Vector3Int(1, -1, 0),
                                new Vector3Int(-1, -1, 0),
                            };

                            foreach (var dir in directions)
                            {
                                if (!map.HasTile(tilePos + dir))
                                {
                                    isEdge = true;
                                    break;
                                }
                            }

                            if (isEdge)
                            {
                                Vector2Int worldPos = new Vector2Int(tilePos.x + (int)roomData.startPosition.x, tilePos.y + (int)roomData.startPosition.y);
                                RegisterTile(worldPos, Color.gray); // 只注册边缘
                            }
                            else
                            {
                                Vector2Int worldPos = new Vector2Int(tilePos.x + (int)roomData.startPosition.x, tilePos.y + (int)roomData.startPosition.y);
                                RegisterTile(worldPos, blackTileColor); // 只注册边缘
                            }
                        }
                    }
                    
                }
            }
        }
    }


    private void InitialMinimap()
    {
        roomGraphGenerator = FindObjectOfType<RoomGraphGenerator>();
        List<ActualRoomData> roomDatas = roomGraphGenerator.roomDatas;
        tileColorMap.Clear();
        revealedTiles.Clear();


        foreach (var room in roomDatas)
        {
            LDtkComponentLevel level = room.room.levelData;

            Vector2Int roomSize = new Vector2Int((int)room.levelWidth, (int)room.levelHeight);
            Vector3Int roomOrigin = Vector3Int.FloorToInt(room.startPosition);

            // 扫描每个房间内的格子
            for (int x = 0; x < roomSize.x; x++)
            {
                for (int y = 0; y < roomSize.y; y++)
                {
                    Vector3Int tilePos = new Vector3Int(roomOrigin.x + x, roomOrigin.y + y, 0);

                    minX = Mathf.Min(minX, tilePos.x);
                    maxX = Mathf.Max(maxX, tilePos.x);
                    minY = Mathf.Min(minY, tilePos.y);
                    maxY = Mathf.Max(maxY, tilePos.y);
                }
            }
        }
        mapWidth = maxX - minX;
        mapHeight = maxY - minY;
        startPos = new Vector2Int(minX, minY);

        foreach (var room in roomDatas)
        {
            LDtkComponentLevel level = room.room.levelData;

            Vector2Int roomSize = new Vector2Int((int)room.levelWidth, (int)room.levelHeight);
            Vector3Int roomOrigin = Vector3Int.FloorToInt(room.startPosition);

            // 扫描每个房间内的格子
            for (int x = 0; x < roomSize.x; x++)
            {
                for (int y = 0; y < roomSize.y; y++)
                {
                    Vector3Int tilePos = new Vector3Int(roomOrigin.x + x, roomOrigin.y + y, 0);
                    Color color = GetRoomColor(room); // 根据类型返回不同颜色

                    //设置和取值不需要动 要动的只有最后的SetPixel。
                    RegisterTile(new Vector2Int(tilePos.x, tilePos.y), color);
                }
            }
        }

        minimapTexture = new Texture2D(mapWidth + mapOffset, mapHeight + mapOffset);
        minimapTexture.filterMode = FilterMode.Point;
        minimapImage.texture = minimapTexture;
        minimapImage.color = Color.white;

        RectTransform rectTransform = minimapImage.gameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(mapWidth + mapOffset, mapHeight + mapOffset);
        rectTransform.localScale = new Vector2(5, 5);
        //rectTransform.localScale = new Vector2(1, 1);

        blackTileColor = new Color(1f, 1f, 1f, 0f);
        // 初始化为纯黑
        for (int x = 0; x < mapWidth + mapOffset; x++)
        {
            for (int y = 0; y < mapHeight + mapOffset; y++)
            {
                minimapTexture.SetPixel(x, y, blackTileColor);
            }
        }

        minimapTexture.Apply();
    }

    private Color GetRoomColor(ActualRoomData room)
    {
        switch (room.gameRoomType)
        {
            case GameRoomType.Exit: return new Color(247 / 255f, 170 / 255f, 181 / 255f);       // 红色透明
            case GameRoomType.Entrance: return new Color(255 / 255f, 238 / 255f, 149 / 255f);   // 黄色透明
            case GameRoomType.Normal: return new Color(183 / 255f, 247 / 255f, 170 / 255f);     // 绿色透明
            default: return new Color(0.5f, 0.5f, 0.5f, 0.3f);                // 灰色透明
        }
    }

    void Update()
    {
        Vector2Int playerTilePos = new Vector2Int(
            Mathf.RoundToInt(playerTransform.position.x * pixelPerUnit),
            Mathf.RoundToInt(playerTransform.position.y * pixelPerUnit)
        );

        for (int dx = -revealRadius; dx <= revealRadius; dx++)
        {
            for (int dy = -revealRadius; dy <= revealRadius; dy++)
            {
                Vector2Int tilePos = new Vector2Int(playerTilePos.x + dx, playerTilePos.y + dy);

                    if (!revealedTiles.Contains(tilePos))
                    {
                        revealedTiles.Add(tilePos);
                        Color color;
                        if (tileColorMap.TryGetValue(tilePos, out color))
                        {
                            minimapTexture.SetPixel(tilePos.x - minX + mapOffset/2, tilePos.y - minY + mapOffset / 2, color);
                        }
                        /*else
                            minimapTexture.SetPixel(tilePos.x - minX, tilePos.y - minY, blackTileColor);*/

                    }
                
            }
        }

        if(player.facingDir == 1 && playerIconImage.localRotation.y != -180)
        {
            playerIconImage.localRotation = Quaternion.Euler(0, -180, 0);
        }
        else if(player.facingDir == -1 && playerIconImage.localRotation.y != 0)
        {
            playerIconImage.localRotation = Quaternion.Euler(0, 0, 0);
        }

        minimapTexture.Apply();
    }

    private void FixedUpdate()
    {
        Vector2Int playerTilePos = new Vector2Int(
            Mathf.RoundToInt(playerTransform.position.x * pixelPerUnit),
            Mathf.RoundToInt(playerTransform.position.y * pixelPerUnit)
        );

        int w = minimapTexture.width * 5 / 2;
        int h = minimapTexture.height * 5 / 2;

        // ✅ 第一步：计算玩家在贴图上的坐标（像素级）
        float pixelX = (playerTilePos.x - minX + mapOffset / 2);
        float pixelY = (playerTilePos.y - minY + mapOffset / 2);

        // ✅ 第二步：计算 UI 上的比例位置（0~1）
        float normalizedX = pixelX / minimapTexture.width;
        float normalizedY = pixelY / minimapTexture.height;

        // ✅ 第三步：映射到 UI 空间（RectTransform 的尺寸）
        float uiX = normalizedX * minimapRectTransform.rect.width;
        float uiY = normalizedY * minimapRectTransform.rect.height;

        // ✅ 设置小图标的位置
        playerIconImage.anchoredPosition = new Vector2(uiX, uiY);


        // ✅ 设置 RectTransform 的位置为 playerTilePos
        minimapRectTransform.anchoredPosition = new Vector2(scale*playerTilePos.x + w -50, scale * playerTilePos.y * .8f -40);
    }

    // 外部用来初始化 tile 数据（如房间颜色）
    public void RegisterTile(Vector2Int position, Color color)
    {
        tileColorMap[position] = color;
    }
}
