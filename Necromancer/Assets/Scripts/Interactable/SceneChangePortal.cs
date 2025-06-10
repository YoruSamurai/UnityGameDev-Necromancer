using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangePortal : MonoBehaviour
{
    [SerializeField] private string tpSceneName;
    private bool canUsePortal;

    private void Start()
    {
        canUsePortal = false;
    }

    private void Update()
    {
        if (!canUsePortal)
            return;
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("即将传送");
            SceneGlobalManager.Instance.ChangeSceneToIndexAsync(tpSceneName,SaveAndLoadType.SLinGameNormalProcess);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("can use portal");
            canUsePortal = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("portal closed");
            canUsePortal = false;
        }
    }
}
