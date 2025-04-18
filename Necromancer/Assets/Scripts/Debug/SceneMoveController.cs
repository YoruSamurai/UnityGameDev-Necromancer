using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMoveController : MonoBehaviour
{
    [Header("在这个脚本中实现一个简单可以调整速度的可以通过WSAD移动搭载了这个脚本的物体的功能")]
    [Header("移动速度")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("速度调整步进值")]
    [SerializeField] private float speedStep = 1f;

    [Header("最小和最大速度")]
    [SerializeField] private float minSpeed = 5f;
    [SerializeField] private float maxSpeed = 10f;

    private void Update()
    {
        HandleMove();
        HandleSpeedChange();
    }

    private void HandleMove()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or 左/右
        float vertical = Input.GetAxisRaw("Vertical");     // W/S or 上/下

        Vector3 direction = new Vector3(horizontal, vertical, 0f).normalized;

        if (direction.magnitude > 0.1f)
        {
            transform.Translate(direction * moveSpeed * Time.deltaTime * 5, Space.World);
        }
    }

    private void HandleSpeedChange()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveSpeed += speedStep;
            moveSpeed = Mathf.Clamp(moveSpeed, minSpeed, maxSpeed);
            Debug.Log("当前速度: " + moveSpeed);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveSpeed -= speedStep;
            moveSpeed = Mathf.Clamp(moveSpeed, minSpeed, maxSpeed);
            Debug.Log("当前速度: " + moveSpeed);
        }
    }
}
