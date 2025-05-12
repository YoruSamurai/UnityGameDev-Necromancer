using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private 简易教学UI tutorialPanel;
    [SerializeField] private InventoryUI inventoryPanel;

    public bool isPaused {  get; private set; }
    
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleTutorialPanel();
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventoryPanel();
        }
    }

    public void ToggleTutorialPanel()
    {
        tutorialPanel.TogglePanel();
    }

    public void ToggleInventoryPanel()
    {
        inventoryPanel.TogglePanel();
    }

    /// <summary>
    /// 无参数刷新 只刷新
    /// </summary>
    public void RefreshInventoryPanel()
    {
        inventoryPanel.RefreshPanel();
    }

    /// <summary>
    /// 2参数刷新，交换两个在仓库的位置
    /// </summary>
    /// <param name="item1"></param>
    /// <param name="item2"></param>
    public void RefreshInventoryPanel(string item1,string item2)
    {
        inventoryPanel.RefreshPanel(item1,item2);
    }

    /// <summary>
    /// 1参数刷新，把这个index放到最后
    /// </summary>
    /// <param name="item1"></param>
    public void RefreshInventoryPanel(string item1)
    {
        inventoryPanel.RefreshPanel(item1);
    }

}
