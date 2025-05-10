using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private 简易教学UI panelTutorial;

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
    }

    public void ToggleTutorialPanel()
    {
        panelTutorial.TogglePanel();
    }

}
