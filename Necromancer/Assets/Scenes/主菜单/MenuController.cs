using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class MenuController : MonoBehaviour
{
    [Header("主菜单")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionMenuPanel;

    [Header("弹出对话")]
    [SerializeField] private GameObject newGamePanelDialog;
    [SerializeField] private GameObject loadGamePanelDialog;
    [SerializeField] private GameObject noSavedGameDialog;

    [Header("设置面板")]
    [SerializeField] private GameObject graphicsSetting;
    [SerializeField] private GameObject volumeSetting;
    [SerializeField] private GameObject gameplaySetting;

    [Header("图形面板")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Text brightnessTextValue = null;
    [SerializeField] private Toggle isFullScreenToggle = null;
    [SerializeField] private float defaultBrightness = 1;

    [Header("图形面板-分辨率")]
    public TMP_Dropdown resolutionDropdown;
    private List<Resolution> resolutions = new List<Resolution>();

    private bool _isFullScreen;
    private int _resolutionX;
    private int _resolutionY;
    private float _brightnessLevel;

    [Header("音量设置")]
    [SerializeField] private GameObject volumePanel;
    [SerializeField] private TMP_Text volumeValue;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private float defaultVolume = .5f;

    public void Start()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (i == 0)
                resolutions.Add(Screen.resolutions[i]);

            if (i > 0 && (Screen.resolutions[i].width != Screen.resolutions[i - 1].width || Screen.resolutions[i].height != Screen.resolutions[i - 1].height))
                resolutions.Add(Screen.resolutions[i]);
        }
        resolutionDropdown.ClearOptions();
        ResetButton("Audio");

        List<string> options = new List<string>();

        int currentResolutionIndex = 0; 

        for(int i = 0; i < resolutions.Count; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    

    public void OnclickGraphicsSettingBtn()
    {
        _isFullScreen = GlobalSettingManager.Instance.isFullScreen;
        _resolutionX = GlobalSettingManager.Instance.resolutionX;
        _resolutionY = GlobalSettingManager.Instance.resolutionY;
        _brightnessLevel = GlobalSettingManager.Instance.brightnessLevel;

        brightnessSlider.value = _brightnessLevel;
        brightnessTextValue.text = _brightnessLevel.ToString("0.0");
        isFullScreenToggle.isOn = _isFullScreen;

        for (int i = 0; i < resolutionDropdown.options.Count; i++)
        {
            string reso = $"{_resolutionX} x {_resolutionY}";
            if(reso == resolutionDropdown.options[i].text)
            {
                resolutionDropdown.value = i;
                break;
            }
        }

        optionMenuPanel.SetActive(false);
        graphicsSetting.SetActive(true);
    }


    public void OnclickVolumeSettingBtn()
    {
        volumeSlider.value = GlobalSettingManager.Instance.globalVolume;
        volumeValue.text = (GlobalSettingManager.Instance.globalVolume * 100).ToString("0");
        optionMenuPanel.SetActive(false);
        volumeSetting.SetActive(true);
    }

    #region 按钮开关 没什么用

    public void GraphicsSettingBackBtn()
    {
        graphicsSetting.SetActive(false);
        optionMenuPanel.SetActive(true);
    }
    public void VolumeSettingBackBtn()
    {
        volumeSetting.SetActive(false);
        optionMenuPanel.SetActive(true);
    }

    public void OnclickNewGameBtn()
    {
        mainMenuPanel.SetActive(false);
        newGamePanelDialog.SetActive(true);
    }

    public void OnclickLoadGameBtn()
    {
        mainMenuPanel.SetActive(false);
        loadGamePanelDialog.SetActive(true);
    }

    public void OnclickOptionsBtn()
    {
        mainMenuPanel.SetActive(false);
        optionMenuPanel.SetActive(true);
    }

    public void OptionsReturnBtn()
    {
        optionMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void NewGameDiaLogBtnYes()
    {
        Debug.Log("本来要开始新游戏的 但是先回去吧");
        newGamePanelDialog.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    public void NewGameDiaLogBtnNo()
    {
        Debug.Log("我补药开始新游戏");
        newGamePanelDialog.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void LoadGameDiaLogBtnYes()
    {
        Debug.Log("本来要载入游戏的 但是先回去吧");
        loadGamePanelDialog.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    public void LoadGameDiaLogBtnNo()
    {
        Debug.Log("我补药载入游戏");
        loadGamePanelDialog.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    #endregion

    #region 图形
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        _resolutionX = resolution.width;
        _resolutionY = resolution.height;
    }

    public void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
        brightnessTextValue.text = brightness.ToString("0.0");
    }

    public void SetFullScreen(bool isFullScreen)
    {
        _isFullScreen = isFullScreen;

    }

    public void GraphicsApply()
    {
        Screen.fullScreen = _isFullScreen;
        GlobalSettingManager.Instance.isFullScreen = _isFullScreen;
        GlobalSettingManager.Instance.brightnessLevel = _brightnessLevel;
        GlobalSettingManager.Instance.resolutionX = _resolutionX;
        GlobalSettingManager.Instance.resolutionY = _resolutionY;
        SaveManager.Instance.SaveSettingData();
    }

    #endregion

    #region 音量设置
    public void SetVolume(float volume)
    {
        // 四舍五入到最近的 0.01
        volume = Mathf.Round(volume * 100) / 100;
        AudioListener.volume = volume;
        volumeValue.text = (volume*100).ToString("0");

    }


    public void VolumeApply()
    {
        Debug.Log("我将保存音量为" + AudioListener.volume);
        GlobalSettingManager.Instance.globalVolume = AudioListener.volume;
        SaveManager.Instance.SaveSettingData();
    }

    #endregion

    public void ResetButton(string MenuType)
    {
        if(MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeValue.text = (defaultVolume * 100).ToString("0");
        }
        else if(MenuType == "Graphics")
        {
            _isFullScreen = false;
            _brightnessLevel = 1;
            _resolutionX = 1920;
            _resolutionY = 1080;
            brightnessSlider.value = _brightnessLevel;
            brightnessTextValue.text = _brightnessLevel.ToString("0.0");
            isFullScreenToggle.isOn = _isFullScreen;
            for (int i = 0; i < resolutionDropdown.options.Count; i++)
            {
                string reso = $"{_resolutionX} x {_resolutionY}";
                if (reso == resolutionDropdown.options[i].text)
                {
                    resolutionDropdown.value = i;
                    break;
                }
            }
        }
    }


    public void ExitButton()
    {
        Application.Quit();
    }

}
