using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private IDataPersistence dataService = new JsonDataService();

    private GameData gameData = new GameData();
    private HashSet<ISaveableGameData> saveableGamedata = new HashSet<ISaveableGameData>();

    private SettingData settingData = new SettingData();
    private HashSet<ISaveableSettingData> saveableSettingData = new HashSet<ISaveableSettingData>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //LoadSettingData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            SaveGameData();
        if (Input.GetKeyDown(KeyCode.O))
            LoadGameData();
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
            Debug.Log("No save file found, creating new game");
            NewGame();
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
        gameData = new GameData();
        foreach (var saveable in saveableGamedata)
        {
            saveable.LoadData(gameData); // 用空数据初始化
        }
    }

    
}
