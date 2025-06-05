using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingProgress;
    private int count;

    public void SetLoadingProgress(float progress)
    {
        switch (progress)
        {
            case 10:
                loadingProgress.text = "加载场景中";
                break;
            case 1:
                loadingProgress.text = "生成场景中";
                break;
            case 2:
                loadingProgress.text = "生成其他物件中";
                break;
            case 3:
                loadingProgress.text = "生成小地图中";
                break;
            case 11:
                loadingProgress.text = "读取场景中";
                break;
            case 12:
                loadingProgress.text = "读取其他物件中";
                break;
        }
        count = (count + 1) % 4;
        for (int i = 0; i < count; i++)
        {
            loadingProgress.text += ".";
        }
    }
}
