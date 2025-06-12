using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩耍吧 测试吧，这个不是instance哦。
/// </summary>
public class PlaygroundManager : MonoBehaviour
{


    [SerializeField] private GameObject playGroundBtn;
    [SerializeField] private GameObject playgroundUI;

    public void OpenPlayground()
    {
        if (!playgroundUI.activeSelf)
        {
            playgroundUI.SetActive(true);
            playGroundBtn.SetActive(false);
        }
    }

    public void ClosePlayground()
    {
        if (playgroundUI.activeSelf)
        {
            playgroundUI.SetActive(false);
            playGroundBtn.SetActive(true);
        }
    }



}
