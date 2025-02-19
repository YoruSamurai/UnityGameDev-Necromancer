/*using System;
using System.Collections.Generic;
using LDtkUnity;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Experimental.GraphView.GraphView;

public enum DoorDirection { Left, Right, Top, Bottom, Default }

public class LdtkTest : MonoBehaviour
{
    // 序列化字段
    [SerializeField] private List<LDtkComponentLevel> rooms;
    [SerializeField] private List<LDtkComponentLevel> corridors;
    [SerializeField] private List<RoomData> roomDatas = new List<RoomData>();
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private LDtkComponentLevel level;


    // 门数据类
    [Serializable]
    public class RoomData
    {
        public LDtkComponentLevel room;
        public Vector2 startPosition;
        public float width;
        public float height;
    }

    // 门数据类
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

    public void GenerateLevelTest()
    {
        // 清理旧关卡
        ClearExistingLevels();

        // 生成初始房间
        LDtkComponentLevel initialRoom = InstantiateRandomRoom(Vector3.zero);
        Door door = FindRoomDoors(initialRoom, DoorDirection.Default);
        SetRoomData(initialRoom, new Vector2(0, 0));
        // 开始连接流程
        ProcessUnconnectedDoors();
    }

    public void SetRoomData(LDtkComponentLevel room, Vector2 startPosition)
    {
        roomDatas.Add(new RoomData
        {
            room = room,
            startPosition = startPosition,
            width = room.BorderRect.width,
            height = room.BorderRect.height
        });
    }

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

    private LDtkComponentLevel InstantiateRandomRoom(Vector3 position)
    {
        LDtkComponentLevel prefab = rooms[UnityEngine.Random.Range(0, rooms.Count)];
        return Instantiate(prefab, position, Quaternion.identity);
    }

    private Door FindRoomDoors(LDtkComponentLevel room, DoorDirection doorDir)
    {
        *//*Debug.Log(level.BorderBounds);
        Debug.Log(level.BorderRect);*//*
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
        //此时获取当前对着的门
        Door positionDoor = null;

        foreach (Vector3Int cellPos in doorLayer.cellBounds.allPositionsWithin)
        {
            if (!doorLayer.HasTile(cellPos)) continue;

            //Vector3 worldPos = doorLayer.GetCellCenterWorld(cellPos);
            DoorDirection dir = GetDoorDirection(cellPos, levelRect);
            //Debug.Log(cellPos + " " + worldPos);
            //Debug.Log(dir);
            //能不能加门
            bool canAddDoor = true;
            //同一个房间 同一个方向 就不能加了
            foreach (var door in unconnectedRoomDoors)
            {
                if (door.Direction == dir && door.ParentLevel == room)
                {
                    canAddDoor = false;
                    break;
                }
            }

            //发现可以加 并且不是当前对着的门
            if (canAddDoor)
            {
                if (doorDir != GetOppositeDirection(dir))
                {
                    unconnectedRoomDoors.Add(new Door
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
            //Debug.Log(unconnectedRoomDoors.Count);
        }
        return positionDoor;
    }
    private Door FindCorridorDoors(LDtkComponentLevel room, DoorDirection doorDir)
    {
        *//*Debug.Log(level.BorderBounds);
        Debug.Log(level.BorderRect);*//*
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
        //此时获取当前对着的门
        Door positionDoor = null;


        foreach (Vector3Int cellPos in doorLayer.cellBounds.allPositionsWithin)
        {
            if (!doorLayer.HasTile(cellPos)) continue;

            //Vector3 worldPos = doorLayer.GetCellCenterWorld(cellPos);
            DoorDirection dir = GetDoorDirection(cellPos, levelRect);
            //Debug.Log(cellPos + " " + worldPos);
            //Debug.Log(dir);

            //能不能加门
            bool canAddDoor = true;
            //同一个房间 同一个方向 就不能加了
            foreach (var door in unconnectedCorridorDoors)
            {
                if (door.Direction == dir && door.ParentLevel == room)
                {
                    canAddDoor = false;
                    break;
                }
            }

            //发现可以加 并且不是当前对着的门
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

            Debug.Log(unconnectedCorridorDoors.Count);
        }
        return positionDoor;
    }
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

        foreach (Vector3Int cellPos in doorLayer.cellBounds.allPositionsWithin)
        {
            if (!doorLayer.HasTile(cellPos)) continue;

            //Vector3 worldPos = doorLayer.GetCellCenterWorld(cellPos);  
            DoorDirection dir = GetDoorDirection(cellPos, levelRect);
            if (!doorDirections.Contains(dir))
            {
                doorDirections.Add(dir);
                //Debug.Log("走廊方向为" +  dir);
            }
        }
        Debug.Log("走廊有x额方向" + doorDirections.Count);
        return doorDirections;
    }


    private DoorDirection GetDoorDirection(Vector3 cellPos, Rect levelRect)
    {
        float tolerance = 1f;
        //Debug.Log(levelRect.xMin + " " + levelRect.width + " " + levelRect.yMin + " " + levelRect.height + " ");
        if (Mathf.Abs(cellPos.x + .5f) < tolerance) return DoorDirection.Left;
        if (Mathf.Abs(cellPos.x + .5f - levelRect.width) < tolerance) return DoorDirection.Right;
        if (Mathf.Abs(cellPos.y + .5f) < tolerance) return DoorDirection.Bottom;
        if (Mathf.Abs(cellPos.y + .5f - levelRect.height) < tolerance) return DoorDirection.Top;
        return DoorDirection.Left; // 默认值
    }

    private void ProcessUnconnectedDoors()
    {
        while (unconnectedRoomDoors.Count > 0)
        {
            Door currentDoor = unconnectedRoomDoors[0];
            unconnectedRoomDoors.RemoveAt(0);

            // 寻找匹配的走廊
            LDtkComponentLevel corridor = FindMatchingCorridor(currentDoor.Direction);
            LDtkComponentLevel corridorInstance = Instantiate(corridor, Vector3.zero, Quaternion.identity);
            Debug.Log("adsadasdasda" + corridorInstance);

            // 寻找出口门
            Door positionDoor = FindCorridorDoors(corridorInstance, currentDoor.Direction);



            // 计算走廊位置
            Vector3 corridorPosition = CalculateCorridorPosition(currentDoor, positionDoor);
            corridorInstance.transform.position = corridorPosition;
            SetRoomData(corridorInstance, corridorPosition);

            //此时应当给所有走廊锁门 对吧
            // 连接新房间
            ConnectNewRoom(corridorInstance);
        }
    }

    private LDtkComponentLevel FindMatchingCorridor(DoorDirection doorDir)
    {
        DoorDirection requiredExit = GetOppositeDirection(doorDir);

        foreach (LDtkComponentLevel corridor in corridors)
        {
            List<DoorDirection> doorDirections = GetRoomDoorDirList(corridor);
            if (doorDirections.Contains(requiredExit))
            {
                Debug.Log("有这样的走廊");
                return corridor;
            }
            *//*if (corridor.Identifier.Contains(requiredExit.ToString()))
                return corridor;*//*
        }
        return null;
    }

    private LDtkComponentLevel FindMatchingRoom(DoorDirection doorDir)
    {
        DoorDirection requiredExit = GetOppositeDirection(doorDir);

        foreach (LDtkComponentLevel room in rooms)
        {
            List<DoorDirection> doorDirections = GetRoomDoorDirList(room);
            if (doorDirections.Contains(requiredExit))
            {
                Debug.Log("有这样的房间");
                return room;
            }
            *//*if (corridor.Identifier.Contains(requiredExit.ToString()))
                return corridor;*//*
        }
        return null;
    }

    private Vector3 CalculateCorridorPosition(Door door, Door offsetDoor)
    {
        RoomData roomData = new RoomData();
        Debug.Log("这是" + door.ParentLevel);
        foreach (var data in roomDatas)
        {
            Debug.Log("这是" + data.room);
            if (data.room.Equals(door.ParentLevel))
            {
                Debug.Log("找到了" + door.ParentLevel);
                roomData = data;
                break;
            }
        }
        Vector2 doorPosition = roomData.startPosition + door.Position;
        Vector2 newRoomPosition = doorPosition - offsetDoor.Position;
        Debug.Log(door.Position + "                " + offsetDoor.Position);

        switch (door.Direction)
        {
            case DoorDirection.Right:
                return newRoomPosition + new Vector2(1f, 0f);
            case DoorDirection.Left:
                return newRoomPosition + new Vector2(-1f, 0f);
            case DoorDirection.Top:
                return door.Position - offsetDoor.Position;
            case DoorDirection.Bottom:
                return door.Position - offsetDoor.Position;
            default: return Vector3.zero;
        }
    }

    private void ConnectNewRoom(LDtkComponentLevel corridor)
    {
        while (unconnectedCorridorDoors.Count > 0)
        {
            Door currentDoor = unconnectedCorridorDoors[0];
            Debug.Log("hadhadhsodaodaod" + currentDoor.Position);
            unconnectedCorridorDoors.RemoveAt(0);

            // 寻找匹配的房间
            LDtkComponentLevel room = FindMatchingRoom(currentDoor.Direction);
            if (room == null) continue;

            // 寻找出口门
            Door positionDoor = FindRoomDoors(room, currentDoor.Direction);

            // 计算走廊位置
            Vector3 roomPosition = CalculateCorridorPosition(currentDoor, positionDoor);
            LDtkComponentLevel newRoom = Instantiate(room, roomPosition, Quaternion.identity);
            SetRoomData(newRoom, roomPosition);
        }



        //FindRoomDoors(newRoom);
    }

    private DoorDirection GetOppositeDirection(DoorDirection dir)
    {
        return dir switch
        {
            DoorDirection.Left => DoorDirection.Right,
            DoorDirection.Right => DoorDirection.Left,
            DoorDirection.Top => DoorDirection.Bottom,
            DoorDirection.Bottom => DoorDirection.Top,
            _ => DoorDirection.Left
        };
    }

    private Vector3 CalculateRoomPosition(Door corridorExit)
    {
        float roomOffset = 5f; // 根据房间尺寸调整
        return corridorExit.Position + GetDirectionVector(corridorExit.Direction) * roomOffset;
    }

    private Vector2 GetDirectionVector(DoorDirection dir)
    {
        return dir switch
        {
            DoorDirection.Right => Vector2.right,
            DoorDirection.Left => Vector2.left,
            DoorDirection.Top => Vector2.up,
            DoorDirection.Bottom => Vector2.down,
            _ => Vector2.zero
        };
    }

    // 其他已有方法...

    public void OutputSomeMsg()
    {

        Debug.Log(level.BorderBounds);
        Debug.Log(level.BorderRect);
    }
}*/