using System;
using System.Collections.Generic;
using LDtkUnity;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Experimental.GraphView.GraphView;

public enum DoorDirection { Left, Right, Top, Bottom, Default }
public enum RoomType { BattleRoom, Corridor}

public class LdtkTest : MonoBehaviour
{
    //房间和走廊都存在这里 提前弄好
    [SerializeField] private List<LDtkComponentLevel> rooms;
    [SerializeField] private List<LDtkComponentLevel> corridors;

    //房间数据
    [SerializeField] private List<RoomData> roomDatas = new List<RoomData>();

    //没什么用 先放着
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private LDtkComponentLevel level;


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
        public Vector2 Position;
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
        Door door = FindRoomDoors(initialRoom,DoorDirection.Default,RoomType.BattleRoom);
        SetRoomData(initialRoom,new Vector2(0,0));
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
            unconnectedRoomDoors.RemoveAt(0);

            // 寻找匹配的走廊并生成
            LDtkComponentLevel corridor = FindMatchingRoom(currentDoor.Direction, RoomType.Corridor);
            LDtkComponentLevel corridorInstance = Instantiate(corridor, Vector3.zero, Quaternion.identity);

            //找到走廊还没有被连接的门 返回刚刚和房间连接的门
            Door positionDoor = FindRoomDoors(corridorInstance, currentDoor.Direction, RoomType.Corridor);

            // 根据房间位置 门位置找到这个走廊的位置并设置
            Vector3 corridorPosition = CalculateCorridorPosition(currentDoor, positionDoor);
            corridorInstance.transform.position = corridorPosition;
            SetRoomData(corridorInstance, corridorPosition);

            //给走廊添加对应的房间
            ConnectNewRoom(corridorInstance);
        }
    }

    //给走廊连接房间
    private void ConnectNewRoom(LDtkComponentLevel corridor)
    {
        //必须给每个走廊连接房间！
        while (unconnectedCorridorDoors.Count > 0)
        {
            //找门
            Door currentDoor = unconnectedCorridorDoors[0];
            Debug.Log("hadhadhsodaodaod" + currentDoor.Position);
            unconnectedCorridorDoors.RemoveAt(0);

            // 寻找匹配的房间
            LDtkComponentLevel room = FindMatchingRoom(currentDoor.Direction,RoomType.Corridor);
            if (room == null) continue;

            // 寻找出口门
            Door positionDoor = FindRoomDoors(room, currentDoor.Direction, RoomType.Corridor);

            // 计算房间位置 并生成1
            Vector3 roomPosition = CalculateCorridorPosition(currentDoor, positionDoor);
            LDtkComponentLevel newRoom = Instantiate(room, roomPosition, Quaternion.identity);
            SetRoomData(newRoom, roomPosition);
        }
    }

    //通过level和位置设置roomdata
    public void SetRoomData(LDtkComponentLevel room,Vector2 startPosition)
    {
        roomDatas.Add(new RoomData
        {
            room = room,
            startPosition = startPosition,
            width = room.BorderRect.width,
            height = room.BorderRect.height
        });
    }

    
    //一开始生成随机的room就好了
    private LDtkComponentLevel InstantiateRandomRoom(Vector3 position)
    {
        LDtkComponentLevel prefab = rooms[UnityEngine.Random.Range(0, rooms.Count)];
        return Instantiate(prefab, position, Quaternion.identity);
    }

    //
    private Door FindRoomDoors(LDtkComponentLevel room, DoorDirection doorDir,RoomType roomType)//参数DoorDir是和他连接的room的门的方向
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

        //遍历门tilemap的每个tile
        //cellPos是这个level的相对坐标
        foreach (Vector3Int cellPos in doorLayer.cellBounds.allPositionsWithin)
        {
            if(roomType == RoomType.BattleRoom)
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
                    //如果这个门不是当前另一个房间的门所对应的连接门，则添加到未连接
                    if (doorDir != GetOppositeDirection(dir))
                    {
                        unconnectedRoomDoors.Add(new Door
                        {
                            Position = new Vector2(cellPos.x, cellPos.y),
                            Direction = dir,
                            ParentLevel = room
                        });
                    }
                    //如果这个门是当前另一个房间的门对应的连接门，则不添加，但此时需要添加到对应门并返回
                    else
                    {
                        if (positionDoor == null)
                        {
                            positionDoor = new Door
                            {
                                Position = new Vector2(cellPos.x, cellPos.y),
                                Direction = dir,
                                ParentLevel = room
                            };
                        }
                    }
                }
            }
            else if(roomType == RoomType.Corridor)
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
                    if (doorDir != GetOppositeDirection(dir))
                    {
                        unconnectedCorridorDoors.Add(new Door
                        {
                            Position = new Vector2(cellPos.x, cellPos.y),
                            Direction = dir,
                            ParentLevel = room
                        });
                    }
                    else
                    {
                        if (positionDoor == null)
                        {
                            positionDoor = new Door
                            {
                                Position = new Vector2(cellPos.x, cellPos.y),
                                Direction = dir,
                                ParentLevel = room
                            };
                        }
                    }
                }
                //Debug.Log(unconnectedCorridorDoors.Count);
            }
            //Debug.Log(unconnectedRoomDoors.Count);
        }
        return positionDoor;
    }
    private Door FindCorridorDoors(LDtkComponentLevel room, DoorDirection doorDir)
    {
        Transform doorTransform = room.transform.Find("Door");
        if (doorTransform == null)
        {
            Debug.LogError($"No Door object found in room: {room.name}");
            return null;
        }
        Tilemap doorLayer = doorTransform.GetComponentInChildren<Tilemap>();
        if (doorLayer == null)
        {
            Debug.LogError($"No Tilemap component found on Door object in room: {room.name}");
            return null;
        }

        Rect levelRect = room.BorderRect;
        //此时获取当前对着的门
        Door positionDoor = null;


        foreach (Vector3Int cellPos in doorLayer.cellBounds.allPositionsWithin)
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
                if(doorDir != GetOppositeDirection(dir))
                {
                    unconnectedCorridorDoors.Add(new Door
                    {
                        Position = new Vector2(cellPos.x, cellPos.y),
                        Direction = dir,
                        ParentLevel = room
                    });
                }
                else
                {
                    if(positionDoor == null)
                    {
                        positionDoor = new Door
                        {
                            Position = new Vector2(cellPos.x,cellPos.y),
                            Direction = dir,
                            ParentLevel = room
                        };
                    }
                }
            }
            //Debug.Log(unconnectedCorridorDoors.Count);
        }
        return positionDoor;
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
        if (doorLayer == null)
        {
            Debug.LogError($"No Tilemap component found on Door object in room: {room.name}");
            return null;
        }

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
    private LDtkComponentLevel FindMatchingCorridor(DoorDirection doorDir)
    {
        DoorDirection requiredExit = GetOppositeDirection(doorDir);
        foreach (LDtkComponentLevel corridor in corridors)
        {
            //获取每个房间的方向
            List<DoorDirection> doorDirections = GetRoomDoorDirList(corridor);
            if (doorDirections.Contains(requiredExit))
            {
                Debug.Log("有这样的走廊");
                return corridor;
            }
        }
        return null;
    }

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
                    Debug.Log("有这样的房间");
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
                    Debug.Log("有这样的走廊");
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
            //Debug.Log("这是" + data.room);
            if (data.room.Equals(door.ParentLevel))
            {
                Debug.Log("找到了" + door.ParentLevel);
                roomData = data;
                break;
            }
        }
        //门的位置是door的房间的起始点+door的相对位置
        Vector2 doorPosition =  roomData.startPosition + door.Position;
        //新房间的位置则是门的位置减去门在新房间的相对位置
        Vector2 newRoomPosition = doorPosition - offsetDoor.Position;
        //给一点每个方向的简单偏移
        switch (door.Direction)
        {
            case DoorDirection.Right:
                return newRoomPosition + new Vector2(1f, 0f);
            case DoorDirection.Left:
                return newRoomPosition + new Vector2(-1f, 0f);
            case DoorDirection.Top:
                return newRoomPosition + new Vector2(0f, 1f);
            case DoorDirection.Bottom:
                return newRoomPosition + new Vector2(0f, 1f);
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

    /*//没用
    private Vector3 CalculateRoomPosition(Door corridorExit)
    {
        float roomOffset = 5f; // 根据房间尺寸调整
        return corridorExit.Position + GetDirectionVector(corridorExit.Direction) * roomOffset;
    }*/

    /*private Vector2 GetDirectionVector(DoorDirection dir)
    {
        return dir switch
        {
            DoorDirection.Right => Vector2.right,
            DoorDirection.Left => Vector2.left,
            DoorDirection.Top => Vector2.up,
            DoorDirection.Bottom => Vector2.down,
            _ => Vector2.zero
        };
    }*/

    // 其他已有方法...

    public void OutputSomeMsg()
    {

        Debug.Log(level.BorderBounds);
        Debug.Log(level.BorderRect);
    }
}