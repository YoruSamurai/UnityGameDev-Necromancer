using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 简易教学UI : MonoBehaviour
{
    
    public void TogglePanel()
    {
        if (gameObject.activeSelf)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }

    public void OpenPanel()
    {
        gameObject.SetActive(true);
        UIManager.Instance.PauseGame();
    }
    public void ClosePanel()
    {
        gameObject.SetActive(false);
        UIManager.Instance.UnpauseGame();
    }
}
