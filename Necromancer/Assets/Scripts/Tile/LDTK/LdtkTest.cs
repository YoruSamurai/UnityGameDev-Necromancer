using System;
using System.Collections.Generic;
using LDtkUnity;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum DoorDirection { Left, Right, Top, Bottom, Default }
public enum RoomType { BattleRoom, Corridor}

public class LdtkTest : MonoBehaviour
{
    //房间和走廊都存在这里 提前弄好
    [SerializeField] private List<LDtkComponentLevel> rooms;
    [SerializeField] private List<LDtkComponentLevel> corridors;

    //房间数据
    [SerializeField] private List<RoomData> roomDatas = new List<RoomData>();

    //父节点 放地图
    [SerializeField] private Transform mapParent;


    //房间数据 存储了：房间 房间起始点 房间长度和宽度
    [Serializable]
    public class RoomData
    {
        public LDtkComponentLevel room;
        public Vector2 startPosition;
        public float width;
        public float height;
    }

    // 门数据类 存储了门位置（什么位置？）房间方向 门对应的room
    [Serializable]
    public class Door
    {
        public Vector2 relativePosition;
        public Vector2 worldPosition;
        public DoorDirection Direction;
        public LDtkComponentLevel ParentLevel;
    }

    // 当前未连接的门列表
    [SerializeField] private List<Door> unconnectedRoomDoors = new List<Door>();
    [SerializeField] private List<Door> unconnectedCorridorDoors = new List<Door>();

    //每次生成前先清空
    private void ClearExistingLevels()
    {
        foreach (var level in FindObjectsOfType<LDtkComponentLevel>())
        {
            if (level.gameObject.name.Contains("Clone"))
                DestroyImmediate(level.gameObject);
        }

        roomDatas.Clear();
        unconnectedRoomDoors.Clear();
        unconnectedCorridorDoors.Clear();
    }

    public void GenerateLevelTest()
    {
        // 清理旧关卡
        ClearExistingLevels();
        // 生成初始房间
        LDtkComponentLevel initialRoom = InstantiateRandomRoom(Vector3.zero);
        //生成房间对应的门加入unconnectedRoomDoors 然后设置roomdata
        //Door door = FindRoomDoors(initialRoom,null,RoomType.BattleRoom);
        SetUpRoom(initialRoom, null, RoomType.BattleRoom);
        //SetRoomData(initialRoom,new Vector2(0,0));
        // 开始连接流程
        ProcessUnconnectedDoors();
    }

    //主要流程
    private void ProcessUnconnectedDoors()
    {
        //只要还有房间未闭合 就不断执行
        while (unconnectedRoomDoors.Count > 0)
        {
            //获取当前门 并丢掉 我们通过这个门找对应的走廊
            Door currentDoor = unconnectedRoomDoors[0];
            Debug.Log(currentDoor.relativePosition);
            unconnectedRoomDoors.RemoveAt(0);

            // 寻找匹配的走廊并生成
            LDtkComponentLevel corridor = FindMatchingRoom(currentDoor.Direction, RoomType.Corridor);

            LDtkComponentLevel instance = SetUpRoom(corridor, currentDoor, RoomType.Corridor);


            ConnectNewRoom(instance);
        }
    }

    //给走廊连接房间
    private void ConnectNewRoom(LDtkComponentLevel corridor)
    {
        //必须给每个走廊连接房间！
        int counter = 100;
        while (unconnectedCorridorDoors.Count > 0 && counter >0)
        {
            counter--;
            //找门
            Door currentDoor = unconnectedCorridorDoors[0];
            Debug.Log("当前门位置" + currentDoor.worldPosition);
            unconnectedCorridorDoors.RemoveAt(0);
            // 寻找匹配的房间
            LDtkComponentLevel room = FindMatchingRoom(currentDoor.Direction,RoomType.BattleRoom);
            if (room == null)
            {
                Debug.LogError("没有找到对应房间");
            }
            LDtkComponentLevel instance = SetUpRoom(room, currentDoor, RoomType.BattleRoom);

        }
        Debug.Log(corridor + "已闭合");
    }

    private RoomData GetRoomData(LDtkComponentLevel room)
    {
        RoomData roomData = new RoomData();
        foreach (var data in roomDatas)
        {
            if (data.room.Equals(room))
            {
                roomData = data;
                break;
            }
        }
        return roomData;
    }

    //通过level和位置设置roomdata
    public void SetRoomData(LDtkComponentLevel room,Door currentDoor,Door positionDoor)
    {
        if(positionDoor == null)
        {
            roomDatas.Add(new RoomData
            {
                room = room,
                startPosition = new Vector2(0,0),
                width = room.BorderRect.width,
                height = room.BorderRect.height
            });
        }
        else
        {
            roomDatas.Add(new RoomData
            {
                room = room,
                startPosition = currentDoor.worldPosition - positionDoor.relativePosition,
                width = room.BorderRect.width,
                height = room.BorderRect.height
            });
        }
        
    }

    
    //一开始生成随机的room就好了
    private LDtkComponentLevel InstantiateRandomRoom(Vector3 position)
    {
        LDtkComponentLevel prefab = rooms[UnityEngine.Random.Range(0, rooms.Count)];
        return prefab;
    }

    
    //在这里初始化房间 初始化门 设置位置
    private LDtkComponentLevel SetUpRoom(LDtkComponentLevel room, Door currentDoor, RoomType roomType)
    {
        //找到这个房间对应的tileMap
        Transform doorTransform = room.transform.Find("Door");
        if (doorTransform == null)
        {
            Debug.LogError($"No Door object found in room: {room.name}");
            return null;
        }

        // 获取 Door 物体的 Tilemap 组件
        Tilemap doorLayer = doorTransform.GetComponentInChildren<Tilemap>();
        if (doorLayer == null)
        {
            Debug.LogError($"No Tilemap component found on Door object in room: {room.name}");
            return null;
        }

        Rect levelRect = room.BorderRect;
        //和他连接的房间的门的对应门
        Door positionDoor = null;
        Vector2 worldPos = new Vector2();
        if (currentDoor != null)
        {
            worldPos = currentDoor.worldPosition;
        }
        LDtkComponentLevel ldtkInstance = null;
        //进行第一次遍历 初始化第一个门/找到对应门
        positionDoor = FindInitialDoor(room, currentDoor, roomType, doorLayer);
        if(positionDoor == null)
        {
            Debug.Log("初始房间一位");
            //生成实例 并set
            ldtkInstance = Instantiate(room, Vector3.zero, Quaternion.identity,mapParent);
            SetRoomData(ldtkInstance, currentDoor, positionDoor);
        }
        else
        {
            //生成实例 并set
            ldtkInstance = Instantiate(room, Vector3.zero, Quaternion.identity, mapParent);
            SetRoomData(ldtkInstance, currentDoor, positionDoor);
            //Vector3 worldPosition = CalculateCorridorPosition(currentDoor, positionDoor);
            Vector3 worldPosition = currentDoor.worldPosition - positionDoor.relativePosition;
            Debug.Log("世界坐标" +  worldPosition);
            ldtkInstance.transform.position = worldPosition;
            //第二次遍历 找到未被连接的门并加入
            Debug.Log("大神解答解答" + positionDoor.worldPosition);
            if(positionDoor != null)
            {
                FindOtherDoor(ldtkInstance, positionDoor, roomType, doorLayer);
            }
            
        }

        return ldtkInstance;
    }

    private Door FindInitialDoor(LDtkComponentLevel room, Door currentDoor, RoomType roomType, Tilemap doorLayer)
    {
        Rect levelRect = room.BorderRect;
        //和他连接的房间的门的对应门
        Door positionDoor = null;
        Vector2 worldPos = new Vector2();
        if (currentDoor != null)
        {
            worldPos = currentDoor.worldPosition;
        }
        //遍历门tilemap的每个tile
        //cellPos是这个level的相对坐标
        foreach (Vector3Int cellPos in doorLayer.cellBounds.allPositionsWithin)
        {
            if (roomType == RoomType.BattleRoom)
            {
                if (!doorLayer.HasTile(cellPos)) continue;

                DoorDirection dir = GetDoorDirection(cellPos, levelRect);//通过tile的位置和这个level的长宽获取门是在上下左右方向。
                                                                         //能不能加门
                bool canAddDoor = true;

                //对没有连接的门进行遍历 如果门的方向相同并且这两个门从属于一个房间 则无法往unconnectedRoomDoors加门（一个房间一个方向的门只能有一个）
                foreach (var door in unconnectedRoomDoors)
                {
                    if (door.Direction == dir && door.ParentLevel == room)
                    {
                        canAddDoor = false;
                        break;
                    }
                }
                //可以加门
                if (canAddDoor)
                {
                    if (currentDoor == null)
                    {
                        unconnectedRoomDoors.Add(new Door
                        {
                            relativePosition = new Vector2(cellPos.x, cellPos.y),
                            worldPosition = new Vector2(cellPos.x, cellPos.y),
                            Direction = dir,
                            ParentLevel = room
                        });
                    }
                    else if (currentDoor.Direction != GetOppositeDirection(dir))
                    {
                        continue;
                    }
                    //如果这个门是当前另一个房间的门对应的连接门，则不添加，但此时需要添加到对应门并返回
                    else
                    {
                        if (positionDoor == null)
                        {
                            positionDoor = new Door
                            {
                                relativePosition = new Vector2(cellPos.x, cellPos.y),
                                worldPosition = worldPos,
                                Direction = dir,
                                ParentLevel = room
                            };
                        }
                    }
                }
            }
            else if (roomType == RoomType.Corridor)
            {
                if (!doorLayer.HasTile(cellPos)) continue;
                DoorDirection dir = GetDoorDirection(cellPos, levelRect);

                bool canAddDoor = true;
                foreach (var door in unconnectedCorridorDoors)
                {
                    if (door.Direction == dir && door.ParentLevel == room)
                    {
                        canAddDoor = false;
                        break;
                    }
                }
                if (canAddDoor)
                {
                    if (currentDoor.Direction != GetOppositeDirection(dir))
                    {
                        continue;
                    }
                    else
                    {
                        if (positionDoor == null)
                        {
                            positionDoor = new Door
                            {
                                relativePosition = new Vector2(cellPos.x, cellPos.y),
                                worldPosition = worldPos,
                                Direction = dir,
                                ParentLevel = room
                            };
                        }
                    }
                }
            }
        }
        if (positionDoor != null)
        {
            Debug.Log("目标门" + positionDoor.ParentLevel + positionDoor.relativePosition + " " + positionDoor.worldPosition);
        }

        return positionDoor;
    }
    private void FindOtherDoor(LDtkComponentLevel room, Door currentDoor, RoomType roomType, Tilemap doorLayer)
    {
        Rect levelRect = room.BorderRect;
        Debug.Log("开始找其他门");
        //和他连接的房间的门的对应门
        Vector2 worldPos = new Vector2();
        if (currentDoor != null)
        {
            worldPos = currentDoor.worldPosition;
        }
        //遍历门tilemap的每个tile
        //cellPos是这个level的相对坐标
        foreach (Vector3Int cellPos in doorLayer.cellBounds.allPositionsWithin)
        {
            if (roomType == RoomType.BattleRoom)
            {
                if (!doorLayer.HasTile(cellPos)) continue;

                DoorDirection dir = GetDoorDirection(cellPos, levelRect);//通过tile的位置和这个level的长宽获取门是在上下左右方向。
                                                                         //能不能加门
                bool canAddDoor = true;

                //对没有连接的门进行遍历 如果门的方向相同并且这两个门从属于一个房间 则无法往unconnectedRoomDoors加门（一个房间一个方向的门只能有一个）
                foreach (var door in unconnectedRoomDoors)
                {
                    if (door.Direction == dir && door.ParentLevel == room)
                    {
                        canAddDoor = false;
                        break;
                    }
                }
                

                //可以加门
                if (canAddDoor)
                {
                    

                    if (currentDoor == null)
                    {
                        continue;
                    }
                    else if (currentDoor.Direction != (dir))
                    {
                        Debug.Log("储蓄额我是");
                        unconnectedRoomDoors.Add(new Door
                        {
                            relativePosition = new Vector2(cellPos.x, cellPos.y),
                            worldPosition = GetRoomData(room).startPosition + new Vector2(cellPos.x, cellPos.y),
                            //worldPosition = worldPos + new Vector2(cellPos.x, cellPos.y),
                            Direction = dir,
                            ParentLevel = room
                        });
                    }
                    //如果这个门是当前另一个房间的门对应的连接门，则不添加，但此时需要添加到对应门并返回
                    else
                    {
                        continue;
                    }
                }
            }
            else if (roomType == RoomType.Corridor)
            {
                if (!doorLayer.HasTile(cellPos)) continue;
                DoorDirection dir = GetDoorDirection(cellPos, levelRect);

                bool canAddDoor = true;
                foreach (var door in unconnectedCorridorDoors)
                {
                    if (door.Direction == dir && door.ParentLevel == room)
                    {
                        canAddDoor = false;
                        break;
                    }
                }
                if (canAddDoor)
                {
                    if (currentDoor.Direction != (dir))
                    {
                        Debug.Log("储蓄额ads我是");
                        Debug.Log(GetRoomData(room).startPosition + "  " + cellPos.x + cellPos.y);
                        unconnectedCorridorDoors.Add(new Door
                        {
                            relativePosition = new Vector2(cellPos.x, cellPos.y),
                            worldPosition = GetRoomData(room).startPosition + new Vector2(cellPos.x, cellPos.y),
                            Direction = dir,
                            ParentLevel = room
                        });
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        return;
    }

    
    

    //返回这个房间有多少方向的门及对应方向
    private List<DoorDirection> GetRoomDoorDirList(LDtkComponentLevel room)
    {
        List<DoorDirection> doorDirections = new List<DoorDirection>();


        Transform doorTransform = room.transform.Find("Door");
        if (doorTransform == null)
        {
            Debug.LogError($"No Door object found in room: {room.name}");
            return null;
        }

        // 获取 Door 物体的 Tilemap 组件
        Tilemap doorLayer = doorTransform.GetComponentInChildren<Tilemap>();
        TilemapRenderer doorRenderer = doorTransform.GetComponentInChildren<TilemapRenderer>();
        if (doorLayer == null)
        {
            Debug.LogError($"No Tilemap component found on Door object in room: {room.name}");
            return null;
        }
        Debug.Log(doorLayer);
        doorRenderer.sortingOrder = 5;
        doorRenderer.sortingLayerName = "Tile";
        Rect levelRect = room.BorderRect;

        //对每个tile都看他的方向并添加到方向list 返回
        foreach (Vector3Int cellPos in doorLayer.cellBounds.allPositionsWithin)
        {
            if (!doorLayer.HasTile(cellPos)) continue;
            DoorDirection dir = GetDoorDirection(cellPos, levelRect);
            if (!doorDirections.Contains(dir))
            {
                doorDirections.Add(dir);
            }
        }
        //Debug.Log("走廊有x额方向"+doorDirections.Count);
        return doorDirections;
    }

    //获取门的方向 这样就差不多了
    private DoorDirection GetDoorDirection(Vector3 cellPos, Rect levelRect)
    {
        float tolerance = 1f;
        if (Mathf.Abs(cellPos.x + .5f) < tolerance) return DoorDirection.Left;
        if (Mathf.Abs(cellPos.x + .5f - levelRect.width) < tolerance) return DoorDirection.Right;
        if (Mathf.Abs(cellPos.y + .5f) < tolerance) return DoorDirection.Bottom;
        if (Mathf.Abs(cellPos.y +.5f - levelRect.height) < tolerance) return DoorDirection.Top;
        return DoorDirection.Default; // 默认值
    }


    

    //通过对应门方向遍历房间 有就返回（也许后续可以做成那种 找列表 随即返回？）
    

    private LDtkComponentLevel FindMatchingRoom(DoorDirection doorDir,RoomType roomType)
    {
        DoorDirection requiredExit = GetOppositeDirection(doorDir);
        List<LDtkComponentLevel> roomList = new List<LDtkComponentLevel>();
        if(roomType == RoomType.BattleRoom)
        {
            foreach (LDtkComponentLevel room in rooms)
            {
                List<DoorDirection> doorDirections = GetRoomDoorDirList(room);
                if (doorDirections.Contains(requiredExit))
                {
                    //Debug.Log("有这样的房间");
                    //return room;
                    roomList.Add(room);
                }
            }
        }
        else if(roomType == RoomType.Corridor)
        {
            foreach (LDtkComponentLevel corridor in corridors)
            {
                //获取每个房间的方向
                List<DoorDirection> doorDirections = GetRoomDoorDirList(corridor);
                if (doorDirections.Contains(requiredExit))
                {
                    //Debug.Log("有这样的走廊");
                    //return corridor;
                    roomList.Add(corridor);
                }
            }
        }
        return roomList[UnityEngine.Random.Range(0,roomList.Count)];
    }

    //通过当前房间门 当前房间 对应门 对应门房间生成对应门房间位置
    private Vector3 CalculateCorridorPosition(Door door,Door offsetDoor)
    {
        RoomData roomData = new RoomData();
        //找到door的房间数据
        foreach (var data in roomDatas)
        {
            if (data.room.Equals(offsetDoor.ParentLevel))
            {
                roomData = data;
                break;
            }
        }


        //门的位置是door的房间的起始点+door的相对位置
        Vector2 doorPosition =  roomData.startPosition + door.relativePosition;
        //新房间的位置则是门的位置减去门在新房间的相对位置
        Vector2 newRoomPosition = doorPosition - offsetDoor.relativePosition;

        //Vector2 testPostion = offsetDoor.worldPosition - offsetDoor.relativePosition;   
        Vector2 testPostion = roomData.startPosition;
        //给一点每个方向的简单偏移
        switch (door.Direction)
        {
            case DoorDirection.Right:
                return testPostion ;
            case DoorDirection.Left:
                return testPostion ;
            case DoorDirection.Top:
                return testPostion ;
            case DoorDirection.Bottom:
                return testPostion ;
            /*case DoorDirection.Right:
                return newRoomPosition + new Vector2(1f, 0f);
            case DoorDirection.Left:
                return newRoomPosition + new Vector2(-1f, 0f);
            case DoorDirection.Top:
                return newRoomPosition + new Vector2(0f, 1f);
            case DoorDirection.Bottom:
                return newRoomPosition + new Vector2(0f, 1f);*/
            default: return Vector3.zero;
        }
    }

    

    //获取反方向
    private DoorDirection GetOppositeDirection(DoorDirection dir)
    {
        return dir switch
        {
            DoorDirection.Left => DoorDirection.Right,
            DoorDirection.Right => DoorDirection.Left,
            DoorDirection.Top => DoorDirection.Bottom,
            DoorDirection.Bottom => DoorDirection.Top,
            _ => DoorDirection.Default
        };
    }

    

    // 其他已有方法...

    public void OutputSomeMsg()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            Debug.Log(rooms[i].tag);
            Debug.Log(rooms[i].FieldInstances.GetEnum<GameRoomType>("GameRoomType"));


        }
    }
}