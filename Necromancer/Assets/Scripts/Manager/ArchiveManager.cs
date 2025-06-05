using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchiveManager : SingletonManagerBase<ArchiveManager>, ISaveableGameData
{

    [SerializeField] private LdtkLevelSoList soList;

    public string SaveID => "Archive"; // 唯一标识符
    public void SaveData(GameData data)
    {
        data.sceneData = new SceneData
        {
            currentSceneName = SceneGlobalManager.Instance.GetCurrentSceneName(),
            ldtkLevelListName = LevelManager.Instance.GetLdtkLevelSoList().name,
            roomDatas = ConvertToSerializableRoomDatas(LevelManager.Instance.roomDatas),
            roomMonsters = ConvertToSerializableMonsterData(LevelManager.Instance.levelMonsterDatas),
        };
    }

    public void LoadData(GameData data)
    {
        if(data.sceneData == null)
        {
            return;
        }
        LevelManager.Instance.roomDatas = ConvertToActualRoomDatas(data.sceneData.roomDatas);
        LevelManager.Instance.levelMonsterDatas = ConvertToActualMonsterDatas(data.sceneData.roomMonsters);
    }

    private GameObject GetMonsterByName(string name)
    {
        foreach (var so in LevelManager.Instance.levelMonsterListSO.levelMonsterList)
        {
            if (so.name == name) return so;
        }
        return null;
    }

    public List<LevelMonsterData> ConvertToActualMonsterDatas(List<SerializableMonsterData> serializableData)
    {
        var result = new List<LevelMonsterData>();
        foreach (var data in serializableData)
        {
            if(data.monster != "Dead")
            {
                GameObject monsterGO = GetMonsterByName(data.monster); // 你需要提供这个方法
                var monsterData = new LevelMonsterData(
                    data.monsterIndex,
                    monsterGO,
                    data.roomIndex,
                    data.roomSpawnPointIndex,
                    data.isDead,
                    data.xPosition,
                    data.yPosition
                );
                result.Add(monsterData);
            }
        }
        return result;
    }
    private List<SerializableMonsterData> ConvertToSerializableMonsterData(List<LevelMonsterData> actualData)
    {
        var result = new List<SerializableMonsterData>();
        foreach (var data in actualData)
        {
            result.Add(new SerializableMonsterData
            {
                monsterIndex = data.monsterIndex,
                monster = data.monster != null ? data.monster.name : "Dead",
                roomIndex = data.roomIndex,
                roomSpawnPointIndex = data.roomSpawnPointIndex,
                isDead = data.isDead,
                xPosition = data.xPosition,
                yPosition = data.yPosition
            });
        }
        return result;
    }

    public List<ActualRoomData> ConvertToActualRoomDatas(List<SerializableRoomData> serializableData)
    {
        var result = new List<ActualRoomData>();
        foreach (var data in serializableData)
        {
            result.Add(new ActualRoomData
            {
                roomID = data.roomID,
                room = GetLevelSoByName(data.roomName), // 你需要实现这个查找逻辑
                startPosition = data.startPosition.ToVector2(),
                gameRoomType = data.gameRoomType,
                levelWidth = data.levelWidth,
                levelHeight = data.levelHeight,
                //doorInfos = data.doorInfos,
                connectionRoom = data.connectionRoom,
                lightPrefabs = new List<LightPrefab>(),
                // lightPrefabs 和 levelData 留空或重新构造
            });
        }
        return result;
    }
    private List<SerializableRoomData> ConvertToSerializableRoomDatas(List<ActualRoomData> actualData)
    {
        var result = new List<SerializableRoomData>();
        foreach (var room in actualData)
        {
            result.Add(new SerializableRoomData
            {
                roomID = room.roomID,
                roomName = room.room != null ? room.room.name : "UnknownRoom",
                startPosition = new SerializableVector2(room.startPosition),
                gameRoomType = room.gameRoomType,
                levelWidth = room.levelWidth,
                levelHeight = room.levelHeight,
                //doorInfos = room.doorInfos,
                connectionRoom = room.connectionRoom,
            });
        }
        return result;
    }

    private LdtkLevelSO GetLevelSoByName(string name)
    {
        foreach(var so in soList.ldtkLevelSoList)
        {
            if (so.name == name) return so;
        }
        return null;
    }


    protected override void Awake()
    {
        base.Awake(); // 必须保留：处理单例与DDOL
        SaveManager.Instance.RegisterGameData(this);
    }

    private void OnDestroy()
    {
        SaveManager.Instance.UnregisterGameData(this);
    }

    public GameData CreateNewGameData()
    {
        GameData gameData = new GameData();
        gameData.playerData = new PlayerData();

        return gameData;
    }

}


