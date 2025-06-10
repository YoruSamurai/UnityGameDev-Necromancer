using System;
using System.Collections.Generic;
using UnityEngine;
// 新建ISaveable接口

public interface ISaveableGameData
{
    string SaveID { get; } // 每个可保存对象的唯一标识
    void SaveData(GameData data, SaveAndLoadType slType); // 保存数据到GameData结构
    void LoadData(GameData data, SaveAndLoadType slType); // 从GameData结构加载数据
}

public interface ISaveableSettingData
{
    string SaveID { get; }
    void SaveData(SettingData data);
    void LoadData(SettingData data);

}

// 新建GameData数据容器类
[System.Serializable]
public class GameData
{
    public PlayerData playerData;
    public SceneData sceneData;
    public GlobalData globalData;
    // 可以添加其他需要保存的数据类（如InventoryData、SettingsData等）
}

//
[System.Serializable]
public class SettingData
{
    public VolumeSettingData volumeSettingData;
    public GraphicsSettingData graphicsSettingData;
    public LanguageSettingData languageSettingData;
}

[System.Serializable]
public class VolumeSettingData
{
    public float masterVolume;
    public float musicVolume;
    public float environmentVolume;
    public float soundFxVolume;

}

public class GraphicsSettingData
{
    public bool isFullScreen;
    public int resolutionX;
    public int resolutionY;
    public float brightnessLevel;
}

public class LanguageSettingData
{
    public LanguageEnum language;
}

/// <summary>
/// 这个应该是全局信息 比如死了多少次 杀了多少人 游戏时长 解鎖信息 什么的
/// </summary>
public class GlobalData
{
    public float playtime;
    public int monsterKilled;

    public GlobalData()
    {
        playtime = 0;
        monsterKilled = 0;
    }
}

public class SceneData
{
    public string currentSceneName;
    public string ldtkLevelListName;
    public List<SerializableMonsterData> roomMonsters;
    public List<SerializableRoomData> roomDatas;

}


[System.Serializable]
public class PlayerData
{
    public int currentHealth;
    public int maxHealth;
    public float healthPercentage;
    public int strLevel;
    public float strPercentage;
    public int agileLevel;
    public float agilePercentage;
    public int magicLevel;
    public float magicPercentage;
    public int soul;
    public int gold;
    public SerializableVector2 position; // 使用 SerializableVector2 保存位置
    public List<SerializableEquipableItemData> serializableEquipmentData;
    // 添加其他需要保存的玩家属性

    public PlayerData()
    {
        currentHealth = 100;
        maxHealth = 100;
        healthPercentage = 100;
        strLevel = 1;
        strPercentage = 100;
        agileLevel = 1;
        agilePercentage = 100;
        magicLevel = 1;
        magicPercentage = 100;
        soul = 100;
        gold = 50000;
        position = new SerializableVector2(new Vector2(10f,5f));
        serializableEquipmentData = new List<SerializableEquipableItemData>();
        SerializableEquipableItemData data = new SerializableEquipableItemData
        {
            itemID = 1001,
            itemLevel = 3,
            slotPositionIndex = SlotPositionIndex.mainSlot,
            itemAffixs = new List<int>()
        };
        serializableEquipmentData.Add(data);
    }
}

[System.Serializable]
public class SerializableVector2
{
    public float x;
    public float y;

    public SerializableVector2(Vector2 vector)
    {
        x = vector.x;
        y = vector.y;
    }

    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }
}


[Serializable]
public class SerializableEquipableItemData
{
    public int itemID;
    public int itemLevel;
    public SlotPositionIndex slotPositionIndex;
    public List<int> itemAffixs;
}

public enum SlotPositionIndex
{
    mainSlot,
    subSlot,
    inventory,
    other,
}


[Serializable]
public class SerializableRoomData
{
    public int roomID;
    public string roomName; // 从 LdtkLevelSO 改为名字或GUID
    public SerializableVector2 startPosition;
    public GameRoomType gameRoomType;
    public float levelWidth;
    public float levelHeight;
    //public List<DoorInfo> doorInfos;
    public List<int> connectionRoom;
}

[Serializable]
public class SerializableMonsterData
{
    public int monsterIndex;
    public string monster;
    public int roomIndex;
    public int roomSpawnPointIndex;
    public bool isDead;
    public float xPosition;
    public float yPosition;
}