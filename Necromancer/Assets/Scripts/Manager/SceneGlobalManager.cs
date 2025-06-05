using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGlobalManager : SingletonManagerBase<SceneGlobalManager>
{

    [SerializeField] private LoadingCanvas loadingCanvas;
    public int loadProgress;

    protected override void Awake()
    {
        base.Awake(); // 必须保留：处理单例与DDOL
        ManualInitScene(SceneManager.GetActiveScene());
        loadingCanvas = GetComponentInChildren<LoadingCanvas>();
        loadingCanvas.gameObject.SetActive(false);
    }




    public void ChangeSceneToIndexAsync(string name)
    {
        StartCoroutine(LoadSceneAsync(true, -1, name));

    }

    public void ChangeSceneToIndexAsync(int index)
    {
        StartCoroutine(LoadSceneAsync(false, index, null));
    }

    public void LoadSceneWithGameData(GameData gameData)
    {
        StartCoroutine(LoadSceneAsync(gameData));
    }

    IEnumerator LoadSceneAsync(GameData gameData)
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


    IEnumerator LoadSceneAsync(bool viaString, int index ,string sceneString)
    {
        LevelManager.Instance.levelIsLoaded = false;
        loadProgress = 0;
        loadingCanvas.gameObject.SetActive(true);
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

    private void ManualInitScene(Scene scene)
    {
        int index = scene.buildIndex;
        Debug.Log($"[SceneInitializer] Scene Loaded: {scene.name}");
        
        if(SaveManager.Instance != null)
        {
            SaveManager.Instance.TryLoadGameData(index);
        }
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.InitLevel(index);
        }

        
        // 你也可以根据场景做其他逻辑，比如：
        // if (scene.name == "BattleScene") {...}
    }

    private void ManualInitScene(Scene scene,GameData gameData)
    {
        int index = scene.buildIndex;
        Debug.Log($"[SceneInitializer] Scene Loaded: {scene.name}");

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.TryLoadGameData(index);
        }
        Debug.Log("加载数据完成");
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.InitLevel(index , gameData);
        }


        // 你也可以根据场景做其他逻辑，比如：
        // if (scene.name == "BattleScene") {...}
    }


    private void LoadMainScene(Scene scene, LoadSceneMode mode)
    {

    }

    private void Update()
    {
        //回到主菜单 このときはまず保存します 但是要看是不是主菜单 是就不管了
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if(SceneManager.GetActiveScene().buildIndex != 0)
            {
                SaveManager.Instance.SaveGameData();
                ChangeSceneToIndexAsync(0);
            }
            else
            {
                Debug.LogWarning("不要在主界面保存好吗");
            }
        }

    }


    #region 一些get方法
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name; 
    }


    #endregion

}
