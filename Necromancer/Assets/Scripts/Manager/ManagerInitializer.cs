using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerInitializer : MonoBehaviour
{

    public static ManagerInitializer Instance { get; private set; }

    [SerializeField] private ProjectileManager projectileManagerPrefab;

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
        if (ProjectileManager.Instance == null && projectileManagerPrefab != null)
        {
            Instantiate(projectileManagerPrefab,transform);
        }

        // 你可以在这里继续添加更多 Manager 的初始化逻辑
    }

}
