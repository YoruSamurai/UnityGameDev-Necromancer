using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [Range(0f, 5f)]
    public float timeScale = 1f; // Inspector 中可调节的时间倍率

    void Update()
    {
        Time.timeScale = timeScale;
    }
}
