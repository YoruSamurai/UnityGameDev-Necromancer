using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomGraphData", menuName = "Level/Room Graph")]
public class RoomGraph : ScriptableObject
{
    public List<RoomNode> allRooms = new List<RoomNode>();

    // 如果需要根据roomID快速查找，可以自己维护一个字典
    // public Dictionary<string, RoomNode> nodeLookup = new Dictionary<string, RoomNode>();

    // 也可以添加一些方法，诸如 AddNode、RemoveNode、GetNodeByID 等
}

[Serializable]
public class RoomNode
{
    public string roomID;               // 用于唯一标识节点
    public GameRoomType roomType;           // 房间类型
    public List<string> connectionRoom;  // 与其他房间的连接

    public RoomNode(GameRoomType type, string id)
    {
        roomType = type;
        roomID = id;
        connectionRoom = new List<string>();
    }
}

public enum GameRoomType
{
    Entrance,
    Normal,
    Exit,
    Treasure,
    Shop,
    Teleport
}
