using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaygroundUI : MonoBehaviour
{
    private PlaygroundManager playgroundManager;

    [SerializeField] private Button OpenEquipmentPlaygroundUI;
    [SerializeField] private PlaygroundBaseSetupUI equipmentPlaygroundUI;


    private void Start()
    {
        playgroundManager = GetComponent<PlaygroundManager>();
        OpenEquipmentPlaygroundUI.onClick.AddListener(OnClickOpenEquipmentPlaygroundUI);
    }


    // 按钮被点击时调用的方法
    private void OnClickOpenEquipmentPlaygroundUI()
    {
        // 在这里执行你想做的事，比如打开某个UI界面
        playgroundManager.ClosePlayground();
        equipmentPlaygroundUI.OpenPanel();
    }

}
