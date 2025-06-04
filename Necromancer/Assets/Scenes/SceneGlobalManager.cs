using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGlobalManager : SingletonManagerBase<SceneGlobalManager>
{

    [SerializeField] private LoadingCanvas loadingCanvas;

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

    public void ChangeSceneToIndex(int index)
    {
        SceneManager.LoadScene(index);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int index = scene.buildIndex;
        Debug.Log($"[SceneInitializer] Scene Loaded: {scene.name}");
        // 示例：初始化 LevelManager
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.InitLevel(index);
        }
        if(SaveManager.Instance != null)
        {
            SaveManager.Instance.TryLoadGameData(index);
        }

        // 你也可以根据场景做其他逻辑，比如：
        // if (scene.name == "BattleScene") {...}
    }

    private void Update()
    {
        //回到主菜单 このときはまず保存します
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SaveManager.Instance.SaveGameData();
            ChangeSceneToIndex(0);
        }
    }
}
