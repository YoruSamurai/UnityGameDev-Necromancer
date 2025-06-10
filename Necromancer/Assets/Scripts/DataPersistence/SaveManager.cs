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


    public IEnumerator SaveGameData(SaveAndLoadType slType)
    {
        if(saveableGamedata.Count == 0)
        {
            Debug.Log("还没有进入游戏 无法保存当前游戏数据");
            yield break;
        }
        if(slType == SaveAndLoadType.StartNewGame)
        {
            Debug.LogWarning("现在是开始游戏 不用保存鹅");
        }
        float startTime = Time.realtimeSinceStartup;
        foreach (var saveable in saveableGamedata)
        {
            saveable.SaveData(gameData,slType);
            yield return null;  // 分帧执行
        }

        dataService.SaveData("/gameData.json", gameData, false);
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log($"花费时间 {elapsedTime}s Data Saved to: {Application.persistentDataPath}/gameData.json");
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


    public void LoadSettingData()
    {
        try
        {
            settingData = dataService.LoadData<SettingData>("/GlobalSetting.json", false);
            foreach (var saveable in saveableSettingData)
            {
                saveable.LoadData(settingData);
            }
            Debug.Log("Game Setting Loaded");
        }
        catch
        {
            Debug.Log("No saved setting found, creating default setting");
        }
    }

    /// <summary>
    /// 虽然创建了gamedata 但似乎并没有赋值给玩家？ 
    /// </summary>
    public void NewGame()
    {
        gameData = ArchiveManager.Instance.CreateNewGameData();
        dataService.SaveData("/gameData.json", gameData, false);
        Debug.Log("New Game Saved, Load bar scene...");
        SceneGlobalManager.Instance.ChangeSceneToIndexAsync(1, SaveAndLoadType.StartNewGame);
    }

    public void LoadGame()
    {
        try
        {
            gameData = dataService.LoadData<GameData>("/gameData.json", false);
            Debug.Log("Game Loaded,Load scene...");
            SceneGlobalManager.Instance.LoadSceneWithGameData(gameData,SaveAndLoadType.LoadSceneWithGameData);

        }
        catch
        {
            Debug.Log("No save file found, missing something?");
        }

    }

    public IEnumerator LoadGameAsync(SaveAndLoadType slType)
    {
        gameData = dataService.LoadData<GameData>("/gameData.json", false);
        yield return null;
        foreach (var saveable in saveableGamedata)
        {
            saveable.LoadData(gameData,slType);
        }
        Debug.Log("Game Loaded");

    }

    public void TryLoadGameData(int index,SaveAndLoadType slType)
    {
        if(index > 0)
        {
            Debug.Log("尝试加载数据呢。");
            foreach (var saveable in saveableGamedata)
            {
                saveable.LoadData(gameData, slType); 
            }

        }
    }


}
