using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettingManager : MonoBehaviour,ISaveableSettingData
{
    public static GlobalSettingManager Instance { get; private set; }

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

    public string SaveID => "GlobalSetting";
    public void SaveData(SettingData data)
    {
        data.volumeSettingData = new VolumeSettingData
        {
            volume = globalVolume
        };
        data.graphicsSettingData = new GraphicsSettingData
        {
            isFullScreen = isFullScreen,
            resolutionX = resolutionX,
            resolutionY = resolutionY,
            brightnessLevel = brightnessLevel,
        };
    }

    public void LoadData(SettingData data)
    {
        if(data.volumeSettingData != null)
        {
            globalVolume = data.volumeSettingData.volume;
        }
        if(data.graphicsSettingData != null)
        {
            isFullScreen = data.graphicsSettingData.isFullScreen;
            resolutionX = data.graphicsSettingData.resolutionX;
            resolutionY = data.graphicsSettingData.resolutionY;
            brightnessLevel = data.graphicsSettingData.brightnessLevel;
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
    }

}
