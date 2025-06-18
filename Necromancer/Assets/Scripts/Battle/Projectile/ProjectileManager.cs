using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : SingletonManagerBase<ProjectileManager>
{

    [SerializeField] private GameObject projectileGeneratorPrefab;

    [SerializeField] private GameObject zoneGeneratorPrefab;


    protected override void Awake()
    {
        base.Awake(); // 必须保留：处理单例与DDOL
    }

    public void GenerateProjectile(ProjectileSO _projectileSO,BaseEquipment _baseEquipment,bool facingRight ,Enemy _enemy,int combo)
    {
        GameObject obj =  ObjectPoolManager.SpawnObject(projectileGeneratorPrefab, transform.position, Quaternion.identity,ObjectPoolManager.PoolType.Projectiles);
        Debug.Log(_projectileSO.ToString() + "_projectileSO");
        BaseProjectileGenerator generator = obj.GetComponent<BaseProjectileGenerator>();
        generator.Initialize(_projectileSO, _baseEquipment, facingRight, _enemy, combo);
    }

    public void GenerateZone(List<Vector2Int> zonePosList, float stayTime, float generateInterval,
        float determineInterval, bool determineOnlyOnce, BaseEquipment _baseEquipment)
    {
        GameObject obj = ObjectPoolManager.SpawnObject(zoneGeneratorPrefab, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.Zone);
        BaseZoneGenerator generator = obj.GetComponent<BaseZoneGenerator>();
        generator.Initialize(zonePosList, stayTime, generateInterval, determineInterval, determineOnlyOnce, _baseEquipment);
    }


}
