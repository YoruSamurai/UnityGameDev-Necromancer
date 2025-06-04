using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : SingletonManagerBase<SaveManager>
{

    [SerializeField] private IDataPersistence dataService = new JsonDataService();

    private GameData gameData = new GameData();
    private HashSet<ISaveableGameData> saveableGamedata = new HashSet<ISaveableGameData>();

    private SettingData settingData = new SettingData();
    private HashSet<ISaveableSettingData> saveableSettingData = new HashSet<ISaveableSettingData>();


    protected override void Awake()
    {
        base.Awake(); // 必须保留：处理单例与DDOL
    }

    private void Start()
    {
        //LoadSettingData();
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.I))
            SaveGameData();*/
        /*if (Input.GetKeyDown(KeyCode.O))
            LoadGameData();*/
    }

    public void RegisterGameData(ISaveableGameData saveable) => saveableGamedata.Add(saveable);
    public void UnregisterGameData(ISaveableGameData saveable) => saveableGamedata.Remove(saveable);

    public void RegisterSettingData(ISaveableSettingData saveable) => saveableSettingData.Add(saveable);
    public void UnregisterSettingData(ISaveableSettingData saveable) => saveableSettingData.Remove(saveable);

    public void SaveGameData()
    {
        if(saveableGamedata.Count == 0)
        {
            Debug.Log("还没有进入游戏 无法保存当前游戏数据");
            return;
        }

        foreach (var saveable in saveableGamedata)
        {
            saveable.SaveData(gameData);
        }

        dataService.SaveData("/gameData.json", gameData, false);
        Debug.Log("Game Saved");
    }

    public void SaveSettingData()
    {
        float startTime = Time.realtimeSinceStartup;
        if (saveableSettingData.Count == 0)
        {
            Debug.Log("没有找到被注册的全局设置 奇怪");
            return;
        }

        foreach (var saveable in saveableSettingData)
        {
            saveable.SaveData(settingData);
        }

        dataService.SaveData("/GlobalSetting.json", settingData, false);
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log($"花费时间 {elapsedTime}s Global Setting Saved to: {Application.persistentDataPath}/GlobalSetting.json");

    }

    public void LoadGameData()
    {
        try
        {
            gameData = dataService.LoadData<GameData>("/gameData.json", false);

            foreach (var saveable in saveableGamedata)
            {
                saveable.LoadData(gameData);
            }
            Debug.Log("Game Loaded");
        }
        catch
        {
            Debug.Log("No save file found, missing something?");
        }
    }

    public void LoadSettingData()
    {
        try
        {
            settingData = dataService.LoadData<SettingData>("/GlobalSetting.json", false);
            foreach (var saveable in saveableSettingData)
            {
                saveable.LoadData(settingData);
            }
            Debug.Log("Game Loaded");
        }
        catch
        {
            Debug.Log("No saved setting found, creating default setting");
        }
    }

    public void NewGame()
    {
        gameData = ArchiveManager.Instance.CreateNewGameData();
        dataService.SaveData("/gameData.json", gameData, false);
        Debug.Log("New Game Saved, Load bar scene...");
        SceneGlobalManager.Instance.ChangeSceneToIndex(1);
    }

    public void LoadGame()
    {
        try
        {
            gameData = dataService.LoadData<GameData>("/gameData.json", false);
            Debug.Log("Game Loaded,Load bar scene...");
            SceneGlobalManager.Instance.ChangeSceneToIndex(1);

        }
        catch
        {
            Debug.Log("No save file found, missing something?");
        }

    }

    public void TryLoadGameData(int index)
    {
        if(index > 0)
        {
            Debug.Log("尝试加载数据呢。");
            foreach (var saveable in saveableGamedata)
            {
                saveable.LoadData(gameData); // 用空数据初始化
            }

        }
    }


}
