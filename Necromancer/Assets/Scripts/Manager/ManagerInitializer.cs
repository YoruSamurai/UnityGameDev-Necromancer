using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerInitializer : MonoBehaviour
{

    public static ManagerInitializer Instance { get; private set; }

    [SerializeField] private ProjectileManager projectileManagerPrefab;
    [SerializeField] private SaveManager saveManagerPrefab;
    [SerializeField] private SoundManager soundManagerPrefab;
    [SerializeField] private PlayFxManager playFxManagerPrefab;
    [SerializeField] private DialogueManager dialogueManagerPrefab;
    [SerializeField] private InventoryManager inventoryManagerPrefab;
    [SerializeField] private LevelManager levelManagerPrefab;
    [SerializeField] private EventManager eventManagerPrefab;
    [SerializeField] private ObjectPoolManager objectPoolManagerPrefab;
    [SerializeField] private BattleManagerTest battleManagerTestPrefab;
    [SerializeField] private SceneGlobalManager sceneGlobalManagerPrefab;
    
    private void Awake()
    {
        // 单例检查
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeManagers();
    }

    private void InitializeManagers()
    {
        float start = Time.realtimeSinceStartup;
        if (ProjectileManager.Instance == null && projectileManagerPrefab != null)
        {
            Instantiate(projectileManagerPrefab);
            Instantiate(saveManagerPrefab);
            Instantiate(soundManagerPrefab);
            Instantiate(playFxManagerPrefab);
            Instantiate(dialogueManagerPrefab);
            Instantiate(inventoryManagerPrefab);
            Instantiate(levelManagerPrefab);
            Instantiate(eventManagerPrefab);
            Instantiate(objectPoolManagerPrefab);
            Instantiate(battleManagerTestPrefab);
            Instantiate(sceneGlobalManagerPrefab);
        }

        
        float duration = Time.realtimeSinceStartup - start;
        Debug.Log($"[ProjectileManager] 初始化耗时: {duration * 1000f} ms");

        // 你可以在这里继续添加更多 Manager 的初始化逻辑
    }

}
