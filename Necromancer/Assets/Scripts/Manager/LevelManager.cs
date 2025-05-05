using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yoru;
using static RoomGraphGenerator;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    private bool isInBattleLevel = true;

    private RoomGraphGenerator roomGraph;

    public List<ActualRoomData> roomDatas;

    public ActualRoomData playerCurrentRoom;

    public int killStreak = 0;

    public float levelTimer = 0f; //  加入计时器变量

    private void Awake()
    {
        // 确保实例唯一
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 防止多个实例
        }
    }


    private void Start()
    {
        roomGraph = FindObjectOfType<RoomGraphGenerator>();

        if (roomGraph == null)
        {
            Debug.LogError("找不到 RoomGraphGenerator 脚本！");
        }
        else
        {
            Debug.Log("成功获取 RoomGraphGenerator：" + roomGraph.name);
            roomDatas = roomGraph.roomDatas;
        }
        EventManager.Instance.AddListener(EventName.NormalEnemyDead, KillEnemy);
    }



    private void FixedUpdate()
    {
        int roomIndex = GetCurrentRoomMessage(PlayerStats.Instance.player.GetCurrentPosition(), roomDatas);
        playerCurrentRoom = roomDatas[roomIndex];

    }



    private void Update()
    {
        if (playerCurrentRoom != null)
        {
            if (CanCountTime())
            {
                levelTimer += Time.deltaTime;   

            }
        }
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener(EventName.NormalEnemyDead, KillEnemy);

    }

    private void KillEnemy(object sender, EventArgs e)
    {
        Debug.Log("敌人死亡！");
        var data = e as NormalEnemyDeadEventArgs;
        if (data != null)
        {
            Debug.Log(data.enemyName);
        }
        killStreak += 1;
    }
    public int GetCurrentRoomMessage(Vector2 playerPosition, List<ActualRoomData> roomDatas)
    {
        ActualRoomData roomData = new ActualRoomData();
        for (int i = 0; i < roomDatas.Count; i++)
        {
            if (playerPosition.x >= roomDatas[i].startPosition.x &&
               playerPosition.x <= roomDatas[i].startPosition.x + roomDatas[i].levelWidth &&
               playerPosition.y >= roomDatas[i].startPosition.y &&
               playerPosition.y <= roomDatas[i].startPosition.y + roomDatas[i].levelHeight)
            {
                return i;
            }
        }
        return -1;
    }

    private bool CanCountTime()
    {
        if (playerCurrentRoom.gameRoomType != GameRoomType.Entrance &&
            playerCurrentRoom.gameRoomType != GameRoomType.Treasure)
        {
            return true;
        }
        return false;
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 500, 30), $"Level Time: {levelTimer:F1}s,当前连杀: {killStreak}");
    }

}
