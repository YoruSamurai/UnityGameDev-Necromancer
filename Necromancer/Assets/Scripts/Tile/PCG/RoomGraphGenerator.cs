using LDtkUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

//一个结果 返回各种生成成功/错误信息
[Serializable]
public struct RoomGenerationResult
{
    public bool success;
    public string message;
    public int errorCode; // 你可以使用 0 表示成功，1 代表无可用房间等

    public RoomGenerationResult(bool success, string message, int errorCode)
    {
        this.success = success;
        this.message = message;
        this.errorCode = errorCode;
    }
}

public enum TilePostProcessType
{
    Church
}

public class RoomGraphGenerator : MonoBehaviour
{
    //父节点 放地图
    [SerializeField] private Transform mapParent;

    //关卡的所有图块
    [SerializeField] private LdtkLevelSoList levelList;

    //关卡后处理类型
    [SerializeField] private TilePostProcessType postProcessType;

    //地图的节点图
    [SerializeField] private RoomGraph roomGraph;

    //实际的房间数据
    [SerializeField] public List<ActualRoomData> roomDatas;

    [SerializeField] private float stepTime;


    //已经生成的节点
    [SerializeField] private Dictionary<int, bool> isVisited = new Dictionary<int, bool>();
    //节点上有的门方向不可生成
    [SerializeField] private Dictionary<int, List<DoorDir>> isDoorClosed = new Dictionary<int, List<DoorDir>>();

    //在方向门被关闭后，需要尝试子节点其他门 此时保存父门以保证子节点开口相同
    [SerializeField] private DoorInfo lastDoor = new DoorInfo();

    // 新增种子变量
    [SerializeField] private int seed = 12345;

    //房间数据 存储了：房间 房间起始点 房间长度和宽度
    [Serializable]
    public class ActualRoomData
    {
        public int roomID;
        public LdtkLevelSO room;
        public Vector2 startPosition;
        public GameRoomType gameRoomType;
        public float levelWidth;
        public float levelHeight;
        public List<DoorInfo> doorInfos;
        public List<int> connectionRoom;
        public List<LightPrefab> lightPrefabs;
        public LDtkComponentLevel levelData;

    }



    //每次生成前先清空
    private void ClearExistingLevels()
    {
        for (int i = mapParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(mapParent.GetChild(i).gameObject);

        }
        roomDatas.Clear();
        isVisited.Clear();
        lastDoor.node = -1;//我们基于node去确定lastDoor是否有东西
        isDoorClosed.Clear();
        blackTilemap.ClearAllTiles();
        foreach (var roomNode in roomGraph.allRooms)
        {
            // 确保字典中存在该键
            if (!isDoorClosed.ContainsKey(roomNode.roomID))
            {
                isDoorClosed[roomNode.roomID] = new List<DoorDir>(); // 初始化空列表
                //Debug.Log(isDoorClosed[roomNode.roomID].Count + " " + roomNode.roomID);
            }
            isVisited[roomNode.roomID] = false; // 假设 roomID 从 1 开始
            isDoorClosed[roomNode.roomID].Clear();
        }
    }

    public IEnumerator GenerateLevelTest()
    {
        // 记录开始时间
        DateTime startTime = DateTime.Now;

        // 固定随机种子
        //UnityEngine.Random.InitState(seed);

        bool successSet = false;
        int tryNum = 0;
        while (!successSet)
        {
            // 清理旧关卡
            ClearExistingLevels();

            int currentNode = 1; //当前要处理的房间
            int currentParentNode = 0; //当前要处理的房间的父亲房间 
            while (successSet == false)
            {
                
                // 添加延迟
                //yield return new WaitForSeconds(.05f);
                if(stepTime > 0)
                {
                    yield return new WaitForSecondsRealtime(stepTime);
                }
                tryNum++;
                if (currentNode == 0 && currentParentNode == 0)
                {
                    Debug.Log("生成完成 欧耶！" + "经过了" + tryNum + "次");
                    successSet = true;
                    break;
                }
                RoomGenerationResult result;
                if (currentNode == 1)
                {
                    result = TryGenerateRoom(currentNode, currentParentNode,
                    roomGraph.allRooms[currentNode - 1], null);
                }
                else
                {
                    result = TryGenerateRoom(currentNode, currentParentNode,
                        roomGraph.allRooms[currentNode - 1], roomDatas[currentParentNode - 1]);
                }
                Debug.Log(result.message);
                //如果成功 设置已经有的节点列表 基于一些算法选择下一个要生成的房间编号
                if (result.success == true)
                {
                    isVisited[currentNode] = true; // 标记当前房间已生成
                    int node = currentNode;
                    //基于一些算法选择下一个要生成的房间编号
                    (currentNode, currentParentNode) = SelectNextNode(node);
                }
                else if (result.success == false)
                {
                    if (result.errorCode == 500)
                    {
                        //我们需要删除它和他的父节点和父节点的其他子节点 那就是找到父节点 把它和它后面的全删掉
                        DeleteParentNodeAndChild(currentParentNode);
                        currentNode = currentParentNode;
                        currentParentNode = GetParentNodeByNode(currentNode);
                    }
                    if (result.errorCode == 600)
                    {
                        //该节点所有方向都不能继续生成了 重启试试看吧
                        Debug.Log("再见");
                        break;
                    }
                }
            }
        }
        // 清理旧房间
        for (int i = mapParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(mapParent.GetChild(i).gameObject);
        }

        // 实例化新房间
        foreach (var room in roomDatas)
        {
            LDtkComponentLevel ldtkInstance = Instantiate(room.room.levelData, room.startPosition, Quaternion.identity, mapParent);
            room.levelData = ldtkInstance;
        }
        // 记录结束时间
        DateTime endTime = DateTime.Now;
        TimeSpan duration = endTime - startTime;
        Debug.Log($"GenerateLevelTest 执行时间: {duration.TotalMilliseconds} 毫秒");

        StartCoroutine( PostProcessTile(TilePostProcessType.Church));
        yield break;
    }

    [SerializeField] private TileBase blackTile;
    [SerializeField] private Tilemap blackTilemap;

    private IEnumerator PostProcessTile(TilePostProcessType postProcessType)
    {
        // 记录开始时间
        DateTime startTime = DateTime.Now;
        yield return null;

        // 找出最小和最大坐标范围
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;

        switch (postProcessType)
        {
            case TilePostProcessType.Church:
                {
                    // 用于记录所有已被房间覆盖的 Tile 坐标
                    HashSet<Vector3Int> occupiedTiles = new HashSet<Vector3Int>();

                    

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
                                occupiedTiles.Add(tilePos);

                                minX = Mathf.Min(minX, tilePos.x);
                                maxX = Mathf.Max(maxX, tilePos.x);
                                minY = Mathf.Min(minY, tilePos.y);
                                maxY = Mathf.Max(maxY, tilePos.y);
                            }
                        }
                    }

                    // 在整个地图范围内，填充未被占用的格子为黑色
                    for (int x = minX - 50; x <= maxX + 50; x++)
                    {
                        for (int y = minY - 50; y <= maxY + 50; y++)
                        {
                            Vector3Int pos = new Vector3Int(x, y, 0);
                            if (!occupiedTiles.Contains(pos))
                            {
                                blackTilemap.SetTile(pos, blackTile);
                            }
                        }
                    }

                    Debug.Log("黑色 tile 后处理完成");
                    break;
                }
        }

        
        

        // 记录结束时间
        DateTime endTime = DateTime.Now;
        TimeSpan duration = endTime - startTime;
        Debug.Log($"GenerateLevelTest 执行时间: {duration.TotalMilliseconds} 毫秒");
    }

    

    //解锁门 删除父节点和他所有子节点
    private void DeleteParentNodeAndChild(int currentParentNode)
    {
        //将parentNode的parent的对应的门也解锁 
        int grandpaNode = GetParentNodeByNode(currentParentNode);
        ActualRoomData grandpaRoom = roomDatas[grandpaNode - 1];
        for(int i = 0; i < grandpaRoom.doorInfos.Count; i++)
        {
            if (grandpaRoom.doorInfos[i].node == currentParentNode)
            {
                DoorInfo door = grandpaRoom.doorInfos[i];
                door.isLocked = false; // 解锁门
                door.node = -1; // 重置节点
                grandpaRoom.doorInfos[i] = door; // 将修改后的门放回列表
                break;
            }
        }
        roomDatas[grandpaNode-1] = grandpaRoom;

        // 逆序删除，避免索引变动问题
        for (int i = roomDatas.Count - 1; i >= 0; i--)
        {
            if (roomDatas[i].roomID >= currentParentNode)
            {
                // 从数据列表中移除
                isVisited[roomDatas[i].roomID] = false;
                roomDatas.RemoveAt(i);
            }
        }
    }


    //尝试生成房间
    private RoomGenerationResult TryGenerateRoom(int _currentNode,int _currentParentNode , 
        RoomNode _roomNode ,ActualRoomData parentRoomData)
    {
        int currentNode = _currentNode;
        int currentParentNode = _currentParentNode;
        Vector2 startPos = new Vector2(0,0);
        if(currentNode == 1 && currentParentNode == 0)
        {
            DoorInfo nodeDoor = new DoorInfo();
            List<LdtkLevelSO> properRooms = FindProperRooms(_roomNode, nodeDoor, out float avgHeight, out float avgWidth);
            LdtkLevelSO level = properRooms[UnityEngine.Random.Range(0, properRooms.Count)];
            ActualRoomData roomData = InitializeRoom(_roomNode, level, startPos);
            roomDatas.Add(roomData);
            return new RoomGenerationResult(true, "成功生成入口房间", 200);
        }
        else if(currentNode > 1)
        {

            RoomGenerationResult result;
            LdtkLevelSO level = new LdtkLevelSO();
            result = GetCanGenerateRoom(currentNode, _roomNode);

            if (result.success == false)
            {
                return result;
            }
            DoorInfo randomDoor = new DoorInfo();

            DoorInfo nodeDoor = new DoorInfo();
            if (lastDoor.node != -1)
                randomDoor = lastDoor;
            else
                randomDoor = GetRandomDoor(parentRoomData);

            //首先我们对房间进行初筛
            List<LdtkLevelSO> properRooms = FindProperRooms(_roomNode, randomDoor, out float avgHeight, out float avgWidth);
            //对房间进行二次筛选  暂时不做 不太需要
            //基于提前验证的筛选方法FindHighPriorityRooms() 


            // 生成一个存储所有索引的列表，并打乱顺序
            List<int> availableIndices = Enumerable.Range(0, properRooms.Count).ToList();
            //System.Random rng = new System.Random(seed);
            System.Random rng = new System.Random();
            availableIndices = availableIndices.OrderBy(_ => rng.Next()).ToList(); // 洗牌算法

            //对所有可用房间进行尝试
            do
            {
                if (availableIndices.Count == 0)
                {
                    Debug.LogWarning("所有房间都尝试过了，没有可用房间。");
                    result = new RoomGenerationResult(false, "已经没有可以生成的对应房间了,删除父亲节点", 500);
                    break;
                }

                // 直接随机取一个未尝试过的房间，并从列表中移除
                int randomIndex = availableIndices[availableIndices.Count - 1];
                availableIndices.RemoveAt(availableIndices.Count - 1); // O(1) 删除操作


                level = properRooms[randomIndex];

                //通过两个房间位置和门确定新房间位置
                
                (startPos, nodeDoor) = PresetRoom(parentRoomData, randomDoor, level);

                //对它所要放的位置进行检验 每一个可用房间都试试
                result = IsAreaFree(startPos, level);
                Debug.Log(result.message);
                if (result.success == true)
                    break;
            }
            while (!result.success);

            if(result.success == true)
            {
                //放置该房间 设置ActualRoomData 
                ActualRoomData roomData = InitializeRoom(_roomNode, level, startPos);//此处的startPos为世界坐标。
                roomDatas.Add(roomData);
                //生成成功的时候 我们对门进行上锁
                LockDoor(roomData, parentRoomData, randomDoor, nodeDoor);
                lastDoor.node = -1;
                return new RoomGenerationResult(true, $"成功生成{_roomNode.roomType}房间", 200);
            }
            else
            {
                LockDir(_currentNode, _currentParentNode, randomDoor);
                return new RoomGenerationResult(false, "已经没有可以生成的对应房间了,删除父亲节点", 500);
            }


        }
        return new RoomGenerationResult(false, "未满足生成条件", 2);
    }

    //看看还有没有能生成门的方向 
    private RoomGenerationResult GetCanGenerateRoom(int currentNode, RoomNode roomNode)
    {

        Debug.Log(currentNode + "当前房间connectionRoom " + roomNode.connectionRoom.Count + "  " + isDoorClosed[currentNode].Count);
        if(roomNode.connectionRoom.Count <= 3 - isDoorClosed[currentNode].Count)
        {
            return new RoomGenerationResult(true, "在锁定规则下可以生成房间", 200);
        }
        return new RoomGenerationResult(false, "在锁定规则下不可以生成房间", 600);
    }

    //当子节点X方向放不了房间 我们锁住这个方向
    private void LockDir(int currentNode, int currentParentNode, DoorInfo randomDoor)
    {
        lastDoor = randomDoor;
        for(int i = isDoorClosed.Count; i >= 1; i--)
        {
            if(i == currentNode)
            {
                isDoorClosed[i].Add(randomDoor.direction);
                break;
            }
            isDoorClosed[i].Clear();
        }
    }

    //成功生成房间的时候，需要锁住父子节点的这个门
    private void LockDoor(ActualRoomData roomData, ActualRoomData parentRoomData, DoorInfo randomDoor, DoorInfo nodeDoor)
    {
        // 锁定子房间的门
        for (int i = 0; i < roomData.doorInfos.Count; i++)
        {
            if (roomData.doorInfos[i].startCell == nodeDoor.startCell && roomData.doorInfos[i].direction == nodeDoor.direction)
            {
                DoorInfo door = roomData.doorInfos[i]; // 复制出一个 DoorInfo
                door.isLocked = true; // 修改 isLocked
                door.node = parentRoomData.roomID;
                roomData.doorInfos[i] = door; // 重新赋值回列表
                break;
            }
        }
        // 锁定父房间的门
        for (int i = 0; i < parentRoomData.doorInfos.Count; i++)
        {
            if (parentRoomData.doorInfos[i].startCell == randomDoor.startCell && parentRoomData.doorInfos[i].direction == randomDoor.direction)
            {
                DoorInfo door = parentRoomData.doorInfos[i]; // 复制出一个 DoorInfo
                door.isLocked = true; // 修改 isLocked
                door.node = roomData.roomID;
                parentRoomData.doorInfos[i] = door; // 重新赋值回列表
                break;
            }
        }
        //赋值
        for(int i = 0; i < roomDatas.Count; i++)
        {
            if (roomDatas[i].roomID == roomData.roomID)
                roomDatas[i] = roomData;
            if (roomDatas[i].roomID == parentRoomData.roomID)
                roomDatas[i] = parentRoomData;
        }
        Debug.Log("锁上了 "+ roomData.roomID + " " + parentRoomData.roomID);
        return;
    }

    //基于RECT确定能不能放上去
    private RoomGenerationResult IsAreaFree(Vector2 startPos, LdtkLevelSO level)
    {
        Rect newRoomRect = new Rect(startPos, new Vector2(level.levelWidth, level.levelHeight));
        foreach (var roomData in roomDatas)
        {
            // 以已有房间的 startPosition 为左下角，构造已有房间的矩形区域
            Rect existingRoomRect = new Rect(roomData.startPosition, new Vector2(roomData.levelWidth, roomData.levelHeight));

            // 使用 Rect.Overlaps 进行 AABB 碰撞检测
            if (newRoomRect.Overlaps(existingRoomRect))
            {
                string msg = $"新房间区域 {newRoomRect} 与房间 {roomData.roomID} 区域 {existingRoomRect} 重叠。";
                return new RoomGenerationResult(false, msg, 1);
            }
        }

        return new RoomGenerationResult(true, "区域空闲", 0);
    }

    //基于房间和门位置确定新房间的位置
    private (Vector2 startPos, DoorInfo nodeDoor) PresetRoom(ActualRoomData parentRoomData, DoorInfo randomDoor, LdtkLevelSO level)
    {
        DoorInfo nodeDoor = new DoorInfo();
        foreach (var door in level.doorInfos)
        {
            if (door.direction == GetOppositeDoorDir(randomDoor.direction))
            {
                nodeDoor = door;
            }
        }
        //startPos + startCell - startCell = new startPos
        Vector2 startPos = parentRoomData.startPosition;
        startPos += new Vector2(randomDoor.startCell.x, randomDoor.startCell.y);
        startPos -= new Vector2(nodeDoor.startCell.x, nodeDoor.startCell.y);

        //基于方向的偏差值1
        switch (nodeDoor.direction)
        {
            case DoorDir.Left:
                startPos += new Vector2(1, 0);
                break;
            case DoorDir.Right:
                startPos += new Vector2(-1, 0);
                break;
            case DoorDir.Top:
                startPos += new Vector2(0, -1);
                break;
            case DoorDir.Bottom:
                startPos += new Vector2(0, 1);
                break;
        }

        return (startPos,nodeDoor);

    }

    //获取随机门
    private DoorInfo GetRandomDoor(ActualRoomData parentRoomData)
    {
        List<DoorInfo> doors = new List<DoorInfo>();
        foreach (var door in parentRoomData.doorInfos)
        {
            if (!door.isLocked)
                doors.Add(door);
        }
        int random = UnityEngine.Random.Range(0, doors.Count);
        DoorInfo randomDoor = doors[random];
        return randomDoor;
    }

    //
    private (int nextNode, int nextParentNode) SelectNextNode(int node)
    {
        int nextnode = node + 1;
        int nextParentNode = 0;
        if (node == roomGraph.allRooms.Count)
        {
            return (0, 0);
        }
        //在生成房间下 分叉房间比主干房间编号更小。因此无脑遍历向后遍历即可 如果没有被visit 则找它和它的父亲节点（第二次遍历 on）1
        if (!isVisited[nextnode])
        {
            nextParentNode = GetParentNodeByNode(nextnode);
        }

        return (nextnode, nextParentNode);
    }

    private int GetParentNodeByNode(int node)
    {
        int parentNode = 0;
        for (int i = 0; i < roomGraph.allRooms.Count; i++)
        {
            if (roomGraph.allRooms[i].connectionRoom.Contains(node))
            {
                parentNode = i + 1; break;
            }
        }
        return parentNode;
    }


    private ActualRoomData InitializeRoom(RoomNode _roomNode, LdtkLevelSO _level,Vector2 startPosition)
    {
        ActualRoomData roomData = new ActualRoomData();
        roomData.roomID = _roomNode.roomID;
        roomData.room = _level;
        roomData.startPosition = startPosition;
        roomData.gameRoomType = _level.gameRoomType;
        roomData.levelHeight = _level.levelHeight;
        roomData.levelWidth = _level.levelWidth;
        // 手动深拷贝 doorInfos
        roomData.doorInfos = new List<DoorInfo>();
        roomData.lightPrefabs = new List<LightPrefab>();
        foreach (var door in _level.doorInfos)
        {
            DoorInfo doorInfo = door;
            roomData.doorInfos.Add(doorInfo);
        }
        roomData.connectionRoom = _roomNode.connectionRoom;
        return roomData;
    }

    //基于房间类型 门的数量 门的方向 不添加被封锁方向门 找到合适的房间~
    private List<LdtkLevelSO> FindProperRooms(RoomNode _roomNode, DoorInfo randomDoor , out float avgHeight, out float avgWidth)
    {
        List<LdtkLevelSO> list = new List<LdtkLevelSO>();
        float totalHeight = 0f;
        float totalWidth = 0f;

        //第一个总是很特殊 不是吗
        if(randomDoor.Equals(default(DoorInfo)))
        {
            foreach (var levelSO in levelList.ldtkLevelSoList)
            {
                if (levelSO.gameRoomType == _roomNode.roomType)
                {
                    list.Add(levelSO);
                    totalHeight += levelSO.levelHeight;
                    totalWidth += levelSO.levelWidth;
                }
            }
        }
        else
        {
            int roomDoorNum;
            /*if (_roomNode.roomType == GameRoomType.Entrance)
                roomDoorNum = 1;
            else*/
            roomDoorNum = _roomNode.connectionRoom.Count + 1;


            foreach (var levelSO in levelList.ldtkLevelSoList)
            {
                //一开始只通过这个判断 后续应该通过门方向大小判断
                if(levelSO.gameRoomType != _roomNode.roomType)
                {
                    //Debug.Log("类型不对");
                    continue;
                }
                if(levelSO.doorInfos.Count != roomDoorNum)
                {
                    //Debug.Log("门数量不对");
                    continue;
                }
                bool canAdd = false;
                foreach(var door in levelSO.doorInfos)
                {
                    if(door.direction == GetOppositeDoorDir(randomDoor.direction))
                    {
                        //Debug.Log("可以加");
                        canAdd = true;
                    }
                    if (isDoorClosed[_roomNode.roomID].Contains(door.direction))
                    {
                        Debug.Log("真能干");
                        continue;
                    }
                }
                if (!canAdd)
                    continue;

                //Debug.Log("方向也对");
                list.Add(levelSO);
                totalHeight += levelSO.levelHeight;
                totalWidth += levelSO.levelWidth;
                

            }
        }
        
        int count = list.Count;
        Debug.Log(count);
        avgHeight = count > 0 ? totalHeight / count : 0f;
        avgWidth = count > 0 ? totalWidth / count : 0f;
        return list;
    }

    private DoorDir GetOppositeDoorDir(DoorDir dir)
    {
        DoorDir oppositeDir = new DoorDir();
        if(dir == DoorDir.Left)
            oppositeDir = DoorDir.Right;
        if (dir == DoorDir.Right)
            oppositeDir = DoorDir.Left;
        if(dir == DoorDir.Top)
            oppositeDir = DoorDir.Bottom;
        if(dir == DoorDir.Bottom)
            oppositeDir = DoorDir.Top;
        return oppositeDir;
    }

}


[CustomEditor(typeof(RoomGraphGenerator))]
public class RoomGraphEditorGenerator : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RoomGraphGenerator generator = (RoomGraphGenerator)target;

        GUILayout.Space(10);
        if (GUILayout.Button("1", GUILayout.Height(40)))
        {
            // 停止所有协程
            generator.StopAllCoroutines();
            generator.StartCoroutine(generator.GenerateLevelTest());
        }

        if (GUILayout.Button("2", GUILayout.Height(40)))
        {
            
        }

        if (GUILayout.Button("3", GUILayout.Height(40)))
        {
            
        }
    }
}

