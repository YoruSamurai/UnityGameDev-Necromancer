using LDtkUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPrefab : MonoBehaviour
{
    [SerializeField] private List<GameObject> lightList;

    private void Awake()
    {
        lightList = new List<GameObject>();
    }
    private void Start()
    {
        StartCoroutine(DelayedLateStart());
    }

    private IEnumerator DelayedLateStart()
    {
        yield return null; // 延迟一帧，等所有 Start() 执行完
        LateStart();
    }

    void LateStart()
    {
        LDtkComponentEntity entity = GetComponentInParent<LDtkComponentEntity>();
        LevelManager.Instance.AddLightPrefab(this, entity.Parent.Parent);

        foreach (Transform child in transform)
        {
            lightList.Add(child.gameObject);
        }
        DisableLight();
    }

    public void EnableLight()
    {
        foreach (var light in lightList)
        {
            light.SetActive(true);
        }
    }

    public void DisableLight()
    {
        foreach(var light in lightList)
        {
            light.SetActive(false);
        }
    }

}
