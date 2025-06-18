using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseZoneGenerator : MonoBehaviour
{

    [Header("配置捏")]
    [SerializeField] private GameObject tileZonePrefab;
    [SerializeField] private List<Vector2Int> zonePosList;
    [SerializeField] private float zoneStayTime;
    [SerializeField] private float zoneGenerateInterval;
    [SerializeField] private float zoneDetermineInterval;
    [SerializeField] private bool isDetermineOnlyOnce;

    [Header("伤害过的敌人id 先用instanceID")]
    [SerializeField] private List<int> damagedEnemyIdList;



    private Coroutine spawnCoroutine;

    public void Initialize(List<Vector2Int> zonePosList, float stayTime, float generateInterval,
        float determineInterval, bool determineOnlyOnce, BaseEquipment _baseEquipment)
    {
        this.zonePosList = zonePosList;
        this.zoneStayTime = stayTime;
        this.zoneGenerateInterval = generateInterval;
        this.zoneDetermineInterval = determineInterval;
        this.isDetermineOnlyOnce = determineOnlyOnce;
        damagedEnemyIdList = new List<int>();

        spawnCoroutine = StartCoroutine(GenerateZonesCoroutine());
    }

    private IEnumerator GenerateZonesCoroutine()
    {
        foreach (Vector2Int pos in zonePosList)
        {
            Vector3 worldPos = (Vector2)pos + new Vector2(0.5f, 0f);
            GameObject zone = Instantiate(tileZonePrefab, worldPos, Quaternion.identity);

            ZoneController controller = zone.GetComponent<ZoneController>();
            controller.Initialize(zoneStayTime, zoneDetermineInterval, isDetermineOnlyOnce, this);

            yield return new WaitForSeconds(zoneGenerateInterval);
        }
    }

    public void RegisterDamagedEnemy(int enemyID)
    {
        if (!damagedEnemyIdList.Contains(enemyID))
        {
            damagedEnemyIdList.Add(enemyID);
        }
    }

    public bool HasDamaged(int enemyID)
    {
        return damagedEnemyIdList.Contains(enemyID);
    }




}
