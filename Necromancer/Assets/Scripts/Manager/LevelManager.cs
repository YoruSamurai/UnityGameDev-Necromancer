using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yoru;
using static RoomGraphGenerator;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    private bool isInBattleLevel = true;

    private RoomGraphGenerator roomGraph;

    public List<ActualRoomData> roomDatas;

    //关卡中有多少怪物
    [SerializeField] private int levelMonsterNum;

    [SerializeField] private Transform monsterParentTransform;

    [SerializeField] private LevelMonsterListSO levelMonsterListSO;

    [SerializeField] private List<LevelMonsterData> levelMonsterDatas;

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
        EventManager.Instance.AddListener(EventName.OnEnemyDead, KillEnemy);
        InitialEnemy(levelMonsterListSO, 1);
    }

    /// <summary>
    /// 在这里我们初始化敌人，可能还需要做一些额外的操作 比如关卡的特殊效果什么的
    /// </summary>
    /// <param name="levelMonsterListSO"></param>
    /// <param name="difficulty"></param>
    private void InitialEnemy(LevelMonsterListSO levelMonsterListSO,int difficulty)
    {
        float startTime = Time.realtimeSinceStartup;

        
        levelMonsterDatas.Clear();
        int enemyIndex = 0;
        //遍历所有房间 给每个房间随机生成两个怪，

        int monsterNum = levelMonsterNum;
        int spawnNormalRoom = roomDatas.Count(roomData => roomData.gameRoomType == GameRoomType.Normal);

        if (roomDatas.Count > 0 && spawnNormalRoom > 0)
        {
            int baseMonstersPerRoom = monsterNum / spawnNormalRoom;
            int extraMonsters = monsterNum % spawnNormalRoom;

            System.Random rng = new System.Random();
            int normalRoomCounter = 0;

            foreach (var roomData in roomDatas)
            {
                if (roomData.gameRoomType != GameRoomType.Normal)
                    continue;

                List<MonsterSpawnPoint> spawnPoints = roomData.room.monsterSpawnPoints;

                // 打乱 spawnPoints（Fisher–Yates）
                for (int i = spawnPoints.Count - 1; i > 0; i--)
                {
                    int swapIndex = rng.Next(i + 1);
                    var temp = spawnPoints[i];
                    spawnPoints[i] = spawnPoints[swapIndex];
                    spawnPoints[swapIndex] = temp;
                }

                int monstersToSpawn = baseMonstersPerRoom;
                if (extraMonsters > 0 && UnityEngine.Random.Range(0,10) < 5)
                {
                    monstersToSpawn += 1;
                    extraMonsters--;
                }

                for (int i = 0; i < monstersToSpawn && i < spawnPoints.Count; i++)
                {
                    enemyIndex++;
                    Vector3 spawnPos = roomData.startPosition + spawnPoints[i].monsterSpawnPosition;
                    List<GameObject> canGenerateMonsterList = new List<GameObject>();
                    if (spawnPoints[i].isGroundMonster)
                    {
                        bool meleeMonster = UnityEngine.Random.Range(0, 100) < spawnPoints[i].meleePossibility;
                        foreach (var singleMonster in levelMonsterListSO.levelMonsterList)
                        {
                            Enemy enemy = singleMonster.GetComponent<Enemy>();
                            if (enemy != null)
                            {
                                EnemyType enemyType = enemy.GetEnemyType();
                                if(meleeMonster && enemyType == EnemyType.Melee)
                                {
                                    canGenerateMonsterList.Add(singleMonster);
                                }
                                else if(!meleeMonster && enemyType == EnemyType.Archer)
                                {
                                    canGenerateMonsterList.Add(singleMonster);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var singleMonster in levelMonsterListSO.levelMonsterList)
                        {
                            Enemy enemy = singleMonster.GetComponent<Enemy>();
                            if (enemy != null)
                            {
                                EnemyType enemyType = enemy.GetEnemyType();
                                if (enemyType == EnemyType.Air)
                                {
                                    canGenerateMonsterList.Add(singleMonster);
                                }
                                else
                                    continue;
                            }
                        }
                    }

                    GameObject monster = Instantiate(
                        canGenerateMonsterList[UnityEngine.Random.Range(0,canGenerateMonsterList.Count)],
                        spawnPos,
                        Quaternion.identity,
                        monsterParentTransform);

                    levelMonsterDatas.Add(new LevelMonsterData(enemyIndex, monster, roomData.roomID, i, false, 0, 0));

                    MonsterStats monsterStats = monster.GetComponent<MonsterStats>();
                    if (monsterStats != null)
                    {
                        monsterStats.SetRoomMonsterInfo(enemyIndex, roomData.roomID, i);
                    }
                }

                normalRoomCounter++;
            }
        }

        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log($"花费时间 {elapsedTime}s来生成敌人");
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
        for(int i = 0; i < levelMonsterDatas.Count; i++)
        {
            if (levelMonsterDatas[i].monster != null)
            {
                levelMonsterDatas[i].SetPosition(levelMonsterDatas[i].monster.transform.position.x, levelMonsterDatas[i].monster.transform.position.y);
            }
        }

    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener(EventName.OnEnemyDead, KillEnemy);

    }



    #region 铁匠相关
    /// <summary>
    /// 查看等级够不够进行接下来的操作。
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public bool IsLevelLimit(int level)
    {
        if(level >= 6)
        {
            return true;
        }
        return false;
    }

    #endregion

    private void KillEnemy(object sender, EventArgs e)
    {
        Debug.Log("敌人死亡！");
        var data = e as OnEnemyDeadEventArgs;
        if (data != null)
        {
            //
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


[Serializable]
public class LevelMonsterData
{
    int monsterIndex;
    public GameObject monster;
    private int roomIndex;
    int roomSpawnPointIndex;
    private bool isDead;
    private float xPosition;
    private float yPosition;
    
    public LevelMonsterData(int monsterIndex, GameObject monster, int roomIndex, int roomSpawnPointIndex, bool isDead, float xPosition, float yPosition)
    {
        this.monsterIndex = monsterIndex;
        this.monster = monster;
        this.roomIndex = roomIndex;
        this.roomSpawnPointIndex = roomSpawnPointIndex;
        this.isDead = isDead;
        this.xPosition = xPosition;
        this.yPosition = yPosition;
    }

    public void SetPosition(float x,float y)
    {
        xPosition = x;
        yPosition = y;
    }
}