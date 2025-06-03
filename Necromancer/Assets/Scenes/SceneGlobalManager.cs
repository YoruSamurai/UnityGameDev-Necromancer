using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGlobalManager : SingletonManagerBase<SceneGlobalManager>
{
    protected override void Awake()
    {
        base.Awake(); // 必须保留：处理单例与DDOL
        SceneManager.sceneLoaded += OnSceneLoaded;
        // 主动触发一次当前场景初始化（避免第一次不触发）
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SceneInitializer] Scene Loaded: {scene.name}");
        // 示例：初始化 LevelManager
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.InitLevel(scene.buildIndex);
        }

        // 你也可以根据场景做其他逻辑，比如：
        // if (scene.name == "BattleScene") {...}
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
