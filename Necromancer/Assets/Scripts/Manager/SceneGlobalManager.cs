using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SaveAndLoadType
{
    LoadSceneWithGameData,
    SLinGameNormalProcess,
    DeadAndBackToBar,
    SaveAndBackToMenu,
    StartNewGame,
    etc,
}

public class SceneGlobalManager : SingletonManagerBase<SceneGlobalManager>
{

    [SerializeField] private LoadingCanvas loadingCanvas;
    public int loadProgress;

    protected override void Awake()
    {
        base.Awake(); // 必须保留：处理单例与DDOL
        //调试用
        Scene newScene = SceneManager.GetActiveScene();
        if(newScene.buildIndex != 0)
        {
            ManualInitScene(SceneManager.GetActiveScene());

        }
        loadingCanvas = GetComponentInChildren<LoadingCanvas>();
        loadingCanvas.gameObject.SetActive(false);
    }



    /// <summary>
    /// 通过场景名异步切换场景
    /// </summary>
    /// <param name="name"></param>
    public void ChangeSceneToIndexAsync(string name, SaveAndLoadType slType)
    {
        StartCoroutine(LoadSceneAsync(true, -1, name, slType));

    }

    /// <summary>
    /// 通过场景index切换
    /// </summary>
    /// <param name="index"></param>
    public void ChangeSceneToIndexAsync(int index, SaveAndLoadType slType)
    {
        StartCoroutine(LoadSceneAsync(false, index, null, slType));
    }

    /// <summary>
    /// 读取游戏数据 进入场景时加载，在载入游戏的时候使用
    /// </summary>
    /// <param name="gameData"></param>
    public void LoadSceneWithGameData(GameData gameData, SaveAndLoadType slType)
    {
        StartCoroutine(LoadSceneAsync(gameData,slType));
    }

    /// <summary>
    /// 加载场景实际函数（通过gameData）
    /// </summary>
    /// <param name="gameData"></param>
    /// <returns></returns>
    IEnumerator LoadSceneAsync(GameData gameData, SaveAndLoadType slType)
    {
        LevelManager.Instance.levelIsLoaded = false;
        loadProgress = 0;
        loadingCanvas.gameObject.SetActive(true);
        string sceneName = gameData.sceneData.currentSceneName;
        if (string.IsNullOrEmpty(sceneName) || sceneName == "null")
        {
            Debug.LogWarning("没有对应的场景名");
            yield break;
        }
        else if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning("场景未添加到 Build Settings 或名称错误: " + sceneName);
            yield break;
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        Time.timeScale = 0f;
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            // 可选：更新进度条 UI
            loadProgress = 10;
            loadingCanvas.SetLoadingProgress(loadProgress);
            float progressValue = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            if (progressValue >= 1)
                break;
            yield return new WaitForSecondsRealtime(.2f); // 加这句非常关键！！否则主线程会死锁
        }

        // 允许激活场景
        asyncLoad.allowSceneActivation = true;

        // 等待场景激活完成
        while (!asyncLoad.isDone)
        {
            yield return null; // 等待激活
            Debug.Log("等待中");
        }

        // 确保下一帧场景完全初始化完成
        yield return null;
        Debug.Log("等待完成");
        // 获取当前场景
        Scene newScene = SceneManager.GetActiveScene();
        ManualInitScene(newScene,gameData);

        while (loadProgress != -1)
        {
            Debug.Log($"[LoadSceneAsync] 初始化进度阶段: {loadProgress}");

            loadingCanvas.SetLoadingProgress(loadProgress);
            yield return new WaitForSecondsRealtime(.2f); // 加这句非常关键！！否则主线程会死锁
        }


        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(.2f);
        loadingCanvas.gameObject.SetActive(false);
        Debug.Log("[LoadSceneAsync] 一切完成，进入场景");
    }

    /// <summary>
    /// 加载场景实际函数（不通过gameData）
    /// </summary>
    /// <param name="viaString"></param>
    /// <param name="index"></param>
    /// <param name="sceneString"></param>
    /// <returns></returns>
    IEnumerator LoadSceneAsync(bool viaString, int index ,string sceneString, SaveAndLoadType slType)
    {
        LevelManager.Instance.levelIsLoaded = false;
        loadProgress = 100;
        loadingCanvas.gameObject.SetActive(true);
        yield return StartCoroutine(PreHandleLoadScene(slType));
        AsyncOperation asyncLoad = new AsyncOperation();
        if (viaString)
        {
            asyncLoad = SceneManager.LoadSceneAsync(sceneString);
        }
        else
        {
            asyncLoad = SceneManager.LoadSceneAsync(index);
        }
        Time.timeScale = 0f;
        asyncLoad.allowSceneActivation = false;

        // 可选：等待加载进度达到90%（Unity特性：达到90%后等待激活）
        while (!asyncLoad.isDone)
        {
            // 可选：更新进度条 UI
            loadProgress = 10;
            loadingCanvas.SetLoadingProgress(loadProgress);
            float progressValue = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            if (progressValue >= 1)
                break;
            yield return new WaitForSecondsRealtime(.2f); // 加这句非常关键！！否则主线程会死锁
        }

        // 允许激活场景
        asyncLoad.allowSceneActivation = true;

        // 等待场景激活完成
        while (!asyncLoad.isDone)
        {
            yield return null; // 等待激活
            Debug.Log("等待中");
        }

        // 确保下一帧场景完全初始化完成
        yield return null;
        Debug.Log("等待完成");



        yield return StartCoroutine(PostHandleLoadScene(slType));
        
        
        
        // 获取当前场景
        Scene newScene = SceneManager.GetActiveScene();
        ManualInitScene(newScene);

        while (loadProgress != -1)
        {
            Debug.Log($"[LoadSceneAsync] 初始化进度阶段: {loadProgress}");

            loadingCanvas.SetLoadingProgress(loadProgress);
            yield return new WaitForSecondsRealtime(.2f); // 加这句非常关键！！否则主线程会死锁
        }

        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(.2f);
        loadingCanvas.gameObject.SetActive(false);
        Debug.Log("[LoadSceneAsync] 一切完成，进入场景");
    }

    /// <summary>
    /// 在加载前的预处理 保存
    /// </summary>
    /// <returns></returns>
    IEnumerator PreHandleLoadScene(SaveAndLoadType slType)
    {
        yield return StartCoroutine(SaveManager.Instance.SaveGameData(slType));
    }

    /// <summary>
    /// 加载场景的后处理 大概率是载入数据
    /// </summary>
    /// <param name="slType"></param>
    /// <returns></returns>
    IEnumerator PostHandleLoadScene(SaveAndLoadType slType)
    {
        yield return StartCoroutine(SaveManager.Instance.LoadGameAsync(slType));
    }

    /// <summary>
    /// 加载进场景 开始做初始化工作 
    /// </summary>
    /// <param name="scene"></param>
    private void ManualInitScene(Scene scene)
    {
        int index = scene.buildIndex;
        Debug.Log($"[SceneInitializer] Scene Loaded: {scene.name}");

        if(InventoryManager.Instance != null)
        {
            InventoryManager.Instance.InitInventory(index);
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.InitLevel(index);
        }

        
        // 你也可以根据场景做其他逻辑，比如：
        // if (scene.name == "BattleScene") {...}
    }

    /// <summary>
    /// 加载进场景 开始做初始化工作（通过gamedata）
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="gameData"></param>
    private void ManualInitScene(Scene scene,GameData gameData)
    {
        int index = scene.buildIndex;
        Debug.Log($"[SceneInitializer] Scene Loaded: {scene.name}");

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.TryLoadGameData(index,SaveAndLoadType.LoadSceneWithGameData);
        }
        Debug.Log("加载数据完成");

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.InitInventory(index);
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.InitLevel(index , gameData);
        }


        // 你也可以根据场景做其他逻辑，比如：
        // if (scene.name == "BattleScene") {...}
    }


    private void Update()
    {
        //回到主菜单 このときはまず保存します 但是要看是不是主菜单 是就不管了
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if(SceneManager.GetActiveScene().buildIndex != 0)
            {
                ChangeSceneToIndexAsync(0,SaveAndLoadType.SaveAndBackToMenu);
            }
            else
            {
                Debug.LogWarning("不要在主界面保存好吗");
            }
        }
        //回到酒吧 このときはまず保存します 但是要看是不是主菜单 是就不管了
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                ChangeSceneToIndexAsync(1, SaveAndLoadType.DeadAndBackToBar);
            }
            else
            {
                Debug.LogWarning("不要在主界面做这种事情好吗");
            }
        }

    }


    #region 一些get方法
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name; 
    }

    public int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }


    #endregion

}
