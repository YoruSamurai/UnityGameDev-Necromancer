using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private IDataPersistence dataService = new JsonDataService();
    private GameData gameData = new GameData();
    private HashSet<ISaveable> saveables = new HashSet<ISaveable>();

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            SaveGame();
        if (Input.GetKeyDown(KeyCode.O))
            LoadGame();
    }

    public void Register(ISaveable saveable) => saveables.Add(saveable);
    public void Unregister(ISaveable saveable) => saveables.Remove(saveable);

    public void SaveGame()
    {
        foreach (var saveable in saveables)
        {
            saveable.SaveData(gameData);
        }

        dataService.SaveData("/gameData.json", gameData, false);
        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        try
        {
            gameData = dataService.LoadData<GameData>("/gameData.json", false);

            foreach (var saveable in saveables)
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

    public void NewGame()
    {
        gameData = new GameData();
        foreach (var saveable in saveables)
        {
            saveable.LoadData(gameData); // 用空数据初始化
        }
    }

    
}
