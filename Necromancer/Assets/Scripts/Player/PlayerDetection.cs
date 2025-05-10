using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{

    [Header("检测范围")]
    public float detectionRadius = 3f; // 检测半径
    public LayerMask detectionLayer; // 用于指定需要检测的物体层

    [SerializeField] private Pickable closestPickable; // 存储离玩家最近的可拾取物品

    private void Update()
    {
        DetectObjects();
    }

    private void DetectObjects()
    {
        // 重置最近的可拾取物品
        closestPickable = null;
        float closestDistance = Mathf.Infinity;

        // 获取范围内的所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(detectionRadius, detectionRadius), detectionLayer);

        foreach (var collider in colliders)
        {
            // 检测可拾取物品
            Pickable pickable = collider.GetComponent<Pickable>();
            if (pickable != null)
            {
                float distance = Vector2.Distance(transform.position, pickable.transform.position);
                // 检查是否是最近的物品
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPickable = pickable;
                }
            }
        }

        // 处理最近的可拾取物品
        //HandleDetectedPickable();
    }

    private void HandleDetectedPickable()
    {
        if (closestPickable != null)
        {
            Debug.Log($"最近的可拾取物品: {closestPickable.name}");
            // 这里可以调用最近物品的拾取方法
            closestPickable.Pickup();
        }
    }

    public void PickUp()
    {
        Debug.Log("PICK");
        if (closestPickable != null)
        {
            Debug.Log($"最近的可拾取物品: {closestPickable.name}");
            // 这里可以调用最近物品的拾取方法
            closestPickable.Pickup();
        }
    }



}
