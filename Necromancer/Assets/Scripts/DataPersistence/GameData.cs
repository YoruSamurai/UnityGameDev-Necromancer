using UnityEngine;
// 新建ISaveable接口

public interface ISaveableGameData
{
    string SaveID { get; } // 每个可保存对象的唯一标识
    void SaveData(GameData data); // 保存数据到GameData结构
    void LoadData(GameData data); // 从GameData结构加载数据
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

[System.Serializable]
public class PlayerData
{
    public int currentHealth;
    public int maxHealth;
    public int soul;
    public int gold;
    public SerializableVector2 position; // 使用 SerializableVector2 保存位置
    // 添加其他需要保存的玩家属性
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