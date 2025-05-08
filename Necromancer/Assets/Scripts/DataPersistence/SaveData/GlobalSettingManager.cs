using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GlobalSettingManager : MonoBehaviour,ISaveableSettingData
{
    public static GlobalSettingManager Instance { get; private set; }

    /// <summary>
    /// 语言设置
    /// </summary>
    public LanguageEnum globalLanguage;
    private Dictionary<string, Locale> languageDic; //存储本地语言

    /// <summary>
    /// 图形设置
    /// </summary>
    public bool isFullScreen;
    public int resolutionX;
    public int resolutionY;
    public float brightnessLevel;

    /// <summary>
    /// 音量相关设置
    /// </summary>
    public float globalVolume { get; set; }
    public float musicVolume { get; set; }
    public float environmentVolume { get; set; }
    public float soundFxVolume { get; set; }

    public string SaveID => "GlobalSetting";
    public void SaveData(SettingData data)
    {
        data.volumeSettingData = new VolumeSettingData
        {
            masterVolume = globalVolume,
            musicVolume = musicVolume,
            environmentVolume = environmentVolume,
            soundFxVolume = soundFxVolume,
        };
        data.graphicsSettingData = new GraphicsSettingData
        {
            isFullScreen = isFullScreen,
            resolutionX = resolutionX,
            resolutionY = resolutionY,
            brightnessLevel = brightnessLevel,
        };
        data.languageSettingData = new LanguageSettingData
        {
            language = globalLanguage,
        };
    }

    public void LoadData(SettingData data)
    {
        if(data.volumeSettingData != null)
        {
            globalVolume = data.volumeSettingData.masterVolume;
            musicVolume = data.volumeSettingData.musicVolume;
            environmentVolume = data.volumeSettingData.environmentVolume;
            soundFxVolume = data.volumeSettingData.soundFxVolume;
        }
        if(data.graphicsSettingData != null)
        {
            isFullScreen = data.graphicsSettingData.isFullScreen;
            resolutionX = data.graphicsSettingData.resolutionX;
            resolutionY = data.graphicsSettingData.resolutionY;
            brightnessLevel = data.graphicsSettingData.brightnessLevel;
        }
        if(data.languageSettingData != null)
        {
            globalLanguage = data.languageSettingData.language;
        }
    }


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

    }

    private void Start()
    {
        SaveManager.Instance.RegisterSettingData(this);
        SaveManager.Instance.LoadSettingData(); // 从这里调用更安全

        languageDic = new Dictionary<string, Locale>();

        //async加载语言环境
        AsyncLoadStrings();

    }


    #region async加载语言内容

    private async void AsyncLoadStrings()
    {
        /* 1.确保 Localization 系统完全初始化
         * 它是整个本地化系统的核心初始化操作，加载基础配置（如 Locale 列表、默认语言环境、语言环境切换器等）。
         * 如果 Localization 系统尚未初始化完成，直接使用其他操作可能会导致不可预期的行为。
         */
        var initialization = LocalizationSettings.InitializationOperation;
        await initialization.Task;
        if (initialization.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Localization 系统初始化失败：\n" + initialization.OperationException?.Message);
            return;
        }

        // 2.确保选定的语言环境初始化 SelectedLocaleAsync 将确保语言环境设置已经初始化，并且已经选择了一个区域设置。
        var m_InitializeOperation = LocalizationSettings.SelectedLocaleAsync;
        await m_InitializeOperation.Task;
        if (initialization.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log("语言环境设置已经初始化失败：\n" + initialization.OperationException?.Message);
            return;
        }

        // 3.确保语言表的初始化 GetTableAsync用于异步返回请求的表
        var loadingTable = LocalizationSettings.StringDatabase.GetTableAsync("myTable");
        await loadingTable.Task;

        if (loadingTable.Status == AsyncOperationStatus.Succeeded)
        {
            GetAllLocals();//获取所有Locale (语言环境)
            if (languageDic != null && languageDic.Count > 0)
            {
                SetLanguage((int)globalLanguage);
            }
        }
        else
        {
            Debug.LogError("无法加载字符串表\n" + loadingTable.OperationException?.Message);
        }
    }

    #endregion

    //获取所有Locale (语言环境)
    private void GetAllLocals()
    {
        //获取语言表
        var locales = LocalizationSettings.AvailableLocales.Locales;

        //将所有的语言环境添加到字典
        for (int i = 0; i < locales.Count; ++i)
        {
            var locale = locales[i];
            //Debug.Log(locale.LocaleName);

            //添加到字典
            if (!languageDic.ContainsKey(locale.LocaleName))
                languageDic.Add(locale.LocaleName, locale);
        }

    }


    

    public void SetLanguage(int language)
    {
        if(language == 0)
        {
            LocalizationSettings.Instance.SetSelectedLocale(languageDic["Chinese (Simplified) (zh-Hans)"]); //名称加载
        }
        else if(language == 1)
        {
            LocalizationSettings.Instance.SetSelectedLocale(languageDic["Chinese (Traditional) (zh-Hant)"]); //名称加载
        }
        else if (language == 2)
        {
            LocalizationSettings.Instance.SetSelectedLocale(languageDic["English (en)"]); //名称加载
        }
        else if (language == 3)
        {
            LocalizationSettings.Instance.SetSelectedLocale(languageDic["Japanese (ja)"]); //名称加载
        }
    }
}

[Serializable]
public enum LanguageEnum
{
    SimplifiedChinese,
    TraditionalChinese,
    English,
    Japanese,
}